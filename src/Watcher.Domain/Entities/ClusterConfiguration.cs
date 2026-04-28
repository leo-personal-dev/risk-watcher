namespace Watcher.Domain.Entities;

public class ClusterConfiguration
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int ScoreMin { get; set; }
    public int ScoreMax { get; set; }
    public int AgeMin { get; set; }
    public int AgeMax { get; set; }
    public string DebtRule { get; set; } = null!;
    public decimal BaseLimit { get; set; }
    public decimal Cap { get; set; }
}