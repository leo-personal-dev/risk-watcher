using MediatR;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.Handlers;

public class CreateJobCategoryCommandHandler : IRequestHandler<CreateJobCategoryCommand, CreateJobCategoryResponse>
{
    private readonly IJobCategoryService _jobCategoryService;

    public CreateJobCategoryCommandHandler(IJobCategoryService jobCategoryService)
    {
        _jobCategoryService = jobCategoryService;
    }

    public async Task<CreateJobCategoryResponse> Handle(CreateJobCategoryCommand request, CancellationToken cancellationToken)
    {
        var jobCategory = new JobCategory
        {
            Name = request.Name,
            Multiplier = request.Multiplier,
            Keywords = request.Keywords
        };

        var created = await _jobCategoryService.CreateAsync(jobCategory);

        return new CreateJobCategoryResponse
        {
            Id = created.Id,
            Name = created.Name,
            Multiplier = created.Multiplier,
            Keywords = created.Keywords
        };
    }
}