using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;
using Watcher.Domain.Entities;

namespace Watcher.Api.IntegrationTests;

public class CustomerClassificationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CustomerClassificationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostCustomersClassify_ReturnsCustomerCluster()
    {
        var client = _factory.CreateClient();
        var request = new ClassifyCustomerCommand
        {
            CustomerId = "123",
            Name = "John Doe",
            Score = 750,
            Age = 35,
            HasMarketDebt = true,
            MarketDebtTypes = new List<string> { "credit_card" },
            JobTitle = "Chief Executive Officer",
            location = new Location
            {
                City = "São Paulo",
                State = "SP",
                Region = "Sudeste"
            }
        };

        var response = await client.PostAsJsonAsync("/customers/classify", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ClassifyCustomerResponse>();

        Assert.NotNull(result);
        Assert.Equal(request.CustomerId, result!.CustomerId);
        Assert.Equal("CLUSTER_B", result.ClusterId);
        Assert.Equal("Gold", result.ClusterName);
        Assert.NotNull(result.JobCategoryId);
        Assert.Equal("EXECUTIVE", result.JobCategoryId);
        Assert.Equal("Executive", result.JobCategoryName);
        Assert.InRange(result.CreditLimit, 20000m, 40000m);
        Assert.True(result.CalculatedAt > DateTime.MinValue);
    }
}