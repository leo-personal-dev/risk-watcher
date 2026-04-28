namespace Watcher.Domain.Entities;

public class Customer
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public int Score { get; set; }
    public bool HasMarketDebt { get; set; }
    public List<string> MarketDebtTypes { get; set; } = new();
    public Location Location { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string Cluster { get; set; } = null!;
}