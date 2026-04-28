using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;
using Watcher.Domain.Services;
using Xunit;

namespace Watcher.Domain.UnitTests;

public class JobCategoryServiceTests
{
    [Fact]
    public async Task CreateAsync_AddsNewCategory()
    {
        // Arrange
        var repository = new InMemoryJobCategoryRepository(seedExecutive: false);
        var service = new JobCategoryService(repository);
        var category = new JobCategory
        {
            Name = "Test Category",
            Multiplier = 1.5m,
            Keywords = new List<string> { "test", "sample" }
        };

        // Act
        var result = await service.CreateAsync(category);

        // Assert
        Assert.Equal("TEST_CATEGORY", result.Id);
        Assert.Equal("Test Category", result.Name);
        var all = await repository.GetAllAsync();
        Assert.Single(all);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenCategoryExists()
    {
        // Arrange
        var repository = new InMemoryJobCategoryRepository();
        var service = new JobCategoryService(repository);
        var category = new JobCategory
        {
            Id = "EXECUTIVE",
            Name = "Test Category",
            Multiplier = 1.5m,
            Keywords = new List<string> { "test" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(category));
    }

    [Fact]
    public async Task UpdateAsync_Throws_WhenCategoryMissing()
    {
        // Arrange
        var repository = new InMemoryJobCategoryRepository();
        var service = new JobCategoryService(repository);
        var category = new JobCategory
        {
            Id = "MISSING",
            Name = "Test Category",
            Multiplier = 1.5m,
            Keywords = new List<string> { "test" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(category));
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingCategory()
    {
        // Arrange
        var repository = new InMemoryJobCategoryRepository(seedExecutive: false);
        var service = new JobCategoryService(repository);
        var category = new JobCategory
        {
            Name = "Test Category",
            Multiplier = 1.5m,
            Keywords = new List<string> { "test" }
        };
        await service.CreateAsync(category);

        // Act
        await service.DeleteAsync("TEST_CATEGORY");

        // Assert
        var all = await repository.GetAllAsync();
        Assert.Empty(all);
    }

    [Fact]
    public async Task IdentifyCategoryAsync_ReturnsMatchingCategory()
    {
        // Arrange
        var repository = new InMemoryJobCategoryRepository();
        var service = new JobCategoryService(repository);

        // Act
        var result = await service.IdentifyCategoryAsync("Chief Executive Officer");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EXECUTIVE", result.Id);
    }

    [Fact]
    public async Task IdentifyCategoryAsync_ReturnsOtherCategory_WhenNoMatch()
    {
        // Arrange
        var repository = new InMemoryJobCategoryRepository();
        var service = new JobCategoryService(repository);

        // Act
        var result = await service.IdentifyCategoryAsync("Unknown Job Title");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OTHER", result.Id);
    }

    [Fact]
    public async Task IdentifyCategoryAsync_ReturnsHighestPriorityMatch_WhenMultipleCategoriesMatch()
    {
        // Arrange
        var repository = new InMemoryJobCategoryRepository(new[]
        {
            new JobCategory
            {
                Id = "MID_PROFESSIONAL",
                Name = "Mid Professional",
                Multiplier = 1.0m,
                Priority = 2,
                Keywords = new List<string> { "Engineer" }
            },
            new JobCategory
            {
                Id = "SENIOR_PROFESSIONAL",
                Name = "Senior Professional",
                Multiplier = 1.5m,
                Priority = 3,
                Keywords = new List<string> { "Engineer", "Senior" }
            }
        });
        var service = new JobCategoryService(repository);

        // Act
        var result = await service.IdentifyCategoryAsync("Senior Engineer");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("SENIOR_PROFESSIONAL", result!.Id);
    }

    [Fact]
    public async Task IdentifyCategoryAsync_ReturnsDeterministicMatch_ForEqualPriorityCategories()
    {
        // Arrange
        var repository = new InMemoryJobCategoryRepository(new[]
        {
            new JobCategory
            {
                Id = "A_CATEGORY",
                Name = "A Category",
                Multiplier = 1.0m,
                Priority = 2,
                Keywords = new List<string> { "Engineer" }
            },
            new JobCategory
            {
                Id = "B_CATEGORY",
                Name = "B Category",
                Multiplier = 1.0m,
                Priority = 2,
                Keywords = new List<string> { "Engineer" }
            }
        });
        var service = new JobCategoryService(repository);

        // Act
        var result = await service.IdentifyCategoryAsync("Engineer");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("B_CATEGORY", result!.Id);
    }

    private sealed class InMemoryJobCategoryRepository : IJobCategoryRepository
    {
        private readonly Dictionary<string, JobCategory> _categories;

        public InMemoryJobCategoryRepository(bool seedExecutive = true)
        {
            _categories = seedExecutive
                ? new Dictionary<string, JobCategory>
                {
                    ["EXECUTIVE"] = new JobCategory
                    {
                        Id = "EXECUTIVE",
                        Name = "Executive",
                        Multiplier = 2.0m,
                        Keywords = new List<string> { "CEO", "Chief", "Executive" }
                    }
                }
                : new Dictionary<string, JobCategory>();
        }

        public InMemoryJobCategoryRepository(IEnumerable<JobCategory> seedCategories)
        {
            _categories = seedCategories.ToDictionary(c => c.Id);
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
            _categories[category.Id] = category;
            return Task.FromResult(category);
        }

        public Task<JobCategory> UpdateAsync(JobCategory category)
        {
            _categories[category.Id] = category;
            return Task.FromResult(category);
        }

        public Task DeleteAsync(string id)
        {
            _categories.Remove(id);
            return Task.CompletedTask;
        }
    }
}