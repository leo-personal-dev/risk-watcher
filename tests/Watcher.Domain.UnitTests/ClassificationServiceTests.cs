using System.Collections.Generic;
using System.Linq;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;
using Watcher.Domain.Services;
using Xunit;

namespace Watcher.Domain.UnitTests;

public class ClassificationServiceTests
{
    private readonly ClassificationService _service;

    public ClassificationServiceTests()
    {
        _service = new ClassificationService(new StubClusterConfigurationService(), new StubJobCategoryService(), new StubMonthlyIncomeMappingService(), new StubPenaltyRuleService());
    }

    private sealed class StubClusterConfigurationService : IClusterConfigurationService
    {
        public Task<ClusterConfiguration> CreateAsync(ClusterConfiguration cluster) => throw new NotImplementedException();
        public Task DeleteAsync(string id) => throw new NotImplementedException();
        public Task<IEnumerable<ClusterConfiguration>> GetAllAsync()
        {
            var clusters = new List<ClusterConfiguration>
            {
                new ClusterConfiguration
                {
                    Id = "CLUSTER_A",
                    Name = "Diamond",
                    ScoreMin = 700,
                    ScoreMax = 1000,
                    AgeMin = 25,
                    AgeMax = 60,
                    DebtRule = "!customer.HasMarketDebt",
                    BaseLimit = 50000m,
                    Cap = 100000m
                },
                new ClusterConfiguration
                {
                    Id = "CLUSTER_B",
                    Name = "Gold",
                    ScoreMin = 500,
                    ScoreMax = 1000,
                    AgeMin = 18,
                    AgeMax = 65,
                    DebtRule = "!customer.MarketDebtTypes.Contains(\"credit_default\")",
                    BaseLimit = 20000m,
                    Cap = 40000m
                },
                new ClusterConfiguration
                {
                    Id = "CLUSTER_C",
                    Name = "Silver",
                    ScoreMin = 300,
                    ScoreMax = 1000,
                    AgeMin = 18,
                    AgeMax = 150,
                    DebtRule = "true",
                    BaseLimit = 5000m,
                    Cap = 10000m
                },
                new ClusterConfiguration
                {
                    Id = "CLUSTER_D",
                    Name = "Bronze",
                    ScoreMin = 0,
                    ScoreMax = 299,
                    AgeMin = 18,
                    AgeMax = 150,
                    DebtRule = "true",
                    BaseLimit = 0m,
                    Cap = 0m
                }
            };
            return Task.FromResult<IEnumerable<ClusterConfiguration>>(clusters);
        }

        public Task<ClusterConfiguration?> GetByIdAsync(string id) => throw new NotImplementedException();
        public Task<ClusterConfiguration> UpdateAsync(ClusterConfiguration cluster) => throw new NotImplementedException();
    }

    private sealed class StubJobCategoryService : IJobCategoryService
    {
        public Task<JobCategory> IdentifyCategoryAsync(string jobTitle)
        {
            return Task.FromResult<JobCategory>(new JobCategory
            {
                Id = "EXECUTIVE",
                Name = "Executive",
                Multiplier = 2.0m,
                Keywords = new List<string> { "CEO", "Chief", "Executive" }
            });
        }

        public Task<JobCategory> CreateAsync(JobCategory category) => throw new NotImplementedException();
        public Task DeleteAsync(string id) => throw new NotImplementedException();
        public Task<IEnumerable<JobCategory>> GetAllAsync() => throw new NotImplementedException();
        public Task<JobCategory?> GetByIdAsync(string id) => throw new NotImplementedException();
        public Task<JobCategory> UpdateAsync(JobCategory category) => throw new NotImplementedException();
    }

    private sealed class StubMonthlyIncomeMappingService : IMonthlyIncomeMappingService
    {
        public Task<MonthlyIncomeMapping> CreateAsync(MonthlyIncomeMapping mapping) => throw new NotImplementedException();
        public Task DeleteAsync(string clusterId, string jobCategoryId) => throw new NotImplementedException();
        public Task<IEnumerable<MonthlyIncomeMapping>> GetAllAsync() => throw new NotImplementedException();
        public Task<MonthlyIncomeMapping?> GetByIdAsync(string clusterId, string jobCategoryId)
        {            
            return Task.FromResult<MonthlyIncomeMapping?>(new MonthlyIncomeMapping
            {
                ClusterId = clusterId,
                JobCategoryId = jobCategoryId,
                Value = 30000m
            });
        }
        public Task<MonthlyIncomeMapping> UpdateAsync(MonthlyIncomeMapping mapping) => throw new NotImplementedException();
    }

    private sealed class StubPenaltyRuleService : IPenaltyRuleService
    {
        public Task<decimal> GetPenaltyMultiplierAsync(Cluster cluster, Customer customer)        {
            return Task.FromResult(0.9m);
        }
    }

    [Fact]
    public async Task ClassifyAsync_ReturnsClusterA_ForHighScoreNoDebtAndMatchedAgeRange()
    {
        var customer = new Customer
        {
            Id = "1",
            Age = 30,
            Score = 750,
            HasMarketDebt = false,
            MarketDebtTypes = new List<string>(),
            JobTitle = "Chief Executive Officer",
            Location = new Location { City = "São Paulo", State = "SP", Region = "Sudeste" }
        };

        var result = await _service.ClassifyAsync(customer);

        Assert.Equal("CLUSTER_A", result.Cluster.IdCluster);
        Assert.Equal("Diamond", result.Cluster.Name);
        Assert.Equal("EXECUTIVE", result.JobCategory?.Id);
        Assert.InRange(result.CreditLimit, 50000m, 100000m);
    }

    [Fact]
    public async Task ClassifyAsync_IncludesJobCategory_WhenJobTitleMatches()
    {
        var customer = new Customer
        {
            Id = "1",
            Age = 30,
            Score = 750,
            HasMarketDebt = false,
            MarketDebtTypes = new List<string>(),
            JobTitle = "Chief Executive Officer",
            Location = new Location { City = "São Paulo", State = "SP", Region = "Sudeste" }
        };

        var result = await _service.ClassifyAsync(customer);

        Assert.NotNull(result.JobCategory);
        Assert.Equal("EXECUTIVE", result.JobCategory.Id);
        Assert.Equal("Executive", result.JobCategory.Name);
    }

    [Fact]
    public async Task ClassifyAsync_ReturnsClusterB_ForScoreAtLeast500WithoutDefaultDebt()
    {
        var customer = new Customer
        {
            Id = "2",
            Age = 35,
            Score = 550,
            HasMarketDebt = true,
            MarketDebtTypes = new List<string> { "loan_default" },
            Location = new Location { City = "Rio de Janeiro", State = "RJ", Region = "Sudeste" }
        };

        var result = await _service.ClassifyAsync(customer);

        Assert.Equal("CLUSTER_B", result.Cluster.IdCluster);
        Assert.Equal("Gold", result.Cluster.Name);
        Assert.InRange(result.CreditLimit, 20000m, 40000m);
    }

    [Fact]
    public async Task ClassifyAsync_ReturnsClusterC_ForScoreAtLeast300AndNotHigherPriorities()
    {
        var customer = new Customer
        {
            Id = "3",
            Age = 70,
            Score = 350,
            HasMarketDebt = false,
            MarketDebtTypes = new List<string>(),
            Location = new Location { City = "Belo Horizonte", State = "MG", Region = "Sudeste" }
        };

        var result = await _service.ClassifyAsync(customer);

        Assert.Equal("CLUSTER_C", result.Cluster.IdCluster);
        Assert.Equal("Silver", result.Cluster.Name);
        Assert.InRange(result.CreditLimit, 5000m, 10000m);
    }

    [Fact]
    public async Task ClassifyAsync_ThrowsException_ForScoreBelow0()
    {
        var customer = new Customer
        {
            Id = "4",
            Age = 50,
            Score = -100,
            HasMarketDebt = true,
            MarketDebtTypes = new List<string> { "credit_default" },
            Location = new Location { City = "Salvador", State = "BA", Region = "Nordeste" }
        };

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.ClassifyAsync(customer));
        Assert.Contains("Score must be between 0 and 1000", exception.Message);
    }

    [Fact]
    public async Task ClassifyAsync_ReturnsCreditLimitWithinClusterBoundaries_ForClusterA()
    {
        var customer = new Customer
        {
            Id = "5",
            Age = 60,
            Score = 700,
            HasMarketDebt = false,
            MarketDebtTypes = new List<string>(),
            Location = new Location { City = "Curitiba", State = "PR", Region = "Sul" }
        };

        var result = await _service.ClassifyAsync(customer);

        Assert.Equal("CLUSTER_A", result.Cluster.IdCluster);
        Assert.InRange(result.CreditLimit, 50000m, 100000m);
    }

    [Fact]
    public async Task ClassifyAsync_Throws_WhenScoreOutOfRange()
    {
        var customer = new Customer
        {
            Id = "6",
            Age = 40,
            Score = -100,
            HasMarketDebt = false,
            MarketDebtTypes = new List<string>(),
            Location = new Location { City = "Brasilia", State = "DF", Region = "Centro-Oeste" }
        };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.ClassifyAsync(customer));
    }

    [Fact]
    public async Task ClassifyAsync_Throws_WhenAgeOutOfRange()
    {
        var customer = new Customer
        {
            Id = "7",
            Age = 17,
            Score = 500,
            HasMarketDebt = false,
            MarketDebtTypes = new List<string>(),
            Location = new Location { City = "Brasilia", State = "DF", Region = "Centro-Oeste" }
        };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.ClassifyAsync(customer));
    }
}