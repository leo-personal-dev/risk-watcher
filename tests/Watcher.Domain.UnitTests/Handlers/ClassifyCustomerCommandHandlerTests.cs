using Moq;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;
using Watcher.Domain.Entities;
using Watcher.Domain.Handlers;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.UnitTests.Handlers;

public class ClassifyCustomerCommandHandlerTests
{
    private readonly Mock<IClassificationService> _classificationServiceMock;
    private readonly ClassifyCustomerCommandHandler _handler;

    public ClassifyCustomerCommandHandlerTests()
    {
        _classificationServiceMock = new Mock<IClassificationService>();
        _handler = new ClassifyCustomerCommandHandler(_classificationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCorrectResponse()
    {
        // Arrange
        var command = new ClassifyCustomerCommand
        {
            CustomerId = "CUST001",
            Score = 750,
            Age = 30,
            HasMarketDebt = false,
            MarketDebtTypes = new List<string>(),
            JobTitle = "Engineer"
        };

        var expectedResult = new ClassificationResult(
            CustomerId: "CUST001",
            Cluster: ClusterDefinitions.CLUSTER_A,
            JobCategory: new JobCategory { Id = "JC001", Name = "Tech", Multiplier = 1.2m, Keywords = new List<string> { "engineer" } },
            MonthlyIncomeValue: 30000m,
            PenaltyEffectValue: 0.9m,
            CreditLimit: 50000,
            CalculatedAt: DateTime.UtcNow
        );

        _classificationServiceMock
            .Setup(s => s.ClassifyAsync(It.IsAny<Customer>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("CUST001", result.CustomerId);
        Assert.Equal("CLUSTER_A", result.ClusterId);
        Assert.Equal("Diamond", result.ClusterName);
        Assert.Equal("JC001", result.JobCategoryId);
        Assert.Equal("Tech", result.JobCategoryName);
        Assert.Equal(50000, result.CreditLimit);
        Assert.NotEqual(default, result.CalculatedAt);
    }

    [Fact]
    public async Task Handle_CallsServiceWithCorrectCustomer()
    {
        // Arrange
        var command = new ClassifyCustomerCommand
        {
            CustomerId = "CUST001",
            Score = 750,
            Age = 30,
            HasMarketDebt = true,
            MarketDebtTypes = new List<string> { "credit_card" },
            JobTitle = "Engineer"
        };

        _classificationServiceMock
            .Setup(s => s.ClassifyAsync(It.IsAny<Customer>()))
            .ReturnsAsync(new ClassificationResult(
                CustomerId: "CUST001",
                Cluster: ClusterDefinitions.CLUSTER_A,
                JobCategory: new JobCategory { Id = "JC001", Name = "Tech", Multiplier = 1.2m, Keywords = new List<string> { "engineer" } },
                MonthlyIncomeValue: 30000m,
                PenaltyEffectValue: 0.9m,
                CreditLimit: 50000,
                CalculatedAt: DateTime.UtcNow
            ));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _classificationServiceMock.Verify(s => s.ClassifyAsync(It.Is<Customer>(c =>
            c.Id == "CUST001" &&
            c.Score == 750 &&
            c.Age == 30 &&
            c.HasMarketDebt == true &&
            c.MarketDebtTypes.Contains("credit_card") &&
            c.JobTitle == "Engineer"
        )), Times.Once);
    }
}