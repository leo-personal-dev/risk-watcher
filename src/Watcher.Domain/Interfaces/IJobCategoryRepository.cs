using Watcher.Domain.Entities;

namespace Watcher.Domain.Interfaces;

public interface IJobCategoryRepository
{
    Task<IEnumerable<JobCategory>> GetAllAsync();
    Task<JobCategory?> GetByIdAsync(string id);
    Task<JobCategory> CreateAsync(JobCategory category);
    Task<JobCategory> UpdateAsync(JobCategory category);
    Task DeleteAsync(string id);
}