using System.Threading.Tasks;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.Services;

public class ClassificationService : IClassificationService
{
    private readonly IClusterConfigurationService _clusterConfigurationService;
    private readonly IJobCategoryService _jobCategoryService;
    private readonly IMonthlyIncomeMappingService _monthlyIncomeMappingService;
    private readonly IPenaltyRuleService _penaltyRuleService;

    public ClassificationService(IClusterConfigurationService clusterConfigurationService, IJobCategoryService jobCategoryService, IMonthlyIncomeMappingService monthlyIncomeMappingService, IPenaltyRuleService penaltyRuleService)
    {
        _clusterConfigurationService = clusterConfigurationService;
        _jobCategoryService = jobCategoryService;
        _monthlyIncomeMappingService = monthlyIncomeMappingService;
        _penaltyRuleService = penaltyRuleService;
    }

    public async Task<ClassificationResult> ClassifyAsync(Customer customer)
    {
        ValidateCustomer(customer);

        var cluster = await EvaluateClusterAsync(customer);
        var jobCategory = await _jobCategoryService.IdentifyCategoryAsync(customer.JobTitle);
        var monthlyIncomeMapping = await _monthlyIncomeMappingService.GetByIdAsync(cluster.IdCluster, jobCategory.Id);
        var penaltyEffect = await _penaltyRuleService.GetPenaltyMultiplierAsync(cluster, customer);
        var creditLimit = CalculateCreditLimit(cluster, jobCategory, penaltyEffect);

        var result = new ClassificationResult(
            customer.Id,
            cluster,
            jobCategory,
            monthlyIncomeMapping?.Value ?? 0m,
            penaltyEffect,
            creditLimit,
            DateTime.UtcNow
        );

        return result;
    }

    private static void ValidateCustomer(Customer customer)
    {
        if (customer.Score < 0 || customer.Score > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(customer.Score), "Score must be between 0 and 1000.");
        }

        if (customer.Age < 18 || customer.Age > 150)
        {
            throw new ArgumentOutOfRangeException(nameof(customer.Age), "Age must be between 18 and 150.");
        }
    }

    private async Task<Cluster> EvaluateClusterAsync(Customer customer)
    {
        var configurations = await _clusterConfigurationService.GetAllAsync();

        foreach (var configuration in configurations)
        {
            if (await MatchesConfiguration(customer, configuration))
            {
                return new Cluster(configuration.Id, configuration.Name, configuration.BaseLimit, configuration.Cap);
            }
        }

        return ClusterDefinitions.CLUSTER_D;
    }

    private static async Task<bool> MatchesConfiguration(Customer customer, ClusterConfiguration configuration)
    {
        if (customer.Score < configuration.ScoreMin || customer.Score > configuration.ScoreMax)
        {
            return false;
        }

        if (customer.Age < configuration.AgeMin || customer.Age > configuration.AgeMax)
        {
            return false;
        }

        return await ClusterRuleEvaluator.Evaluate(configuration.DebtRule, customer);
    }

    private static decimal CalculateCreditLimit(Cluster cluster, JobCategory jobCategory, decimal penaltyFactor)
    {
        if (cluster.IdCluster == "CLUSTER_D")
        {
            return 0m;
        }

        return RoundToNearest100(Math.Min(cluster.BaseLimit * jobCategory.Multiplier * penaltyFactor, cluster.Cap));
    }

    private static decimal RoundToNearest100(decimal value)
    {
        return Math.Round(value / 100, MidpointRounding.AwayFromZero) * 100;
    }
}