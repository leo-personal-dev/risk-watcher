using MediatR;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.Handlers;

public class UpdateJobCategoryCommandHandler : IRequestHandler<UpdateJobCategoryCommand, UpdateJobCategoryResponse>
{
    private readonly IJobCategoryService _jobCategoryService;

    public UpdateJobCategoryCommandHandler(IJobCategoryService jobCategoryService)
    {
        _jobCategoryService = jobCategoryService;
    }

    public async Task<UpdateJobCategoryResponse> Handle(UpdateJobCategoryCommand request, CancellationToken cancellationToken)
    {
        var jobCategory = new JobCategory
        {
            Id = request.Id,
            Name = request.Name,
            Multiplier = request.Multiplier,
            Keywords = request.Keywords
        };

        var updated = await _jobCategoryService.UpdateAsync(jobCategory);

        return new UpdateJobCategoryResponse
        {
            Id = updated.Id,
            Name = updated.Name,
            Multiplier = updated.Multiplier,
            Keywords = updated.Keywords
        };
    }
}