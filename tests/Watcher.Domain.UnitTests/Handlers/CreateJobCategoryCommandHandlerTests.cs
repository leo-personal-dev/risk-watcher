using Moq;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;
using Watcher.Domain.Entities;
using Watcher.Domain.Handlers;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.UnitTests.Handlers;

public class CreateJobCategoryCommandHandlerTests
{
    private readonly Mock<IJobCategoryService> _jobCategoryServiceMock;
    private readonly CreateJobCategoryCommandHandler _handler;

    public CreateJobCategoryCommandHandlerTests()
    {
        _jobCategoryServiceMock = new Mock<IJobCategoryService>();
        _handler = new CreateJobCategoryCommandHandler(_jobCategoryServiceMock.Object);
    }

    [Fact]
    public async Task Handle_CreatesCategory_ReturnsCreatedCategory()
    {
        // Arrange
        var command = new CreateJobCategoryCommand
        {
            Name = "Tech",
            Multiplier = 1.2m,
            Keywords = new List<string> { "engineer", "developer" }
        };

        var createdCategory = new JobCategory
        {
            Id = "JC001",
            Name = "Tech",
            Multiplier = 1.2m,
            Keywords = new List<string> { "engineer", "developer" }
        };

        _jobCategoryServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<JobCategory>()))
            .ReturnsAsync(createdCategory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("JC001", result.Id);
        Assert.Equal("Tech", result.Name);
        Assert.Equal(1.2m, result.Multiplier);
        Assert.Contains("engineer", result.Keywords);
    }

    [Fact]
    public async Task Handle_CallsServiceWithCorrectData()
    {
        // Arrange
        var command = new CreateJobCategoryCommand
        {
            Name = "Finance",
            Multiplier = 1.1m,
            Keywords = new List<string> { "analyst" }
        };

        _jobCategoryServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<JobCategory>()))
            .ReturnsAsync(new JobCategory { Id = "JC002", Name = "Finance", Multiplier = 1.1m, Keywords = new List<string> { "analyst" } });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _jobCategoryServiceMock.Verify(s => s.CreateAsync(It.Is<JobCategory>(jc =>
            jc.Name == "Finance" &&
            jc.Multiplier == 1.1m &&
            jc.Keywords.Contains("analyst")
        )), Times.Once);
    }
}