using Moq;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;
using Watcher.Domain.Entities;
using Watcher.Domain.Handlers;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.UnitTests.Handlers;

public class UpdateJobCategoryCommandHandlerTests
{
    private readonly Mock<IJobCategoryService> _jobCategoryServiceMock;
    private readonly UpdateJobCategoryCommandHandler _handler;

    public UpdateJobCategoryCommandHandlerTests()
    {
        _jobCategoryServiceMock = new Mock<IJobCategoryService>();
        _handler = new UpdateJobCategoryCommandHandler(_jobCategoryServiceMock.Object);
    }

    [Fact]
    public async Task Handle_UpdatesCategory_ReturnsUpdatedCategory()
    {
        // Arrange
        var command = new UpdateJobCategoryCommand
        {
            Id = "JC001",
            Name = "Updated Tech",
            Multiplier = 1.3m,
            Keywords = new List<string> { "senior engineer" }
        };

        var updatedCategory = new JobCategory
        {
            Id = "JC001",
            Name = "Updated Tech",
            Multiplier = 1.3m,
            Keywords = new List<string> { "senior engineer" }
        };

        _jobCategoryServiceMock
            .Setup(s => s.UpdateAsync(It.IsAny<JobCategory>()))
            .ReturnsAsync(updatedCategory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("JC001", result.Id);
        Assert.Equal("Updated Tech", result.Name);
        Assert.Equal(1.3m, result.Multiplier);
        Assert.Contains("senior engineer", result.Keywords);
    }

    [Fact]
    public async Task Handle_CallsServiceWithCorrectId()
    {
        // Arrange
        var command = new UpdateJobCategoryCommand
        {
            Id = "JC001",
            Name = "Test",
            Multiplier = 1.0m,
            Keywords = new List<string> { "test" }
        };

        _jobCategoryServiceMock
            .Setup(s => s.UpdateAsync(It.IsAny<JobCategory>()))
            .ReturnsAsync(new JobCategory { Id = "JC001", Name = "Test", Multiplier = 1.0m, Keywords = new List<string> { "test" } });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _jobCategoryServiceMock.Verify(s => s.UpdateAsync(It.Is<JobCategory>(jc =>
            jc.Id == "JC001"
        )), Times.Once);
    }
}