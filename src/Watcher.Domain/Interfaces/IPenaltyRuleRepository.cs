

using Watcher.Domain.Entities;

namespace Watcher.Domain.Interfaces;

public interface IPenaltyRuleRepository
{
    /// <summary>
    /// Retrieves all penalty rules for a given cluster.
    /// </summary>
    Task<IEnumerable<PenaltyRuleConfiguration>> GetAllAsync();
}