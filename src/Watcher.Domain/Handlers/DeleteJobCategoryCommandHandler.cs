using MediatR;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.Handlers;

public class DeleteJobCategoryCommandHandler : IRequestHandler<DeleteJobCategoryCommand, Unit>
{
    private readonly IJobCategoryService _jobCategoryService;

    public DeleteJobCategoryCommandHandler(IJobCategoryService jobCategoryService)
    {
        _jobCategoryService = jobCategoryService;
    }

    public async Task<Unit> Handle(DeleteJobCategoryCommand request, CancellationToken cancellationToken)
    {
        await _jobCategoryService.DeleteAsync(request.JobCategoryId);
        return Unit.Value;
    }
}