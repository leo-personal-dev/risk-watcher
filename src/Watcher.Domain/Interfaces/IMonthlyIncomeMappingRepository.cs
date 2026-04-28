
using Watcher.Domain.Entities;

namespace Watcher.Domain.Interfaces;

public interface IMonthlyIncomeMappingRepository {
    Task<IEnumerable<MonthlyIncomeMapping>> GetAllAsync();
    Task<MonthlyIncomeMapping?> GetByIdAsync(string clusterId, string jobCategoryId);
    Task<MonthlyIncomeMapping> CreateAsync(MonthlyIncomeMapping mapping);
    Task<MonthlyIncomeMapping> UpdateAsync(MonthlyIncomeMapping mapping);
    Task DeleteAsync(string clusterId, string jobCategoryId);
}