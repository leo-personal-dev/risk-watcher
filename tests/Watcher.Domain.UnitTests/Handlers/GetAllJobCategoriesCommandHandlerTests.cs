using Moq;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;
using Watcher.Domain.Entities;
using Watcher.Domain.Handlers;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.UnitTests.Handlers;

public class GetAllJobCategoriesCommandHandlerTests
{
    private readonly Mock<IJobCategoryService> _jobCategoryServiceMock;
    private readonly GetAllJobCategoriesCommandHandler _handler;

    public GetAllJobCategoriesCommandHandlerTests()
    {
        _jobCategoryServiceMock = new Mock<IJobCategoryService>();
        _handler = new GetAllJobCategoriesCommandHandler(_jobCategoryServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<JobCategory>
        {
            new JobCategory { Id = "JC001", Name = "Tech", Multiplier = 1.2m, Keywords = new List<string> { "engineer" } },
            new JobCategory { Id = "JC002", Name = "Finance", Multiplier = 1.1m, Keywords = new List<string> { "analyst" } }
        };

        _jobCategoryServiceMock
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(categories);

        var command = new GetAllJobCategoriesCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.JobCategories.Count);
        Assert.Equal("JC001", result.JobCategories[0].Id);
        Assert.Equal("Tech", result.JobCategories[0].Name);
        Assert.Equal("JC002", result.JobCategories[1].Id);
        Assert.Equal("Finance", result.JobCategories[1].Name);
    }

    [Fact]
    public async Task Handle_EmptyList_ReturnsEmptyResponse()
    {
        // Arrange
        _jobCategoryServiceMock
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<JobCategory>());

        var command = new GetAllJobCategoriesCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(result.JobCategories);
    }
}