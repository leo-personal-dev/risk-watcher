using MediatR;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.Handlers;

public class GetAllJobCategoriesCommandHandler : IRequestHandler<GetAllJobCategoriesCommand, GetAllJobCategoriesResponse>
{
    private readonly IJobCategoryService _jobCategoryService;

    public GetAllJobCategoriesCommandHandler(IJobCategoryService jobCategoryService)
    {
        _jobCategoryService = jobCategoryService;
    }

    public async Task<GetAllJobCategoriesResponse> Handle(GetAllJobCategoriesCommand request, CancellationToken cancellationToken)
    {
        var jobCategories = await _jobCategoryService.GetAllAsync();

        var response = new GetAllJobCategoriesResponse
        {
            JobCategories = jobCategories.Select(jc => new GetJobCategoryResponse
            {
                Id = jc.Id,
                Name = jc.Name,
                Multiplier = jc.Multiplier,
                Keywords = jc.Keywords
            }).ToList()
        };

        return response;
    }
}