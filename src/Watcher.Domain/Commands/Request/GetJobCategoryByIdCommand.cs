using MediatR;
using Watcher.Domain.Commands.Response;

namespace Watcher.Domain.Commands.Request;

public class GetJobCategoryByIdCommand : IRequest<GetJobCategoryResponse>
{
    public string JobCategoryId { get; set; } = null!;
}