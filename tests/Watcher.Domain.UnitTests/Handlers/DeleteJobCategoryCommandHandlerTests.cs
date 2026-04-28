using Moq;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Handlers;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.UnitTests.Handlers;

public class DeleteJobCategoryCommandHandlerTests
{
    private readonly Mock<IJobCategoryService> _jobCategoryServiceMock;
    private readonly DeleteJobCategoryCommandHandler _handler;

    public DeleteJobCategoryCommandHandlerTests()
    {
        _jobCategoryServiceMock = new Mock<IJobCategoryService>();
        _handler = new DeleteJobCategoryCommandHandler(_jobCategoryServiceMock.Object);
    }

    [Fact]
    public async Task Handle_DeletesCategory()
    {
        // Arrange
        var command = new DeleteJobCategoryCommand { JobCategoryId = "JC001" };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _jobCategoryServiceMock.Verify(s => s.DeleteAsync("JC001"), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsUnit()
    {
        // Arrange
        var command = new DeleteJobCategoryCommand { JobCategoryId = "JC001" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(MediatR.Unit.Value, result);
    }
}