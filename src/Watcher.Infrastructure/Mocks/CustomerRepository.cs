using System.Collections.Concurrent;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Infrastructure.Mocks;

public class CustomerRepository : ICustomerRepository
{
    private readonly ConcurrentDictionary<string, Customer> _store = new();

    public Task AddAsync(Customer customer)
    {
        _store[customer.Id] = customer;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Customer>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Customer>>(_store.Values.ToList());
    }

    public Task<Customer?> GetByIdAsync(string id)
    {
        return Task.FromResult(_store.TryGetValue(id, out var customer) ? customer : null);
    }

    public Task UpdateAsync(Customer customer)
    {
        _store[customer.Id] = customer;
        return Task.CompletedTask;
    }
}