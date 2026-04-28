using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Infrastructure.Mocks;

public class ClusterConfigurationRepository : IClusterConfigurationRepository
{
    private readonly List<ClusterConfiguration> _clusters = new()
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
            DebtRule = "!(customer.MarketDebtTypes.Contains(\"credit_default\") || customer.MarketDebtTypes.Contains(\"loan_default\"))",
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

    private readonly object _lock = new();

    public Task<IEnumerable<ClusterConfiguration>> GetAllAsync()
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<ClusterConfiguration>>(_clusters.Select(Clone).ToList());
        }
    }

    public Task<ClusterConfiguration?> GetByIdAsync(string id)
    {
        lock (_lock)
        {
            var cluster = _clusters.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(cluster is null ? null : Clone(cluster));
        }
    }

    public Task AddAsync(ClusterConfiguration cluster)
    {
        lock (_lock)
        {
            _clusters.Add(Clone(cluster));
            return Task.CompletedTask;
        }
    }

    public Task UpdateAsync(ClusterConfiguration cluster)
    {
        lock (_lock)
        {
            var index = _clusters.FindIndex(x => string.Equals(x.Id, cluster.Id, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                throw new KeyNotFoundException($"Cluster '{cluster.Id}' not found.");
            }
            _clusters[index] = Clone(cluster);
            return Task.CompletedTask;
        }
    }

    public Task DeleteAsync(string id)
    {
        lock (_lock)
        {
            var cluster = _clusters.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
            if (cluster == null)
            {
                throw new KeyNotFoundException($"Cluster '{id}' not found.");
            }
            _clusters.Remove(cluster);
            return Task.CompletedTask;
        }
    }

    private static ClusterConfiguration Clone(ClusterConfiguration cluster)
    {
        return new ClusterConfiguration
        {
            Id = cluster.Id,
            Name = cluster.Name,
            ScoreMin = cluster.ScoreMin,
            ScoreMax = cluster.ScoreMax,
            AgeMin = cluster.AgeMin,
            AgeMax = cluster.AgeMax,
            DebtRule = cluster.DebtRule,
            BaseLimit = cluster.BaseLimit,
            Cap = cluster.Cap
        };
    }
}