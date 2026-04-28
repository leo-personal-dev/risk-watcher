using MediatR;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.Handlers;

public class GetJobCategoryByIdCommandHandler : IRequestHandler<GetJobCategoryByIdCommand, GetJobCategoryResponse>
{
    private readonly IJobCategoryService _jobCategoryService;

    public GetJobCategoryByIdCommandHandler(IJobCategoryService jobCategoryService)
    {
        _jobCategoryService = jobCategoryService;
    }

    public async Task<GetJobCategoryResponse> Handle(GetJobCategoryByIdCommand request, CancellationToken cancellationToken)
    {
        var jobCategory = await _jobCategoryService.GetByIdAsync(request.JobCategoryId);

        if (jobCategory == null)
        {
            throw new KeyNotFoundException($"Job category with ID '{request.JobCategoryId}' not found.");
        }

        return new GetJobCategoryResponse
        {
            Id = jobCategory.Id,
            Name = jobCategory.Name,
            Multiplier = jobCategory.Multiplier,
            Keywords = jobCategory.Keywords
        };
    }
}