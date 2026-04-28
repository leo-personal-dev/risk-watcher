

using Watcher.Domain.Entities;

namespace Watcher.Domain.Interfaces;

public interface IPenaltyRuleService
{
    /// <summary>
    /// Retrieves the penalty multiplier for a given cluster and job category combination.
    /// If no specific mapping exists, returns null to indicate no penalty applies.
    /// </summary>
    Task<decimal> GetPenaltyMultiplierAsync(Cluster cluster, Customer customer);
}