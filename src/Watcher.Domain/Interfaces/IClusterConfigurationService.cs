using Watcher.Domain.Entities;

namespace Watcher.Domain.Interfaces;

public interface IClusterConfigurationService
{
    Task<IEnumerable<ClusterConfiguration>> GetAllAsync();
    Task<ClusterConfiguration?> GetByIdAsync(string id);
    Task<ClusterConfiguration> CreateAsync(ClusterConfiguration cluster);
    Task<ClusterConfiguration> UpdateAsync(ClusterConfiguration cluster);
    Task DeleteAsync(string id);
}