namespace Watcher.Domain.Commands.Response;

public class ClassifyCustomerResponse
{
    public string CustomerId { get; set; } = null!;
    public string ClusterId { get; set; } = null!;
    public string ClusterName { get; set; } = null!;
    public string? JobCategoryId { get; set; }
    public string? JobCategoryName { get; set; }
    public decimal MonthlyIncomeValue { get; set; }
    public decimal PenaltyEffectValue { get; set; }
    public decimal CreditLimit { get; set; }
    public DateTime CalculatedAt { get; set; }
}