# Quickstart Guide: Update Risk Rules

**Date**: 2026-04-25  
**Feature**: Update Risk Rules  
**Audience**: Backend developers implementing the feature

---

## Overview

This guide helps you implement the updated customer classification rules feature. The feature:

1. **Classifies customers** into risk clusters (CLUSTER_A through CLUSTER_D) using priority-based business rules
2. **Returns cluster information** including limits for each cluster
3. **Calculates credit limits** based on customer score and cluster limits
4. **Maintains backward compatibility** with existing API consumers

---

## Feature Scope

### What's New
- ✅ Enhanced `POST /customers/classify` endpoint returns full cluster object
- ✅ New `Cluster` entity with ID, Name, BaseLimit, and Cap
- ✅ Calculated credit limit per customer based on score
- ✅ Reorganized domain project structure (Handlers, Services, Interfaces, etc.)

### What's Unchanged
- ✅ Same endpoint URL (`/customers/classify`)
- ✅ Same request body structure
- ✅ HTTP 200 response for valid requests
- ✅ MediatR/CQRS command pattern
- ✅ Existing Customer entity structure

---

## Architecture Overview

```
┌──────────────┐
│ API Layer    │
│ Controller   │  Receives HTTP request
└──────┬───────┘
       │ Maps to MediatR command
       ▼
┌──────────────────┐
│ Application Layer│
│ MediatR Handler  │  Orchestrates command execution
└──────┬───────────┘
       │ Delegates to service
       ▼
┌──────────────────┐
│ Domain Layer     │
│ Classification   │  Implements business rules
│ Service          │  Returns Cluster + CreditLimit
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│ Response Mapping │
│ AutoMapper       │  DTO transformation
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│ HTTP Response    │
│ 200 OK with body │  Full cluster object
└──────────────────┘
```

---

## Project Structure

### Domain Project Organization

After implementation, your `Watcher.Domain` project should look like:

```
src/Watcher.Domain/
├── Entities/
│   ├── Customer.cs              (existing, enhanced)
│   ├── Cluster.cs              (NEW)
│   └── ClassificationResult.cs (NEW)
├── Services/
│   ├── IClassificationService.cs (existing interface)
│   └── ClassificationService.cs  (existing, enhanced)
├── Interfaces/
│   ├── IClassificationService.cs
│   └── ...
├── Commands/
│   └── ClassifyCustomerCommand.cs (existing)
├── Queries/
│   └── ...
├── Handlers/
│   └── ClassifyCustomerHandler.cs (NEW or moved)
├── Mappers/
│   ├── ClusterMapper.cs         (NEW)
│   └── ...
├── Requests/
│   └── ClassifyCustomerRequest.cs (NEW, DTO)
├── Responses/
│   ├── ClassifyCustomerResponse.cs (NEW, DTO)
│   └── ClusterResponse.cs        (NEW, DTO)
└── Watcher.Domain.csproj
```

---

## Implementation Steps

### Step 1: Create Domain Entities

Create `src/Watcher.Domain/Entities/Cluster.cs`:

```csharp
namespace Watcher.Domain.Entities;

public record Cluster(
    string IdCluster,
    string Name,
    decimal BaseLimit,
    decimal Cap
);

public static class ClusterDefinitions
{
    public static readonly Cluster CLUSTER_A = new("CLUSTER_A", "Diamond", 50000, 100000);
    public static readonly Cluster CLUSTER_B = new("CLUSTER_B", "Gold", 20000, 40000);
    public static readonly Cluster CLUSTER_C = new("CLUSTER_C", "Silver", 5000, 10000);
    public static readonly Cluster CLUSTER_D = new("CLUSTER_D", "Bronze", 0, 0);
}
```

Create `src/Watcher.Domain/Entities/ClassificationResult.cs`:

```csharp
namespace Watcher.Domain.Entities;

public record ClassificationResult(
    string CustomerId,
    Cluster Cluster,
    decimal CreditLimit,
    DateTime CalculatedAt
);
```

### Step 2: Update Classification Service

Enhance `src/Watcher.Domain/Services/ClassificationService.cs`:

```csharp
using Watcher.Domain.Entities;

namespace Watcher.Domain.Services;

public interface IClassificationService
{
    Task<ClassificationResult> ClassifyAsync(Customer customer);
}

public class ClassificationService : IClassificationService
{
    public async Task<ClassificationResult> ClassifyAsync(Customer customer)
    {
        // Validate input
        ValidateCustomer(customer);

        // Determine cluster based on priority rules
        var cluster = EvaluateCluster(customer);

        // Calculate credit limit
        var creditLimit = CalculateCreditLimit(customer.Score, cluster);

        // Return result
        return new ClassificationResult(
            customer.Id,
            cluster,
            creditLimit,
            DateTime.UtcNow
        );
    }

    private void ValidateCustomer(Customer customer)
    {
        if (customer.Score < 300 || customer.Score > 1000)
            throw new ArgumentException("Score must be between 300 and 1000");
        if (customer.Age < 18 || customer.Age > 100)
            throw new ArgumentException("Age must be between 18 and 100");
    }

    private Cluster EvaluateCluster(Customer customer)
    {
        // Priority 1: CLUSTER_A
        if (customer.Score >= 700 &&
            customer.Age >= 25 && customer.Age <= 60 &&
            !customer.HasMarketDebt)
        {
            return ClusterDefinitions.CLUSTER_A;
        }

        // Priority 2: CLUSTER_B
        if (customer.Score >= 500 &&
            customer.Age >= 18 && customer.Age <= 65 &&
            !HasBadDebt(customer.MarketDebtTypes))
        {
            return ClusterDefinitions.CLUSTER_B;
        }

        // Priority 3: CLUSTER_C
        if (customer.Score >= 300)
        {
            return ClusterDefinitions.CLUSTER_C;
        }

        // Priority 4: CLUSTER_D (default)
        return ClusterDefinitions.CLUSTER_D;
    }

    private bool HasBadDebt(string[] debtTypes)
    {
        return debtTypes.Contains("credit_default") || debtTypes.Contains("loan_default");
    }

    private decimal CalculateCreditLimit(int score, Cluster cluster)
    {
        // For CLUSTER_D, always return 0
        if (cluster.IdCluster == "CLUSTER_D")
            return 0;

        // Calculate position within score range (300-1000)
        var normalized = (decimal)(score - 300) / 700;

        // Scale to cluster range
        var creditLimit = cluster.BaseLimit + (normalized * (cluster.Cap - cluster.BaseLimit));

        // Ensure within bounds
        return Math.Min(Math.Max(creditLimit, cluster.BaseLimit), cluster.Cap);
    }
}
```

### Step 3: Create Request/Response DTOs

Create `src/Watcher.Domain/Requests/ClassifyCustomerRequest.cs`:

```csharp
namespace Watcher.Domain.Requests;

public class ClassifyCustomerRequest
{
    public string CustomerId { get; set; }
    public int Score { get; set; }
    public int Age { get; set; }
    public bool HasMarketDebt { get; set; }
    public string[] MarketDebtTypes { get; set; }
}
```

Create `src/Watcher.Domain/Responses/ClusterResponse.cs`:

```csharp
namespace Watcher.Domain.Responses;

public class ClusterResponse
{
    public string IdCluster { get; set; }
    public string Name { get; set; }
    public decimal BaseLimit { get; set; }
    public decimal Cap { get; set; }
}
```

Create `src/Watcher.Domain/Responses/ClassifyCustomerResponse.cs`:

```csharp
namespace Watcher.Domain.Responses;

public class ClassifyCustomerResponse
{
    public string CustomerId { get; set; }
    public ClusterResponse Cluster { get; set; }
    public decimal CreditLimit { get; set; }
    public DateTime CalculatedAt { get; set; }
}
```

### Step 4: Create AutoMapper Profile

Create `src/Watcher.Domain/Mappers/ClusterMapper.cs`:

```csharp
using AutoMapper;
using Watcher.Domain.Entities;
using Watcher.Domain.Responses;

namespace Watcher.Domain.Mappers;

public class ClusterMappingProfile : Profile
{
    public ClusterMappingProfile()
    {
        // Map Cluster entity to ClusterResponse DTO
        CreateMap<Cluster, ClusterResponse>()
            .ForMember(dest => dest.IdCluster, opt => opt.MapFrom(src => src.IdCluster))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.BaseLimit, opt => opt.MapFrom(src => src.BaseLimit))
            .ForMember(dest => dest.Cap, opt => opt.MapFrom(src => src.Cap));

        // Map ClassificationResult to ClassifyCustomerResponse DTO
        CreateMap<ClassificationResult, ClassifyCustomerResponse>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.Cluster, opt => opt.MapFrom(src => src.Cluster))
            .ForMember(dest => dest.CreditLimit, opt => opt.MapFrom(src => src.CreditLimit))
            .ForMember(dest => dest.CalculatedAt, opt => opt.MapFrom(src => src.CalculatedAt));
    }
}
```

### Step 5: Update MediatR Handler

Create/update `src/Watcher.Domain/Handlers/ClassifyCustomerHandler.cs`:

```csharp
using MediatR;
using AutoMapper;
using Watcher.Domain.Commands;
using Watcher.Domain.Services;
using Watcher.Domain.Responses;
using Watcher.Domain.Entities;

namespace Watcher.Domain.Handlers;

public class ClassifyCustomerHandler : IRequestHandler<ClassifyCustomerCommand, ClassifyCustomerResponse>
{
    private readonly IClassificationService _classificationService;
    private readonly IMapper _mapper;

    public ClassifyCustomerHandler(IClassificationService classificationService, IMapper mapper)
    {
        _classificationService = classificationService;
        _mapper = mapper;
    }

    public async Task<ClassifyCustomerResponse> Handle(ClassifyCustomerCommand request, CancellationToken cancellationToken)
    {
        // Create domain entity from command
        var customer = new Customer
        {
            Id = request.CustomerId,
            Score = request.Score,
            Age = request.Age,
            HasMarketDebt = request.HasMarketDebt,
            MarketDebtTypes = request.MarketDebtTypes
        };

        // Classify customer and get result
        var classificationResult = await _classificationService.ClassifyAsync(customer);

        // Map to response DTO
        var response = _mapper.Map<ClassifyCustomerResponse>(classificationResult);

        return response;
    }
}
```

### Step 6: Register in Dependency Injection

Update `src/Watcher.Api/Program.cs`:

```csharp
// Add domain services
builder.Services.AddScoped<IClassificationService, ClassificationService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(ClusterMappingProfile));

// Add MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<ClassifyCustomerHandler>()
);
```

### Step 7: Update API Controller

Ensure `src/Watcher.Api/Controllers/CustomerController.cs` uses the updated response:

```csharp
[HttpPost("classify")]
public async Task<ActionResult<ClassifyCustomerResponse>> Classify(
    [FromBody] ClassifyCustomerRequest request)
{
    var command = new ClassifyCustomerCommand
    {
        CustomerId = request.CustomerId,
        Score = request.Score,
        Age = request.Age,
        HasMarketDebt = request.HasMarketDebt,
        MarketDebtTypes = request.MarketDebtTypes
    };

    var response = await _mediator.Send(command);
    return Ok(response);
}
```

---

## Testing

### Unit Test Example

Add to `tests/Watcher.Domain.UnitTests/ClassificationServiceTests.cs`:

```csharp
[Fact]
public async Task ClassifyAsync_ReturnsClusterA_WithCorrectCreditLimit()
{
    // Arrange
    var service = new ClassificationService();
    var customer = new Customer
    {
        Id = "CUST-001",
        Score = 750,
        Age = 45,
        HasMarketDebt = false,
        MarketDebtTypes = []
    };

    // Act
    var result = await service.ClassifyAsync(customer);

    // Assert
    Assert.Equal("CLUSTER_A", result.Cluster.IdCluster);
    Assert.Equal("Diamond", result.Cluster.Name);
    Assert.True(result.CreditLimit >= 50000);
    Assert.True(result.CreditLimit <= 100000);
}
```

### Integration Test Example

Add to `tests/Watcher.Api.IntegrationTests/CustomerControllerTests.cs`:

```csharp
[Fact]
public async Task Classify_WithValidRequest_ReturnsClusterWithLimits()
{
    // Arrange
    var request = new ClassifyCustomerRequest
    {
        CustomerId = "CUST-001",
        Score = 750,
        Age = 45,
        HasMarketDebt = false,
        MarketDebtTypes = []
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/customers/classify", request);

    // Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    var body = await response.Content.ReadAsAsync<ClassifyCustomerResponse>();
    body.Cluster.IdCluster.Should().Be("CLUSTER_A");
    body.CreditLimit.Should().BeGreaterThanOrEqualTo(50000);
}
```

---

## API Usage Examples

### Example 1: Classify Diamond Customer

**Request**:
```bash
curl -X POST http://localhost:5000/api/customers/classify \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "CUST-001",
    "score": 800,
    "age": 40,
    "hasMarketDebt": false,
    "marketDebtTypes": []
  }'
```

**Response**:
```json
{
  "customerId": "CUST-001",
  "cluster": {
    "idCluster": "CLUSTER_A",
    "name": "Diamond",
    "baseLimit": 50000,
    "cap": 100000
  },
  "creditLimit": 85714.29,
  "calculatedAt": "2026-04-25T10:35:00Z"
}
```

### Example 2: Classify Bronze Customer

**Request**:
```bash
curl -X POST http://localhost:5000/api/customers/classify \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "CUST-004",
    "score": 250,
    "age": 25,
    "hasMarketDebt": true,
    "marketDebtTypes": ["credit_default"]
  }'
```

**Response**:
```json
{
  "customerId": "CUST-004",
  "cluster": {
    "idCluster": "CLUSTER_D",
    "name": "Bronze",
    "baseLimit": 0,
    "cap": 0
  },
  "creditLimit": 0,
  "calculatedAt": "2026-04-25T10:35:30Z"
}
```

---

## Debugging

### Common Issues

**Issue**: `ClassificationService` not found
- **Solution**: Ensure `IClassificationService` is registered in DI (`Program.cs`)

**Issue**: Credit limit outside cluster range
- **Solution**: Verify `CalculateCreditLimit` method bounds checking

**Issue**: Wrong cluster assigned
- **Solution**: Check priority order in `EvaluateCluster` - CLUSTER_A must be checked first

### Logging

Enable debug logging to trace classification:

```csharp
// In handler
_logger.LogInformation("Classifying customer {CustomerId} with score {Score}", 
    customer.Id, customer.Score);
```

---

## Performance Checklist

- [ ] Verify response time < 100ms (use stopwatch in handler)
- [ ] Ensure no unnecessary database calls
- [ ] Check that AutoMapper is cached (mapped profiles registered once)
- [ ] Confirm MediatR handlers are registered as scoped services
- [ ] Load test with 1000+ requests/second simulated

---

## Next Steps

1. **Implement** all code changes following the steps above
2. **Test** using provided unit and integration test examples
3. **Run** the full test suite to ensure no regressions
4. **Deploy** to staging and conduct smoke testing
5. **Monitor** API metrics (response time, error rate) after production release

---

## Additional Resources

- API Contract: `contracts/api-contract.md`
- Data Model: `data-model.md`
- Research & Decisions: `research.md`
- Implementation Plan: `plan.md`
