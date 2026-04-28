using Watcher.Domain.Entities;

namespace Watcher.Domain.Interfaces;

public interface IJobCategoryService
{
    Task<JobCategory> CreateAsync(JobCategory category);
    Task<JobCategory> UpdateAsync(JobCategory category);
    Task DeleteAsync(string id);
    Task<IEnumerable<JobCategory>> GetAllAsync();
    Task<JobCategory?> GetByIdAsync(string id);

    /// <summary>
    /// Identifies the best-fit job category for a job title using priority-ordered category evaluation.
    /// If multiple categories match, the last category found after ordering categories by Priority is returned.
    /// </summary>
    Task<JobCategory> IdentifyCategoryAsync(string jobTitle);
}