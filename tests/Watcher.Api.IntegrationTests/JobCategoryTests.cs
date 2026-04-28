using System.Net;
using System.Net.Http.Json;
using Xunit;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;

using Microsoft.AspNetCore.Mvc.Testing;

namespace Watcher.Api.IntegrationTests;

public class JobCategoryTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public JobCategoryTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededCategories()
    {
        // Act
        var response = await _client.GetAsync("/api/job-categories");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GetAllJobCategoriesResponse>();
        Assert.NotNull(result);
        Assert.True(result.JobCategories.Count >= 4); // At least the seeded ones
    }

    [Fact]
    public async Task Create_ReturnsCreatedCategory()
    {
        // Arrange
        var request = new CreateJobCategoryCommand
        {
            Name = "New Category",
            Multiplier = 1.8m,
            Keywords = new List<string> { "new", "test" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/job-categories", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<GetJobCategoryResponse>();
        Assert.NotNull(created);
        Assert.Equal("NEW_CATEGORY", created.Id);
        Assert.Equal("New Category", created.Name);
    }

    [Fact]
    public async Task Update_ReturnsUpdatedCategory()
    {
        // Arrange
        var request = new UpdateJobCategoryCommand
        {
            Id = "EXECUTIVE",
            Name = "Updated Executive",
            Multiplier = 2.5m,
            Keywords = new List<string> { "CEO", "CFO", "Chief" }
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/job-categories/EXECUTIVE", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<UpdateJobCategoryResponse>();
        Assert.NotNull(updated);
        Assert.Equal("EXECUTIVE", updated.Id);
        Assert.Equal("Updated Executive", updated.Name);
        Assert.Equal(2.5m, updated.Multiplier);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange - Create a category to delete
        var createRequest = new CreateJobCategoryCommand
        {
            Name = "To Delete",
            Multiplier = 1.0m,
            Keywords = new List<string> { "delete" }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/job-categories", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<GetJobCategoryResponse>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/job-categories/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsCategory()
    {
        // Act
        var response = await _client.GetAsync("/api/job-categories/EXECUTIVE");

        // Assert
        response.EnsureSuccessStatusCode();
        var category = await response.Content.ReadFromJsonAsync<GetJobCategoryResponse>();
        Assert.NotNull(category);
        Assert.Equal("EXECUTIVE", category.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_ForMissingCategory()
    {
        // Act
        var response = await _client.GetAsync("/api/job-categories/MISSING");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}