using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Watcher.Domain.Entities;
using Xunit;

namespace Watcher.Api.IntegrationTests;

public class ClusterConfigurationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ClusterConfigurationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostClusters_CreatesCluster_AndListIncludesIt()
    {
        var client = _factory.CreateClient();
        var request = new ClusterConfiguration
        {
            Id = "TEST_CLUSTER",
            Name = "Test Cluster",
            ScoreMin = 650,
            ScoreMax = 900,
            AgeMin = 25,
            AgeMax = 70,
            DebtRule = "!customer.HasMarketDebt",
            BaseLimit = 25000m,
            Cap = 50000m
        };

        var createResponse = await client.PostAsJsonAsync("/clusters", request);
        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<ClusterConfiguration>();
        Assert.NotNull(created);
        Assert.Equal(request.Id, created!.Id);

        var listResponse = await client.GetAsync("/clusters");
        listResponse.EnsureSuccessStatusCode();

        var configurations = await listResponse.Content.ReadFromJsonAsync<List<ClusterConfiguration>>();
        Assert.NotNull(configurations);
        Assert.Contains(configurations!, item => item.Id == request.Id);
    }

    [Fact]
    public async Task DeleteClusters_RemovesCluster_AndReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var request = new ClusterConfiguration
        {
            Id = "TEST_CLUSTER_DELETE",
            Name = "Delete Cluster",
            ScoreMin = 300,
            ScoreMax = 400,
            AgeMin = 18,
            AgeMax = 60,
            DebtRule = "true",
            BaseLimit = 1000m,
            Cap = 2000m
        };

        var createResponse = await client.PostAsJsonAsync("/clusters", request);
        createResponse.EnsureSuccessStatusCode();

        var deleteResponse = await client.DeleteAsync($"/clusters/{request.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/clusters/{request.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
