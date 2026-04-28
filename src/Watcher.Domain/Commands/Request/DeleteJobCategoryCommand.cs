using MediatR;

namespace Watcher.Domain.Commands.Request;

public class DeleteJobCategoryCommand : IRequest<Unit>
{
    public string JobCategoryId { get; set; } = null!;
}