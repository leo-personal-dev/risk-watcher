namespace Watcher.Domain.Entities;

public class JobCategory
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Multiplier { get; set; }
    public int Priority { get; set; }
    public List<string> Keywords { get; set; } = new();
}