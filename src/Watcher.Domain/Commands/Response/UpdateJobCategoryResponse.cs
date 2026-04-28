namespace Watcher.Domain.Commands.Response;

public class UpdateJobCategoryResponse
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Multiplier { get; set; }
    public List<string> Keywords { get; set; } = new();
}