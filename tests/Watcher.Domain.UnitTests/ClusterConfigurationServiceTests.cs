using System.Collections.Generic;
using System.Threading.Tasks;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;
using Watcher.Domain.Services;
using Xunit;

namespace Watcher.Domain.UnitTests;

public class ClusterConfigurationServiceTests
{
    [Fact]
    public async Task CreateAsync_AddsNewCluster()
    {
        var repository = new InMemoryClusterConfigurationRepository();
        var service = new ClusterConfigurationService(repository);

        var cluster = new ClusterConfiguration
        {
            Id = "CUSTOM_A",
            Name = "Custom A",
            ScoreMin = 600,
            ScoreMax = 800,
            AgeMin = 25,
            AgeMax = 65,
            DebtRule = "!customer.HasMarketDebt",
            BaseLimit = 25000m,
            Cap = 50000m
        };

        var created = await service.CreateAsync(cluster);

        Assert.Equal(cluster.Id, created.Id);
        Assert.Collection(await repository.GetAllAsync(), item => Assert.Equal(cluster.Id, item.Id));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenIdAlreadyExists()
    {
        var repository = new InMemoryClusterConfigurationRepository();
        var service = new ClusterConfigurationService(repository);

        var cluster = new ClusterConfiguration
        {
            Id = "CUSTOM_A",
            Name = "Custom A",
            ScoreMin = 600,
            ScoreMax = 800,
            AgeMin = 25,
            AgeMax = 65,
            DebtRule = "!customer.HasMarketDebt",
            BaseLimit = 25000m,
            Cap = 50000m
        };

        await service.CreateAsync(cluster);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(cluster));
    }

    [Fact]
    public async Task UpdateAsync_Throws_WhenClusterMissing()
    {
        var repository = new InMemoryClusterConfigurationRepository();
        var service = new ClusterConfigurationService(repository);

        var cluster = new ClusterConfiguration
        {
            Id = "NOT_FOUND",
            Name = "Missing",
            ScoreMin = 300,
            ScoreMax = 400,
            AgeMin = 18,
            AgeMax = 60,
            DebtRule = "true",
            BaseLimit = 5000m,
            Cap = 10000m
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(cluster));
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingCluster()
    {
        var repository = new InMemoryClusterConfigurationRepository();
        var service = new ClusterConfigurationService(repository);

        var cluster = new ClusterConfiguration
        {
            Id = "CUSTOM_B",
            Name = "Custom B",
            ScoreMin = 400,
            ScoreMax = 600,
            AgeMin = 20,
            AgeMax = 60,
            DebtRule = "true",
            BaseLimit = 10000m,
            Cap = 20000m
        };

        await service.CreateAsync(cluster);
        await service.DeleteAsync(cluster.Id);

        var loaded = await repository.GetByIdAsync(cluster.Id);
        Assert.Null(loaded);
    }

    private sealed class InMemoryClusterConfigurationRepository : IClusterConfigurationRepository
    {
        private readonly List<ClusterConfiguration> _items = new();

        public Task AddAsync(ClusterConfiguration cluster)
        {
            _items.Add(Clone(cluster));
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var item = _items.Find(x => x.Id == id);
            if (item != null)
            {
                _items.Remove(item);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ClusterConfiguration>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<ClusterConfiguration>>(_items.Select(Clone).ToList());
        }

        public Task<ClusterConfiguration?> GetByIdAsync(string id)
        {
            var item = _items.Find(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(item is null ? null : Clone(item));
        }

        public Task UpdateAsync(ClusterConfiguration cluster)
        {
            var index = _items.FindIndex(x => string.Equals(x.Id, cluster.Id, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                _items[index] = Clone(cluster);
            }
            else
            {
                throw new KeyNotFoundException();
            }
            return Task.CompletedTask;
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
}
