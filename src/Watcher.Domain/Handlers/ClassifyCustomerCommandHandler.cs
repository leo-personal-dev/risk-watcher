using MediatR;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.Handlers;

public class ClassifyCustomerCommandHandler : IRequestHandler<ClassifyCustomerCommand, ClassifyCustomerResponse>
{
    private readonly IClassificationService _classificationService;

    public ClassifyCustomerCommandHandler(IClassificationService classificationService)
    {
        _classificationService = classificationService;
    }

    public async Task<ClassifyCustomerResponse> Handle(ClassifyCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = request.CustomerId,
            Name = request.Name,
            Score = request.Score,
            Age = request.Age,
            HasMarketDebt = request.HasMarketDebt,
            MarketDebtTypes = request.MarketDebtTypes,
            JobTitle = request.JobTitle,
            Location = request.location
        };

        var result = await _classificationService.ClassifyAsync(customer);

        return new ClassifyCustomerResponse
        {
            CustomerId = result.CustomerId,
            ClusterId = result.Cluster.IdCluster,
            ClusterName = result.Cluster.Name,
            JobCategoryId = result.JobCategory?.Id,
            JobCategoryName = result.JobCategory?.Name,
            MonthlyIncomeValue = result.MonthlyIncomeValue,
            PenaltyEffectValue = result.PenaltyEffectValue,
            CreditLimit = result.CreditLimit,
            CalculatedAt = result.CalculatedAt
        };
    }
}