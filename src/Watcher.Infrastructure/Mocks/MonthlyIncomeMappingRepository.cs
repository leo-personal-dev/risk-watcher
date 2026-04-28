
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Infrastructure.Mocks;

public class MonthlyIncomeMappingRepository : IMonthlyIncomeMappingRepository {
    private readonly List<MonthlyIncomeMapping> _mappings = new();

    public MonthlyIncomeMappingRepository() {
        // Initialize with some default mappings
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_A", JobCategoryId = "EXECUTIVE", Value = 30000m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_A", JobCategoryId = "SENIOR_PROFESSIONAL", Value = 20000m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_A", JobCategoryId = "MID_PROFESSIONAL", Value = 12000m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_A", JobCategoryId = "JUNIOR_PROFESSIONAL", Value = 8000m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_A", JobCategoryId = "OTHER", Value = 10000m });

        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_B", JobCategoryId = "EXECUTIVE", Value = 20000m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_B", JobCategoryId = "SENIOR_PROFESSIONAL", Value = 15000m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_B", JobCategoryId = "MID_PROFESSIONAL", Value = 8000m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_B", JobCategoryId = "JUNIOR_PROFESSIONAL", Value = 5000m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_B", JobCategoryId = "OTHER", Value = 6500m });

        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_C", JobCategoryId = "EXECUTIVE", Value = 10000m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_C", JobCategoryId = "SENIOR_PROFESSIONAL", Value = 7000m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_C", JobCategoryId = "MID_PROFESSIONAL", Value = 5000m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_C", JobCategoryId = "JUNIOR_PROFESSIONAL", Value = 3000m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_C", JobCategoryId = "OTHER", Value = 4000m });

        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_D", JobCategoryId = "EXECUTIVE", Value = 0m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_D", JobCategoryId = "SENIOR_PROFESSIONAL", Value = 0m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_D", JobCategoryId = "MID_PROFESSIONAL", Value = 0m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_D", JobCategoryId = "JUNIOR_PROFESSIONAL", Value = 0m });
        _mappings.Add(new MonthlyIncomeMapping { ClusterId = "CLUSTER_D", JobCategoryId = "OTHER", Value = 0m });
    }

    public Task<IEnumerable<MonthlyIncomeMapping>> GetAllAsync() {
        return Task.FromResult(_mappings.AsEnumerable());
    }

    public Task<MonthlyIncomeMapping?> GetByIdAsync(string clusterId, string jobCategoryId) {
        var mapping = _mappings.FirstOrDefault(m => m.ClusterId == clusterId && m.JobCategoryId == jobCategoryId);
        return Task.FromResult(mapping);
    }

    public Task<MonthlyIncomeMapping> CreateAsync(MonthlyIncomeMapping mapping) {
        _mappings.Add(mapping);
        return Task.FromResult(mapping);
    }

    public Task<MonthlyIncomeMapping> UpdateAsync(MonthlyIncomeMapping mapping) {
        var existing = _mappings.FirstOrDefault(m => m.ClusterId == mapping.ClusterId && m.JobCategoryId == mapping.JobCategoryId);
        if (existing != null) {
            existing.JobCategoryId = mapping.JobCategoryId;
            existing.Value = mapping.Value;
        }
        return Task.FromResult(existing ?? mapping);
    }

    public Task DeleteAsync(string clusterId, string jobCategoryId) {
        var existing = _mappings.FirstOrDefault(m => m.ClusterId == clusterId && m.JobCategoryId == jobCategoryId);
        if (existing != null) {
            _mappings.Remove(existing);
        }
        return Task.CompletedTask;
    }
}   