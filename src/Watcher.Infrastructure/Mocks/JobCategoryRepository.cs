using System.Collections.Concurrent;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Infrastructure.Mocks;

public class JobCategoryRepository : IJobCategoryRepository
{
    private readonly ConcurrentDictionary<string, JobCategory> _categories = new();

    public JobCategoryRepository()
    {
        // Seed with default categories
        _categories["EXECUTIVE"] = new JobCategory
        {
            Id = "EXECUTIVE",
            Name = "Executive",
            Multiplier = 2.0m,
            Priority = 1,
            Keywords = new List<string> { "CEO", "CFO", "CTO", "COO", "CIO", "CMO", "Chief", "President", "Vice President", "VP", "Director" }
        };

        _categories["SENIOR_PROFESSIONAL"] = new JobCategory
        {
            Id = "SENIOR_PROFESSIONAL",
            Name = "Senior Professional",
            Multiplier = 1.5m,
            Priority = 2,
            Keywords = new List<string> { "Senior", "Principal", "Lead", "Manager", "Supervisor" }
        };

        _categories["MID_PROFESSIONAL"] = new JobCategory
        {
            Id = "MID_PROFESSIONAL",
            Name = "Mid Professional",
            Multiplier = 1.0m,
            Priority = 3,
            Keywords = new List<string> { "Engineer", "Analyst", "Developer", "Specialist", "Designer", "Accountant", "Consultant", "Architect" }
        };

        _categories["JUNIOR_PROFESSIONAL"] = new JobCategory
        {
            Id = "JUNIOR_PROFESSIONAL",
            Name = "Junior Professional",
            Multiplier = 0.7m,
            Priority = 4,
            Keywords = new List<string> { "Junior", "Trainee", "Intern", "Apprentice", "Assistant", "Associate" }
        };
    }

    public Task<IEnumerable<JobCategory>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<JobCategory>>(_categories.Values.ToList());
    }

    public Task<JobCategory?> GetByIdAsync(string id)
    {
        _categories.TryGetValue(id, out var category);
        return Task.FromResult(category);
    }

    public Task<JobCategory> CreateAsync(JobCategory category)
    {
        if (_categories.ContainsKey(category.Id))
        {
            throw new InvalidOperationException($"Job category with ID '{category.Id}' already exists.");
        }

        _categories[category.Id] = category;
        return Task.FromResult(category);
    }

    public Task<JobCategory> UpdateAsync(JobCategory category)
    {
        if (!_categories.ContainsKey(category.Id))
        {
            throw new KeyNotFoundException($"Job category with ID '{category.Id}' not found.");
        }

        _categories[category.Id] = category;
        return Task.FromResult(category);
    }

    public Task DeleteAsync(string id)
    {
        if (!_categories.TryRemove(id, out _))
        {
            throw new KeyNotFoundException($"Job category with ID '{id}' not found.");
        }

        return Task.CompletedTask;
    }
}