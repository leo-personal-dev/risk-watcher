
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.Services;

public class MonthlyIncomeMappingService : IMonthlyIncomeMappingService {
    private readonly IMonthlyIncomeMappingRepository _repository;

    public MonthlyIncomeMappingService(IMonthlyIncomeMappingRepository repository) {
        _repository = repository;
    }

    public Task<IEnumerable<MonthlyIncomeMapping>> GetAllAsync() {
        return _repository.GetAllAsync();
    }

    public Task<MonthlyIncomeMapping?> GetByIdAsync(string clusterId, string jobCategoryId) {
        return _repository.GetByIdAsync(clusterId, jobCategoryId);
    }

    public Task<MonthlyIncomeMapping> CreateAsync(MonthlyIncomeMapping mapping) {
        return _repository.CreateAsync(mapping);
    }

    public Task<MonthlyIncomeMapping> UpdateAsync(MonthlyIncomeMapping mapping) {
        return _repository.UpdateAsync(mapping);
    }

    public Task DeleteAsync(string clusterId, string jobCategoryId) {
        return _repository.DeleteAsync(clusterId, jobCategoryId);
    }
}