using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.Services;

public class JobCategoryService : IJobCategoryService
{
    private readonly IJobCategoryRepository _repository;

    public JobCategoryService(IJobCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<JobCategory> CreateAsync(JobCategory category)
    {
        ValidateCategory(category);

        // Generate ID from name if not provided
        if (string.IsNullOrEmpty(category.Id))
        {
            category.Id = category.Name.ToUpper().Replace(" ", "_");
        }

        // Check if ID already exists
        var existing = await _repository.GetByIdAsync(category.Id);
        if (existing != null)
        {
            throw new InvalidOperationException($"Job category with ID '{category.Id}' already exists.");
        }

        return await _repository.CreateAsync(category);
    }

    public async Task<JobCategory> UpdateAsync(JobCategory category)
    {
        ValidateCategory(category);

        var existing = await _repository.GetByIdAsync(category.Id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Job category with ID '{category.Id}' not found.");
        }

        return await _repository.UpdateAsync(category);
    }

    public async Task DeleteAsync(string id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Job category with ID '{id}' not found.");
        }

        await _repository.DeleteAsync(id);
    }

    public Task<IEnumerable<JobCategory>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<JobCategory?> GetByIdAsync(string id)
    {
        return _repository.GetByIdAsync(id);
    }

    public async Task<JobCategory> IdentifyCategoryAsync(string jobTitle)
    {
        if (string.IsNullOrWhiteSpace(jobTitle))
        {
            throw new ArgumentException("Job title is required.");
        }

        var categories = await _repository.GetAllAsync();

        JobCategory? matchedCategory = null;
        foreach (var category in categories
                     .OrderBy(c => c.Priority)
                     .ThenBy(c => c.Id, StringComparer.Ordinal))
        {
            if (category.Keywords.Any(k => jobTitle.Contains(k, StringComparison.OrdinalIgnoreCase)))
            {
                matchedCategory = category;
            }
        }

        if(matchedCategory == null){
            matchedCategory = new JobCategory
            {
                Id = "OTHER",
                Name = "Other",
                Multiplier = 0.8m,
                Priority = 5,
                Keywords = new List<string>()
            };
        }

        return matchedCategory;
    }

    private static void ValidateCategory(JobCategory category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            throw new ArgumentException("Job category name is required.");
        }

        if (category.Multiplier <= 0)
        {
            throw new ArgumentException("Job category multiplier must be greater than 0.");
        }

        if (category.Keywords == null || !category.Keywords.Any())
        {
            throw new ArgumentException("Job category must have at least one keyword.");
        }
    }
}