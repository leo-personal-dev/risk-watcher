
namespace Watcher.Domain.Entities;
public class PenaltyRuleConfiguration
{
    public string Id { get; set; } = null!;
    public int Priority { get; set; }
    public string Trigger { get; set; } = null!;
    public decimal Effect { get; set; }
}