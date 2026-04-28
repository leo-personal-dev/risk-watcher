namespace Watcher.Domain.Entities;

public record ClassificationResult(
    string CustomerId,
    Cluster Cluster,
    JobCategory JobCategory,
    decimal MonthlyIncomeValue,
    decimal PenaltyEffectValue,
    decimal CreditLimit,
    DateTime CalculatedAt
);