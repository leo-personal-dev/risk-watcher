using MediatR;
using Watcher.Domain.Commands.Response;
using Watcher.Domain.Entities;

namespace Watcher.Domain.Commands.Request;

public class ClassifyCustomerCommand : IRequest<ClassifyCustomerResponse>
{
    public string CustomerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public int Score { get; set; }
    public bool HasMarketDebt { get; set; }
    public List<string> MarketDebtTypes { get; set; } = new();
    public Location location { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
}