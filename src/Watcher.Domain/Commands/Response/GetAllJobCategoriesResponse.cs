namespace Watcher.Domain.Commands.Response;

public class GetAllJobCategoriesResponse
{
    public List<GetJobCategoryResponse> JobCategories { get; set; } = new();
}