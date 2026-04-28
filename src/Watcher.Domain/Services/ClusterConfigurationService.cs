using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.Services;

public class ClusterConfigurationService : IClusterConfigurationService
{
    private readonly IClusterConfigurationRepository _repository;

    public ClusterConfigurationService(IClusterConfigurationRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<ClusterConfiguration>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<ClusterConfiguration?> GetByIdAsync(string id)
    {
        return _repository.GetByIdAsync(id);
    }

    public async Task<ClusterConfiguration> CreateAsync(ClusterConfiguration cluster)
    {
        ValidateCluster(cluster);
        var existing = await _repository.GetByIdAsync(cluster.Id);
        if (existing != null)
        {
            throw new InvalidOperationException($"Cluster '{cluster.Id}' already exists.");
        }

        await _repository.AddAsync(cluster);
        return cluster;
    }

    public async Task<ClusterConfiguration> UpdateAsync(ClusterConfiguration cluster)
    {
        ValidateCluster(cluster);
        var existing = await _repository.GetByIdAsync(cluster.Id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Cluster '{cluster.Id}' not found.");
        }

        await _repository.UpdateAsync(cluster);
        return cluster;
    }

    public async Task DeleteAsync(string id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Cluster '{id}' not found.");
        }

        await _repository.DeleteAsync(id);
    }

    private static void ValidateCluster(ClusterConfiguration cluster)
    {
        if (string.IsNullOrWhiteSpace(cluster.Id))
        {
            throw new ArgumentException("Cluster Id is required.", nameof(cluster.Id));
        }

        if (string.IsNullOrWhiteSpace(cluster.Name))
        {
            throw new ArgumentException("Cluster Name is required.", nameof(cluster.Name));
        }

        if (cluster.ScoreMin < 300 || cluster.ScoreMin > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(cluster.ScoreMin), "ScoreMin must be between 300 and 1000.");
        }

        if (cluster.ScoreMax < 300 || cluster.ScoreMax > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(cluster.ScoreMax), "ScoreMax must be between 300 and 1000.");
        }

        if (cluster.ScoreMax < cluster.ScoreMin)
        {
            throw new ArgumentException("ScoreMax must be greater than or equal to ScoreMin.", nameof(cluster.ScoreMax));
        }

        if (cluster.AgeMin < 18 || cluster.AgeMin > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(cluster.AgeMin), "AgeMin must be between 18 and 100.");
        }

        if (cluster.AgeMax < 18 || cluster.AgeMax > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(cluster.AgeMax), "AgeMax must be between 18 and 100.");
        }

        if (cluster.AgeMax < cluster.AgeMin)
        {
            throw new ArgumentException("AgeMax must be greater than or equal to AgeMin.", nameof(cluster.AgeMax));
        }

        if (cluster.BaseLimit < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(cluster.BaseLimit), "BaseLimit must be non-negative.");
        }

        if (cluster.Cap < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(cluster.Cap), "Cap must be non-negative.");
        }

        if (cluster.Cap < cluster.BaseLimit)
        {
            throw new ArgumentException("Cap must be greater than or equal to BaseLimit.", nameof(cluster.Cap));
        }

        if (string.IsNullOrWhiteSpace(cluster.DebtRule))
        {
            throw new ArgumentException("DebtRule is required.", nameof(cluster.DebtRule));
        }

        _ = ClusterRuleEvaluator.Evaluate(cluster.DebtRule, new Customer
        {
            Id = "__validation__",
            Name = "Validation",
            Age = cluster.AgeMin,
            Score = cluster.ScoreMin,
            HasMarketDebt = false,
            MarketDebtTypes = new List<string>(),
            Location = new Location { City = string.Empty, State = string.Empty, Region = string.Empty },
            JobTitle = string.Empty
        });
    }
}