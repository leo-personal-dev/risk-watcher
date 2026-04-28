using Watcher.Domain.Entities;

namespace Watcher.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(string id);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(string id);
}