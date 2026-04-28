using Watcher.Domain.Entities;

namespace Watcher.Domain.Interfaces;

public interface IClusterConfigurationRepository
{
    Task<IEnumerable<ClusterConfiguration>> GetAllAsync();
    Task<ClusterConfiguration?> GetByIdAsync(string id);
    Task AddAsync(ClusterConfiguration cluster);
    Task UpdateAsync(ClusterConfiguration cluster);
    Task DeleteAsync(string id);
}