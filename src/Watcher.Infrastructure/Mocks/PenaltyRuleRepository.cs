
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Infrastructure.Mocks;
public class PenaltyRuleRepository : IPenaltyRuleRepository
{
    private readonly List<PenaltyRuleConfiguration> _mappings = new()
    {
        new PenaltyRuleConfiguration { Id = "PR001", Priority = 1, Trigger = "customer.MarketDebtTypes.Contains(\"credit_default\") || customer.MarketDebtTypes.Contains(\"loan_default\")", Effect = 0.5m }
        // Add more mappings as needed
    };

    public Task<IEnumerable<PenaltyRuleConfiguration>> GetAllAsync()
    {
        return Task.FromResult(_mappings.AsEnumerable());
    }
}