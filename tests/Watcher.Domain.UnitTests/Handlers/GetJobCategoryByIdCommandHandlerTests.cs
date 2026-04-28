using Moq;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;
using Watcher.Domain.Entities;
using Watcher.Domain.Handlers;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.UnitTests.Handlers;

public class GetJobCategoryByIdCommandHandlerTests
{
    private readonly Mock<IJobCategoryService> _jobCategoryServiceMock;
    private readonly GetJobCategoryByIdCommandHandler _handler;

    public GetJobCategoryByIdCommandHandlerTests()
    {
        _jobCategoryServiceMock = new Mock<IJobCategoryService>();
        _handler = new GetJobCategoryByIdCommandHandler(_jobCategoryServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingCategory_ReturnsCategory()
    {
        // Arrange
        var category = new JobCategory
        {
            Id = "JC001",
            Name = "Tech",
            Multiplier = 1.2m,
            Keywords = new List<string> { "engineer", "developer" }
        };

        _jobCategoryServiceMock
            .Setup(s => s.GetByIdAsync("JC001"))
            .ReturnsAsync(category);

        var command = new GetJobCategoryByIdCommand { JobCategoryId = "JC001" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("JC001", result.Id);
        Assert.Equal("Tech", result.Name);
        Assert.Equal(1.2m, result.Multiplier);
        Assert.Contains("engineer", result.Keywords);
    }

    [Fact]
    public async Task Handle_NonExistingCategory_ThrowsKeyNotFoundException()
    {
        // Arrange
        _jobCategoryServiceMock
            .Setup(s => s.GetByIdAsync("JC999"))
            .ReturnsAsync((JobCategory)null);

        var command = new GetJobCategoryByIdCommand { JobCategoryId = "JC999" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}