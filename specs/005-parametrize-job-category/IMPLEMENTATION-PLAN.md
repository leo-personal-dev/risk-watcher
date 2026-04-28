# Implementation Plan: Mediator Pattern Refactoring for Watcher Application

**Date**: 2026-04-26 | **Target Branch**: `005-parametrize-job-category`

## Executive Summary

This document outlines a comprehensive implementation plan for refactoring the Watcher application from a direct service-based architecture to a clean mediator pattern architecture using MediatR. The refactoring maintains all existing functionality while establishing clear separation of concerns and preventing the API layer from coupling to domain entities and business rules.

### Key Objectives

- ✅ Establish clear architectural boundaries between layers
- ✅ Implement CQRS-inspired mediator pattern using MediatR
- ✅ Move all domain logic to Watcher.Domain
- ✅ Decouple API layer from business rules and entities
- ✅ Maintain all existing functionality without breaking changes
- ✅ Preserve in-memory repository pattern
- ✅ Ensure comprehensive test coverage throughout refactoring

---

## Section 1: Architecture Overview

### 1.1 Current State (To Be Refactored)

```
┌─────────────────────────────────────────────┐
│         Watcher.Api Layer                   │
├─────────────────────────────────────────────┤
│ • Controllers (direct service coupling)     │
│ • Models (Request/Response DTO)             │
│ • Validators                                │
│ • Mappings (AutoMapper profiles)            │
└────────────┬────────────────────────────────┘
             │ (tightly coupled)
             ▼
┌─────────────────────────────────────────────┐
│       Watcher.Domain Layer                  │
├─────────────────────────────────────────────┤
│ • Entities (Customer, JobCategory, etc.)    │
│ • Services (business logic)                 │
│ • Interfaces (dependencies)                 │
│ • Requests (ClassifyCustomerRequest)        │
└────────────┬────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────┐
│    Watcher.Infrastructure Layer             │
├─────────────────────────────────────────────┤
│ • Mock Repositories (in-memory storage)     │
└─────────────────────────────────────────────┘
```

**Issues with Current Architecture:**
- Controllers directly reference domain entities (tight coupling)
- Controllers directly call services (no abstraction)
- No clear request/response contracts separate from entities
- Business logic mixed with infrastructure concerns
- API layer knowledge of domain business rules
- Difficult to test layers in isolation

### 1.2 Target State (Mediator Pattern)

```
┌──────────────────────────────────────────────┐
│         Watcher.Api Layer                    │
├──────────────────────────────────────────────┤
│ • Controllers (only HTTP concerns)           │
│ • Validators (only contract validation)      │
│ • Models (API Request/Response DTO only)     │
│ • Mappings (DTO ↔ Query/Command mapping)     │
└────────────┬─────────────────────────────────┘
             │ (decoupled via mediator)
             ▼ MediatR.IMediator
┌──────────────────────────────────────────────┐
│      Watcher.Domain Layer                    │
├──────────────────────────────────────────────┤
│ ┌─── Entities ──────────────────────────┐   │
│ │ • Customer, JobCategory, etc.         │   │
│ └───────────────────────────────────────┘   │
│ ┌─── Commands/Queries ──────────────────┐   │
│ │ Command/Request/*.cs                  │   │
│ │ Command/Response/*.cs                 │   │
│ └───────────────────────────────────────┘   │
│ ┌─── Handlers ──────────────────────────┐   │
│ │ Handlers/*.cs (business logic)        │   │
│ └───────────────────────────────────────┘   │
│ ┌─── Services ──────────────────────────┐   │
│ │ • Repository interfaces               │   │
│ │ • Business rule evaluators            │   │
│ └───────────────────────────────────────┘   │
└────────────┬─────────────────────────────────┘
             │
             ▼
┌──────────────────────────────────────────────┐
│   Watcher.Infrastructure Layer               │
├──────────────────────────────────────────────┤
│ • Mock Repositories (in-memory storage)      │
└──────────────────────────────────────────────┘
```

**Advantages of Target Architecture:**
- Complete API layer decoupling from domain logic
- Clear separation of concerns (SoC)
- CQRS pattern enables independent scaling of reads/writes
- Handlers encapsulate all business logic
- Easy to add cross-cutting concerns (logging, validation, caching)
- Testable handlers independent of HTTP concerns
- Command/Request/Response contracts are explicit
- MediatR enables flexible pipeline behavior

### 1.3 Separation of Concerns

| Responsibility | Layer | Implementation |
|---|---|---|
| HTTP Routing & Endpoints | **Watcher.Api** | Controllers |
| Input Validation (Contracts) | **Watcher.Api** | Validators, FluentValidation |
| DTO Mapping | **Watcher.Api** | AutoMapper profiles |
| Command/Query Dispatch | **Watcher.Api** | MediatR.IMediator injection |
| Command/Query Definitions | **Watcher.Domain** | Command/Request & Command/Response folders |
| Business Logic | **Watcher.Domain** | MediatR Handlers |
| Domain Entities | **Watcher.Domain** | Entities folder |
| Data Access Abstraction | **Watcher.Domain** | Interfaces (IRepository, etc.) |
| Data Persistence | **Watcher.Infrastructure** | Repository Implementations |

---

## Section 2: Data Model

### 2.1 Domain Entities (Watcher.Domain/Entities/)

All entities already exist and remain unchanged:

#### Entity: Customer
```csharp
// Watcher.Domain/Entities/Customer.cs
public class Customer
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public int Score { get; set; }
    public bool HasMarketDebt { get; set; }
    public List<string> MarketDebtTypes { get; set; } = new();
    public Location Location { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string Cluster { get; set; } = null!;
}
```

#### Entity: JobCategory
```csharp
// Watcher.Domain/Entities/JobCategory.cs
public class JobCategory
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Multiplier { get; set; }
    public List<string> Keywords { get; set; } = new();
}
```

#### Entity: ClusterConfiguration
```csharp
// Watcher.Domain/Entities/ClusterConfiguration.cs
public class ClusterConfiguration
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int ScoreMin { get; set; }
    public int ScoreMax { get; set; }
    public int AgeMin { get; set; }
    public int AgeMax { get; set; }
    public string DebtRule { get; set; } = null!;
    public decimal BaseLimit { get; set; }
    public decimal Cap { get; set; }
}
```

#### Entity: ClassificationResult
```csharp
// Watcher.Domain/Entities/ClassificationResult.cs
public class ClassificationResult
{
    public string CustomerId { get; set; } = null!;
    public Cluster Cluster { get; set; } = null!;
    public JobCategory? JobCategory { get; set; }
    public decimal CreditLimit { get; set; }
    public DateTime CalculatedAt { get; set; }
}
```

#### Supporting Entities
- **Location**: Address information
- **Cluster**: Classification result cluster
- **ScriptContext**: Runtime script evaluation context

### 2.2 Entity Relationships

```
┌──────────────────┐
│    Customer      │
├──────────────────┤
│ - Id             │ ◄──── Primary identifier
│ - Name           │
│ - Score          │       ┌─────────────────────┐
│ - Age            │──────►│  Location           │
│ - JobTitle       │       │ - Country           │
│ - JobTitle ──────┼──────►│ - State/Region      │
│ MarketDebtTypes  │       │ - City              │
└──────────────────┘       └─────────────────────┘
          │
          │ Classification triggers
          ▼
┌──────────────────────────────┐      ┌────────────────┐
│  ClusterConfiguration         │      │  JobCategory   │
├──────────────────────────────┤      ├────────────────┤
│ - Id (unique identifier)     │      │ - Id           │
│ - Name                       │      │ - Name         │
│ - ScoreMin/Max               │      │ - Multiplier   │
│ - AgeMin/Max                 │      │ - Keywords[]   │
│ - DebtRule (script)          │      └────────────────┘
│ - BaseLimit, Cap             │
└──────────────────────────────┘
          ▲
          │ Matching
          │
        Rules ────────────────┐
                              ▼
                  ┌─────────────────────┐
                  │ ClassificationResult│
                  ├─────────────────────┤
                  │ - CustomerId        │
                  │ - Cluster ◄─────────┤
                  │ - JobCategory ◄────┤
                  │ - CreditLimit       │
                  │ - CalculatedAt      │
                  └─────────────────────┘
```

---

## Section 3: Command/Request/Response Contracts

### 3.1 Folder Structure (Watcher.Domain)

```
Watcher.Domain/
├── Command/
│   ├── Requests/
│   │   ├── ClassifyCustomerCommand.cs
│   │   ├── CreateJobCategoryCommand.cs
│   │   ├── UpdateJobCategoryCommand.cs
│   │   ├── DeleteJobCategoryCommand.cs
│   │   ├── GetJobCategoryByIdQuery.cs
│   │   ├── GetAllJobCategoriesQuery.cs
│   │   ├── CreateClusterConfigurationCommand.cs
│   │   ├── UpdateClusterConfigurationCommand.cs
│   │   ├── DeleteClusterConfigurationCommand.cs
│   │   ├── GetClusterConfigurationByIdQuery.cs
│   │   └── GetAllClusterConfigurationsQuery.cs
│   └── Responses/
│       ├── ClassifyCustomerResponse.cs
│       ├── JobCategoryResponse.cs
│       ├── ClusterConfigurationResponse.cs
│       └── GenericResponse.cs
├── Entities/
├── Handlers/
├── Interfaces/
├── Services/
└── ...
```

### 3.2 Classification Command

```csharp
// Watcher.Domain/Command/Requests/ClassifyCustomerCommand.cs
using MediatR;
using Watcher.Domain.Command.Responses;

namespace Watcher.Domain.Command.Requests;

/// <summary>
/// Command to classify a customer and determine their credit limit and cluster.
/// This command encapsulates all business logic for customer classification.
/// </summary>
public class ClassifyCustomerCommand : IRequest<ClassifyCustomerResponse>
{
    public string CustomerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Score { get; set; }
    public int Age { get; set; }
    public bool HasMarketDebt { get; set; }
    public List<string> MarketDebtTypes { get; set; } = new();
    public LocationDto Location { get; set; } = null!;
    public string JobTitle { get; set; } = null!;

    public ClassifyCustomerCommand() { }

    public ClassifyCustomerCommand(
        string customerId,
        string name,
        int score,
        int age,
        bool hasMarketDebt,
        List<string> marketDebtTypes,
        LocationDto location,
        string jobTitle)
    {
        CustomerId = customerId;
        Name = name;
        Score = score;
        Age = age;
        HasMarketDebt = hasMarketDebt;
        MarketDebtTypes = marketDebtTypes;
        Location = location;
        JobTitle = jobTitle;
    }
}

public class LocationDto
{
    public string Country { get; set; } = null!;
    public string State { get; set; } = null!;
    public string City { get; set; } = null!;
}
```

### 3.3 Classification Response

```csharp
// Watcher.Domain/Command/Responses/ClassifyCustomerResponse.cs
namespace Watcher.Domain.Command.Responses;

/// <summary>
/// Response containing customer classification results.
/// Includes cluster assignment, job category, and calculated credit limit.
/// </summary>
public class ClassifyCustomerResponse
{
    public string CustomerId { get; set; } = null!;
    public ClusterDto Cluster { get; set; } = null!;
    public JobCategoryDto? JobCategory { get; set; }
    public decimal CreditLimit { get; set; }
    public DateTime CalculatedAt { get; set; }

    public ClassifyCustomerResponse() { }

    public ClassifyCustomerResponse(
        string customerId,
        ClusterDto cluster,
        JobCategoryDto? jobCategory,
        decimal creditLimit,
        DateTime calculatedAt)
    {
        CustomerId = customerId;
        Cluster = cluster;
        JobCategory = jobCategory;
        CreditLimit = creditLimit;
        CalculatedAt = calculatedAt;
    }
}

public class ClusterDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal BaseLimit { get; set; }
    public decimal Cap { get; set; }
}

public class JobCategoryDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Multiplier { get; set; }
}
```

### 3.4 Job Category Commands

```csharp
// Watcher.Domain/Command/Requests/CreateJobCategoryCommand.cs
using MediatR;
using Watcher.Domain.Command.Responses;

namespace Watcher.Domain.Command.Requests;

public class CreateJobCategoryCommand : IRequest<JobCategoryResponse>
{
    public string Name { get; set; } = null!;
    public decimal Multiplier { get; set; }
    public List<string> Keywords { get; set; } = new();
}

// Watcher.Domain/Command/Requests/UpdateJobCategoryCommand.cs
public class UpdateJobCategoryCommand : IRequest<JobCategoryResponse>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Multiplier { get; set; }
    public List<string> Keywords { get; set; } = new();
}

// Watcher.Domain/Command/Requests/DeleteJobCategoryCommand.cs
public class DeleteJobCategoryCommand : IRequest<Unit>
{
    public string Id { get; set; } = null!;
}

// Watcher.Domain/Command/Requests/GetJobCategoryByIdQuery.cs
public class GetJobCategoryByIdQuery : IRequest<JobCategoryResponse?>
{
    public string Id { get; set; } = null!;
}

// Watcher.Domain/Command/Requests/GetAllJobCategoriesQuery.cs
public class GetAllJobCategoriesQuery : IRequest<IEnumerable<JobCategoryResponse>>
{
}
```

### 3.5 Job Category Response

```csharp
// Watcher.Domain/Command/Responses/JobCategoryResponse.cs
namespace Watcher.Domain.Command.Responses;

public class JobCategoryResponse
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Multiplier { get; set; }
    public List<string> Keywords { get; set; } = new();
}
```

### 3.6 Cluster Configuration Commands

```csharp
// Watcher.Domain/Command/Requests/CreateClusterConfigurationCommand.cs
using MediatR;
using Watcher.Domain.Command.Responses;

namespace Watcher.Domain.Command.Requests;

public class CreateClusterConfigurationCommand : IRequest<ClusterConfigurationResponse>
{
    public string Name { get; set; } = null!;
    public int ScoreMin { get; set; }
    public int ScoreMax { get; set; }
    public int AgeMin { get; set; }
    public int AgeMax { get; set; }
    public string DebtRule { get; set; } = null!;
    public decimal BaseLimit { get; set; }
    public decimal Cap { get; set; }
}

// Watcher.Domain/Command/Requests/UpdateClusterConfigurationCommand.cs
public class UpdateClusterConfigurationCommand : IRequest<ClusterConfigurationResponse>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int ScoreMin { get; set; }
    public int ScoreMax { get; set; }
    public int AgeMin { get; set; }
    public int AgeMax { get; set; }
    public string DebtRule { get; set; } = null!;
    public decimal BaseLimit { get; set; }
    public decimal Cap { get; set; }
}

// Watcher.Domain/Command/Requests/DeleteClusterConfigurationCommand.cs
public class DeleteClusterConfigurationCommand : IRequest<Unit>
{
    public string Id { get; set; } = null!;
}

// Watcher.Domain/Command/Requests/GetClusterConfigurationByIdQuery.cs
public class GetClusterConfigurationByIdQuery : IRequest<ClusterConfigurationResponse?>
{
    public string Id { get; set; } = null!;
}

// Watcher.Domain/Command/Requests/GetAllClusterConfigurationsQuery.cs
public class GetAllClusterConfigurationsQuery : IRequest<IEnumerable<ClusterConfigurationResponse>>
{
}
```

### 3.7 Cluster Configuration Response

```csharp
// Watcher.Domain/Command/Responses/ClusterConfigurationResponse.cs
namespace Watcher.Domain.Command.Responses;

public class ClusterConfigurationResponse
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int ScoreMin { get; set; }
    public int ScoreMax { get; set; }
    public int AgeMin { get; set; }
    public int AgeMax { get; set; }
    public string DebtRule { get; set; } = null!;
    public decimal BaseLimit { get; set; }
    public decimal Cap { get; set; }
}
```

---

## Section 4: Handler Implementations

Handlers contain all business logic and are defined in `Watcher.Domain/Handlers/`.

### 4.1 Folder Structure

```
Watcher.Domain/Handlers/
├── Classification/
│   └── ClassifyCustomerCommandHandler.cs
├── JobCategory/
│   ├── CreateJobCategoryCommandHandler.cs
│   ├── UpdateJobCategoryCommandHandler.cs
│   ├── DeleteJobCategoryCommandHandler.cs
│   ├── GetJobCategoryByIdQueryHandler.cs
│   └── GetAllJobCategoriesQueryHandler.cs
└── ClusterConfiguration/
    ├── CreateClusterConfigurationCommandHandler.cs
    ├── UpdateClusterConfigurationCommandHandler.cs
    ├── DeleteClusterConfigurationCommandHandler.cs
    ├── GetClusterConfigurationByIdQueryHandler.cs
    └── GetAllClusterConfigurationsQueryHandler.cs
```

### 4.2 Classification Handler

```csharp
// Watcher.Domain/Handlers/Classification/ClassifyCustomerCommandHandler.cs
using MediatR;
using Watcher.Domain.Command.Requests;
using Watcher.Domain.Command.Responses;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;
using Watcher.Domain.Services;

namespace Watcher.Domain.Handlers.Classification;

/// <summary>
/// Handles customer classification command.
/// Encapsulates all business logic for determining cluster, job category, and credit limit.
/// </summary>
public class ClassifyCustomerCommandHandler : IRequestHandler<ClassifyCustomerCommand, ClassifyCustomerResponse>
{
    private readonly IClusterConfigurationService _clusterConfigurationService;
    private readonly IJobCategoryService _jobCategoryService;

    public ClassifyCustomerCommandHandler(
        IClusterConfigurationService clusterConfigurationService,
        IJobCategoryService jobCategoryService)
    {
        _clusterConfigurationService = clusterConfigurationService;
        _jobCategoryService = jobCategoryService;
    }

    public async Task<ClassifyCustomerResponse> Handle(
        ClassifyCustomerCommand request,
        CancellationToken cancellationToken)
    {
        // Validate input
        ValidateRequest(request);

        // Build domain entity from command
        var customer = new Customer
        {
            Id = request.CustomerId,
            Name = request.Name,
            Score = request.Score,
            Age = request.Age,
            HasMarketDebt = request.HasMarketDebt,
            MarketDebtTypes = request.MarketDebtTypes,
            Location = new Location
            {
                Country = request.Location.Country,
                State = request.Location.State,
                City = request.Location.City
            },
            JobTitle = request.JobTitle
        };

        // Execute business logic
        var cluster = await EvaluateClusterAsync(customer, cancellationToken);
        var jobCategory = await _jobCategoryService.IdentifyCategoryAsync(customer.JobTitle);
        var creditLimit = CalculateCreditLimit(customer.Score, cluster);

        // Build response
        return new ClassifyCustomerResponse(
            request.CustomerId,
            new ClusterDto
            {
                Id = cluster.Id,
                Name = cluster.Name,
                BaseLimit = cluster.BaseLimit,
                Cap = cluster.Cap
            },
            jobCategory != null ? new JobCategoryDto
            {
                Id = jobCategory.Id,
                Name = jobCategory.Name,
                Multiplier = jobCategory.Multiplier
            } : null,
            creditLimit,
            DateTime.UtcNow
        );
    }

    private static void ValidateRequest(ClassifyCustomerCommand request)
    {
        if (request.Score < 0 || request.Score > 1000)
            throw new ArgumentOutOfRangeException(nameof(request.Score), "Score must be between 0 and 1000.");

        if (request.Age < 18 || request.Age > 150)
            throw new ArgumentOutOfRangeException(nameof(request.Age), "Age must be between 18 and 150.");

        if (string.IsNullOrWhiteSpace(request.CustomerId))
            throw new ArgumentNullException(nameof(request.CustomerId));

        if (string.IsNullOrWhiteSpace(request.JobTitle))
            throw new ArgumentNullException(nameof(request.JobTitle));
    }

    private async Task<Cluster> EvaluateClusterAsync(Customer customer, CancellationToken cancellationToken)
    {
        var configurations = await _clusterConfigurationService.GetAllAsync();

        foreach (var configuration in configurations)
        {
            if (await MatchesConfiguration(customer, configuration))
            {
                return new Cluster(configuration.Id, configuration.Name, configuration.BaseLimit, configuration.Cap);
            }
        }

        // Default cluster if no rules match
        return ClusterDefinitions.CLUSTER_D;
    }

    private static async Task<bool> MatchesConfiguration(Customer customer, ClusterConfiguration configuration)
    {
        if (customer.Score < configuration.ScoreMin || customer.Score > configuration.ScoreMax)
            return false;

        if (customer.Age < configuration.AgeMin || customer.Age > configuration.AgeMax)
            return false;

        return await ClusterRuleEvaluator.Evaluate(configuration.DebtRule, customer);
    }

    private static decimal CalculateCreditLimit(int score, Cluster cluster)
    {
        var normalizedScore = score / 1000m;
        var limit = cluster.BaseLimit * normalizedScore;
        return Math.Min(limit, cluster.Cap);
    }
}
```

### 4.3 Job Category Handlers (Example Patterns)

```csharp
// Watcher.Domain/Handlers/JobCategory/CreateJobCategoryCommandHandler.cs
using MediatR;
using Watcher.Domain.Command.Requests;
using Watcher.Domain.Command.Responses;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.Handlers.JobCategory;

public class CreateJobCategoryCommandHandler : IRequestHandler<CreateJobCategoryCommand, JobCategoryResponse>
{
    private readonly IJobCategoryService _jobCategoryService;

    public CreateJobCategoryCommandHandler(IJobCategoryService jobCategoryService)
    {
        _jobCategoryService = jobCategoryService;
    }

    public async Task<JobCategoryResponse> Handle(
        CreateJobCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var jobCategory = new JobCategory
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Multiplier = request.Multiplier,
            Keywords = request.Keywords
        };

        var created = await _jobCategoryService.CreateAsync(jobCategory);

        return new JobCategoryResponse
        {
            Id = created.Id,
            Name = created.Name,
            Multiplier = created.Multiplier,
            Keywords = created.Keywords
        };
    }
}

// Watcher.Domain/Handlers/JobCategory/GetAllJobCategoriesQueryHandler.cs
public class GetAllJobCategoriesQueryHandler : IRequestHandler<GetAllJobCategoriesQuery, IEnumerable<JobCategoryResponse>>
{
    private readonly IJobCategoryService _jobCategoryService;

    public GetAllJobCategoriesQueryHandler(IJobCategoryService jobCategoryService)
    {
        _jobCategoryService = jobCategoryService;
    }

    public async Task<IEnumerable<JobCategoryResponse>> Handle(
        GetAllJobCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _jobCategoryService.GetAllAsync();

        return categories.Select(c => new JobCategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Multiplier = c.Multiplier,
            Keywords = c.Keywords
        });
    }
}
```

### 4.4 Cluster Configuration Handlers (Example Patterns)

```csharp
// Watcher.Domain/Handlers/ClusterConfiguration/CreateClusterConfigurationCommandHandler.cs
using MediatR;
using Watcher.Domain.Command.Requests;
using Watcher.Domain.Command.Responses;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Domain.Handlers.ClusterConfiguration;

public class CreateClusterConfigurationCommandHandler : IRequestHandler<CreateClusterConfigurationCommand, ClusterConfigurationResponse>
{
    private readonly IClusterConfigurationService _clusterConfigurationService;

    public CreateClusterConfigurationCommandHandler(IClusterConfigurationService clusterConfigurationService)
    {
        _clusterConfigurationService = clusterConfigurationService;
    }

    public async Task<ClusterConfigurationResponse> Handle(
        CreateClusterConfigurationCommand request,
        CancellationToken cancellationToken)
    {
        var clusterConfig = new ClusterConfiguration
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            ScoreMin = request.ScoreMin,
            ScoreMax = request.ScoreMax,
            AgeMin = request.AgeMin,
            AgeMax = request.AgeMax,
            DebtRule = request.DebtRule,
            BaseLimit = request.BaseLimit,
            Cap = request.Cap
        };

        var created = await _clusterConfigurationService.CreateAsync(clusterConfig);

        return new ClusterConfigurationResponse
        {
            Id = created.Id,
            Name = created.Name,
            ScoreMin = created.ScoreMin,
            ScoreMax = created.ScoreMax,
            AgeMin = created.AgeMin,
            AgeMax = created.AgeMax,
            DebtRule = created.DebtRule,
            BaseLimit = created.BaseLimit,
            Cap = created.Cap
        };
    }
}

// Watcher.Domain/Handlers/ClusterConfiguration/GetAllClusterConfigurationsQueryHandler.cs
public class GetAllClusterConfigurationsQueryHandler : IRequestHandler<GetAllClusterConfigurationsQuery, IEnumerable<ClusterConfigurationResponse>>
{
    private readonly IClusterConfigurationService _clusterConfigurationService;

    public GetAllClusterConfigurationsQueryHandler(IClusterConfigurationService clusterConfigurationService)
    {
        _clusterConfigurationService = clusterConfigurationService;
    }

    public async Task<IEnumerable<ClusterConfigurationResponse>> Handle(
        GetAllClusterConfigurationsQuery request,
        CancellationToken cancellationToken)
    {
        var configurations = await _clusterConfigurationService.GetAllAsync();

        return configurations.Select(c => new ClusterConfigurationResponse
        {
            Id = c.Id,
            Name = c.Name,
            ScoreMin = c.ScoreMin,
            ScoreMax = c.ScoreMax,
            AgeMin = c.AgeMin,
            AgeMax = c.AgeMax,
            DebtRule = c.DebtRule,
            BaseLimit = c.BaseLimit,
            Cap = c.Cap
        });
    }
}
```

---

## Section 5: API Controller Mappings

### 5.1 Design Principles for Controllers

- Controllers only handle HTTP concerns (routing, status codes, headers)
- Controllers map API DTOs to MediatR Commands/Queries
- Controllers dispatch via IMediator
- Controllers do NOT reference domain entities directly
- Controllers do NOT contain business logic
- Responses are always mapped from mediator response objects

### 5.2 Refactored Customer Controller

```csharp
// Watcher.Api/Controllers/CustomerController.cs
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Watcher.Api.Models;
using Watcher.Domain.Command.Requests;
using Watcher.Domain.Command.Responses;

namespace Watcher.Api.Controllers;

[ApiController]
[Route("customers")]
public class CustomerController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly Watcher.Api.Services.RequestMetrics _metrics;

    public CustomerController(
        IMediator mediator,
        IMapper mapper,
        Watcher.Api.Services.RequestMetrics metrics)
    {
        _mediator = mediator;
        _mapper = mapper;
        _metrics = metrics;
    }

    /// <summary>
    /// Classify a customer and determine their credit limit and cluster.
    /// </summary>
    /// <param name="request">Customer classification request with personal and financial information</param>
    /// <returns>Classification result including cluster, job category, and credit limit</returns>
    [HttpPost("classify")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerResponse>> Classify([FromBody] CustomerRequest request)
    {
        // Map API DTO to MediatR command
        var command = _mapper.Map<ClassifyCustomerCommand>(request);

        // Dispatch command through mediator
        var response = await _mediator.Send(command);

        // Map mediator response to API DTO
        var apiResponse = _mapper.Map<CustomerResponse>(response);

        _metrics.ClassificationRequests++;

        return Ok(apiResponse);
    }
}
```

### 5.3 Refactored Job Category Controller

```csharp
// Watcher.Api/Controllers/JobCategoryController.cs
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Watcher.Api.Models;
using Watcher.Domain.Command.Requests;
using Watcher.Domain.Command.Responses;

namespace Watcher.Api.Controllers;

[ApiController]
[Route("api/job-categories")]
public class JobCategoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public JobCategoryController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JobCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<JobCategoryResponse>>> GetAll()
    {
        var query = new GetAllJobCategoriesQuery();
        var response = await _mediator.Send(query);
        return Ok(_mapper.Map<IEnumerable<JobCategoryResponse>>(response));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(JobCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobCategoryResponse>> GetById(string id)
    {
        var query = new GetJobCategoryByIdQuery { Id = id };
        var response = await _mediator.Send(query);

        if (response == null)
            return NotFound();

        return Ok(_mapper.Map<JobCategoryResponse>(response));
    }

    [HttpPost]
    [ProducesResponseType(typeof(JobCategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobCategoryResponse>> Create([FromBody] JobCategoryRequest request)
    {
        var command = _mapper.Map<CreateJobCategoryCommand>(request);
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(JobCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobCategoryResponse>> Update(string id, [FromBody] JobCategoryRequest request)
    {
        var command = _mapper.Map<UpdateJobCategoryCommand>(request);
        command.Id = id;

        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var command = new DeleteJobCategoryCommand { Id = id };

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
```

### 5.4 Refactored Cluster Configuration Controller

```csharp
// Watcher.Api/Controllers/ClusterController.cs
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Watcher.Api.Models;
using Watcher.Domain.Command.Requests;
using Watcher.Domain.Command.Responses;

namespace Watcher.Api.Controllers;

[ApiController]
[Route("clusters")]
public class ClusterController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ClusterController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClusterConfigurationResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClusterConfigurationResponse>>> GetAll()
    {
        var query = new GetAllClusterConfigurationsQuery();
        var response = await _mediator.Send(query);
        return Ok(_mapper.Map<IEnumerable<ClusterConfigurationResponse>>(response));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClusterConfigurationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClusterConfigurationResponse>> GetById(string id)
    {
        var query = new GetClusterConfigurationByIdQuery { Id = id };
        var response = await _mediator.Send(query);

        if (response == null)
            return NotFound(new { error = "Cluster not found" });

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClusterConfigurationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClusterConfigurationResponse>> Create([FromBody] ClusterConfigurationRequest request)
    {
        var command = _mapper.Map<CreateClusterConfigurationCommand>(request);
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ClusterConfigurationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClusterConfigurationResponse>> Update(string id, [FromBody] ClusterConfigurationRequest request)
    {
        if (!string.Equals(id, request.Id, StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { error = "Cluster ID in the path must match the request body." });

        var command = _mapper.Map<UpdateClusterConfigurationCommand>(request);

        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Cluster not found" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var command = new DeleteClusterConfigurationCommand { Id = id };

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Cluster not found" });
        }
    }
}
```

---

## Section 6: Dependency Injection Configuration

### 6.1 Updated Program.cs

```csharp
// Watcher.Api/Program.cs
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Watcher.Api.Mappings;
using Watcher.Api.Validators;
using Watcher.Domain.Interfaces;
using Watcher.Domain.Services;
using Watcher.Infrastructure.Mocks;

var builder = WebApplication.CreateBuilder(args);

// Core services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper - scan for profiles
builder.Services.AddAutoMapper(typeof(CustomerMappingProfile).Assembly);

// MediatR - scan for handlers and requests
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Watcher.Domain.Entities.Customer).Assembly));

// Domain Services
builder.Services.AddScoped<IClassificationService, ClassificationService>();
builder.Services.AddScoped<IJobCategoryService, JobCategoryService>();
builder.Services.AddScoped<IClusterConfigurationService, ClusterConfigurationService>();

// Repository implementations (in-memory)
builder.Services.AddSingleton<IClusterConfigurationRepository, ClusterConfigurationRepository>();
builder.Services.AddSingleton<IJobCategoryRepository, JobCategoryRepository>();
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();

// Metrics service
builder.Services.AddSingleton<Watcher.Api.Services.RequestMetrics>();

// Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CustomerRequestValidator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

// Request tracking middleware
app.Use(async (context, next) =>
{
    var metrics = context.RequestServices.GetRequiredService<Watcher.Api.Services.RequestMetrics>();
    metrics.TotalRequests++;
    await next();
});

app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    .WithName("HealthCheck")
    .WithOpenApi();

// Metrics endpoint
app.MapGet("/metrics", (Watcher.Api.Services.RequestMetrics metrics) => Results.Ok(metrics))
    .WithName("Metrics")
    .WithOpenApi();

app.Run();

public partial class Program { }
```

### 6.2 MediatR Registration Details

The MediatR registration in Program.cs performs several key functions:

```csharp
// Register MediatR handlers from API project
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Register MediatR handlers from Domain project
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssemblyContaining(
        typeof(Watcher.Domain.Entities.Customer).Assembly));
```

This configuration:
- ✅ Auto-discovers all `IRequestHandler<>` implementations
- ✅ Auto-discovers all `INotificationHandler<>` implementations
- ✅ Auto-registers MediatR pipeline behaviors
- ✅ Registers validators as pipeline behaviors
- ✅ Scans both API and Domain assemblies for handlers

---

## Section 7: AutoMapper Configuration Updates

### 7.1 Mapping Strategy

The mappings follow a clear pattern:
- **API Request DTOs** → **MediatR Commands/Queries**
- **MediatR Responses** → **API Response DTOs**
- **Domain Entities** are kept internal to handlers (not exposed through mappings)

### 7.2 Updated Mapping Profiles

```csharp
// Watcher.Api/Mappings/CustomerMappingProfile.cs
using AutoMapper;
using Watcher.Api.Models;
using Watcher.Domain.Command.Requests;
using Watcher.Domain.Command.Responses;

namespace Watcher.Api.Mappings;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        // API Request → MediatR Command
        CreateMap<CustomerRequest, ClassifyCustomerCommand>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => new LocationDto
            {
                Country = src.Location.Country,
                State = src.Location.State,
                City = src.Location.City
            }));

        // MediatR Response → API Response
        CreateMap<ClassifyCustomerResponse, CustomerResponse>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.Cluster, opt => opt.MapFrom(src => src.Cluster))
            .ForMember(dest => dest.JobCategory, opt => opt.MapFrom(src => src.JobCategory))
            .ForMember(dest => dest.CreditLimit, opt => opt.MapFrom(src => src.CreditLimit))
            .ForMember(dest => dest.CalculatedAt, opt => opt.MapFrom(src => src.CalculatedAt));

        // DTOs
        CreateMap<LocationDto, LocationDto>();
    }
}

// Watcher.Api/Mappings/JobCategoryMappingProfile.cs
public class JobCategoryMappingProfile : Profile
{
    public JobCategoryMappingProfile()
    {
        // API Request → MediatR Command
        CreateMap<JobCategoryRequest, CreateJobCategoryCommand>();

        // API Request → MediatR Update Command
        CreateMap<JobCategoryRequest, UpdateJobCategoryCommand>();

        // MediatR Response → API Response
        CreateMap<JobCategoryResponse, JobCategoryResponse>();
    }
}

// Watcher.Api/Mappings/ClusterConfigurationMappingProfile.cs
public class ClusterConfigurationMappingProfile : Profile
{
    public ClusterConfigurationMappingProfile()
    {
        // API Request → MediatR Command
        CreateMap<ClusterConfigurationRequest, CreateClusterConfigurationCommand>();

        // API Request → MediatR Update Command
        CreateMap<ClusterConfigurationRequest, UpdateClusterConfigurationCommand>();

        // MediatR Response → API Response
        CreateMap<ClusterConfigurationResponse, ClusterConfigurationResponse>();
    }
}
```

---

## Section 8: Testing Strategy

### 8.1 Test Pyramid Architecture

```
        △
       /|\
      / | \
     /  |  \
    ╱───┼───╲          E2E / Integration Tests
   ╱    |    ╲         (API layer - controllers)
  ╱─────┼─────╲        ~10-15% of test count
 ╱      |      ╲
╱───────┼───────╲      Handler / Query Tests
        |        ╲    (MediatR layer - business logic)
   ┌────┴────┐   ╲   ~40-50% of test count
   │          │    ╲
   │ Domain   │     ╲ Unit Tests
   │ Logic    │      ╲(Entities, services, utilities)
   │ Tests    │    ───╲ ~35-45% of test count
   │          │   ╱────╲
   └──────────┘  ╱
```

### 8.2 Unit Tests (Domain Layer)

**Test Files**: `tests/Watcher.Domain.UnitTests/`

#### Test: Entity Validation
```csharp
// tests/Watcher.Domain.UnitTests/Entities/CustomerTests.cs
[TestClass]
public class CustomerValidationTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Constructor_WithInvalidScore_ThrowsException()
    {
        // Arrange & Act
        var customer = new Customer { Score = 1001 };
    }

    [TestMethod]
    public void Constructor_WithValidData_CreatesEntity()
    {
        // Arrange
        var customerId = "CUST001";
        var score = 500;
        var age = 35;

        // Act
        var customer = new Customer
        {
            Id = customerId,
            Score = score,
            Age = age
        };

        // Assert
        Assert.AreEqual(customerId, customer.Id);
        Assert.AreEqual(score, customer.Score);
        Assert.AreEqual(age, customer.Age);
    }
}

// tests/Watcher.Domain.UnitTests/Services/ClusterRuleEvaluatorTests.cs
[TestClass]
public class ClusterRuleEvaluatorTests
{
    [TestMethod]
    public async Task Evaluate_WithSimpleRule_ReturnsCorrectResult()
    {
        // Arrange
        var rule = "HasMarketDebt";
        var customer = new Customer { HasMarketDebt = true };

        // Act
        var result = await ClusterRuleEvaluator.Evaluate(rule, customer);

        // Assert
        Assert.IsTrue(result);
    }
}
```

### 8.3 Handler Tests (MediatR Layer)

**Test Files**: `tests/Watcher.Domain.UnitTests/Handlers/`

#### Test: Classification Handler
```csharp
// tests/Watcher.Domain.UnitTests/Handlers/ClassifyCustomerCommandHandlerTests.cs
[TestClass]
public class ClassifyCustomerCommandHandlerTests
{
    private Mock<IClusterConfigurationService> _mockClusterService;
    private Mock<IJobCategoryService> _mockJobCategoryService;
    private ClassifyCustomerCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _mockClusterService = new Mock<IClusterConfigurationService>();
        _mockJobCategoryService = new Mock<IJobCategoryService>();
        _handler = new ClassifyCustomerCommandHandler(
            _mockClusterService.Object,
            _mockJobCategoryService.Object);
    }

    [TestMethod]
    public async Task Handle_WithValidCommand_ReturnsClassificationResult()
    {
        // Arrange
        var command = new ClassifyCustomerCommand
        {
            CustomerId = "CUST001",
            Name = "John Doe",
            Score = 750,
            Age = 35,
            HasMarketDebt = false,
            MarketDebtTypes = new(),
            Location = new LocationDto { Country = "US", State = "CA", City = "SF" },
            JobTitle = "Software Engineer"
        };

        // Mock dependencies
        _mockClusterService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new[] { CreateMockClusterConfig() });
        _mockJobCategoryService.Setup(x => x.IdentifyCategoryAsync(It.IsAny<string>()))
            .ReturnsAsync(new JobCategory { Id = "JC001", Name = "Tech" });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("CUST001", result.CustomerId);
        Assert.IsNotNull(result.Cluster);
        Assert.IsNotNull(result.JobCategory);
        Assert.IsTrue(result.CreditLimit > 0);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public async Task Handle_WithInvalidScore_ThrowsException()
    {
        // Arrange
        var command = new ClassifyCustomerCommand
        {
            CustomerId = "CUST001",
            Score = 1001, // Invalid: > 1000
            Age = 35,
            JobTitle = "Engineer"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);
    }

    private ClusterConfiguration CreateMockClusterConfig()
    {
        return new ClusterConfiguration
        {
            Id = "CLU_A",
            Name = "Cluster A",
            ScoreMin = 700,
            ScoreMax = 1000,
            AgeMin = 30,
            AgeMax = 50,
            DebtRule = "!HasMarketDebt",
            BaseLimit = 5000m,
            Cap = 25000m
        };
    }
}

// tests/Watcher.Domain.UnitTests/Handlers/JobCategoryHandlerTests.cs
[TestClass]
public class CreateJobCategoryCommandHandlerTests
{
    private Mock<IJobCategoryService> _mockJobCategoryService;
    private CreateJobCategoryCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _mockJobCategoryService = new Mock<IJobCategoryService>();
        _handler = new CreateJobCategoryCommandHandler(_mockJobCategoryService.Object);
    }

    [TestMethod]
    public async Task Handle_WithValidCommand_CreatesJobCategory()
    {
        // Arrange
        var command = new CreateJobCategoryCommand
        {
            Name = "Tech",
            Multiplier = 1.2m,
            Keywords = new List<string> { "engineer", "developer" }
        };

        var createdCategory = new JobCategory
        {
            Id = "JC001",
            Name = command.Name,
            Multiplier = command.Multiplier,
            Keywords = command.Keywords
        };

        _mockJobCategoryService.Setup(x => x.CreateAsync(It.IsAny<JobCategory>()))
            .ReturnsAsync(createdCategory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("JC001", result.Id);
        Assert.AreEqual("Tech", result.Name);
        Assert.AreEqual(1.2m, result.Multiplier);
        Assert.AreEqual(2, result.Keywords.Count);
    }
}
```

### 8.4 Integration Tests (API Layer)

**Test Files**: `tests/Watcher.Api.IntegrationTests/`

#### Test: Classification Endpoint
```csharp
// tests/Watcher.Api.IntegrationTests/Endpoints/ClassifyCustomerTests.cs
[TestClass]
public class ClassifyCustomerEndpointTests
{
    private HttpClient _client;
    private WebApplicationFactory<Program> _factory;

    [TestInitialize]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task Post_ClassifyCustomer_WithValidRequest_ReturnsOkWithResult()
    {
        // Arrange
        var request = new
        {
            id = "CUST001",
            name = "John Doe",
            score = 750,
            age = 35,
            hasMarketDebt = false,
            marketDebtTypes = new List<string>(),
            location = new
            {
                country = "US",
                state = "CA",
                city = "San Francisco"
            },
            jobTitle = "Software Engineer"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/customers/classify", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseBody);

        Assert.IsTrue(jsonResponse.RootElement.TryGetProperty("customerId", out _));
        Assert.IsTrue(jsonResponse.RootElement.TryGetProperty("cluster", out _));
        Assert.IsTrue(jsonResponse.RootElement.TryGetProperty("creditLimit", out _));
    }

    [TestMethod]
    public async Task Post_ClassifyCustomer_WithInvalidScore_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            id = "CUST001",
            name = "John Doe",
            score = 1001, // Invalid: > 1000
            age = 35,
            jobTitle = "Engineer"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/customers/classify", content);

        // Assert
        Assert.IsTrue(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.UnprocessableEntity);
    }
}

// tests/Watcher.Api.IntegrationTests/Endpoints/JobCategoryTests.cs
[TestClass]
public class JobCategoryEndpointTests
{
    private HttpClient _client;
    private WebApplicationFactory<Program> _factory;

    [TestInitialize]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task Get_GetAllJobCategories_ReturnsOkWithList()
    {
        // Act
        var response = await _client.GetAsync("/api/job-categories");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        var categories = JsonSerializer.Deserialize<List<JobCategoryResponse>>(responseBody);

        Assert.IsNotNull(categories);
    }

    [TestMethod]
    public async Task Post_CreateJobCategory_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new
        {
            name = "Engineering",
            multiplier = 1.3m,
            keywords = new List<string> { "engineer", "developer", "architect" }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/job-categories", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var locationHeader = response.Headers.Location;
        Assert.IsNotNull(locationHeader);
        Assert.IsTrue(locationHeader.ToString().Contains("job-categories"));
    }
}
```

### 8.5 Test Data Builders

```csharp
// tests/Common/Builders/ClassifyCustomerCommandBuilder.cs
public class ClassifyCustomerCommandBuilder
{
    private ClassifyCustomerCommand _command;

    public ClassifyCustomerCommandBuilder()
    {
        _command = new ClassifyCustomerCommand
        {
            CustomerId = "CUST_DEFAULT",
            Name = "Default Customer",
            Score = 650,
            Age = 35,
            HasMarketDebt = false,
            MarketDebtTypes = new(),
            Location = new LocationDto
            {
                Country = "US",
                State = "CA",
                City = "San Francisco"
            },
            JobTitle = "Software Engineer"
        };
    }

    public ClassifyCustomerCommandBuilder WithScore(int score)
    {
        _command.Score = score;
        return this;
    }

    public ClassifyCustomerCommandBuilder WithAge(int age)
    {
        _command.Age = age;
        return this;
    }

    public ClassifyCustomerCommandBuilder WithJobTitle(string jobTitle)
    {
        _command.JobTitle = jobTitle;
        return this;
    }

    public ClassifyCustomerCommand Build() => _command;
}

// Usage in tests:
var command = new ClassifyCustomerCommandBuilder()
    .WithScore(800)
    .WithAge(40)
    .Build();
```

### 8.6 Test Coverage Goals

| Layer | Test Type | File Pattern | Target Coverage |
|---|---|---|---|
| Domain Entities | Unit Tests | `*Tests.cs` in Entities/ | 90%+ |
| Domain Services | Unit Tests | `*ServiceTests.cs` | 85%+ |
| MediatR Handlers | Unit + Integration | `*HandlerTests.cs` | 90%+ |
| API Controllers | Integration Tests | `*EndpointTests.cs` | 80%+ |
| Validators | Unit Tests | `*ValidatorTests.cs` | 100% |
| Mappings | Integration Tests | Tested via endpoint tests | 75%+ |

---

## Section 9: Implementation Phases

### Phase 1: Foundation (Week 1)

**Deliverables:**
1. Create folder structure in Watcher.Domain
   - `Command/Requests/`
   - `Command/Responses/`
   - `Handlers/Classification/`
   - `Handlers/JobCategory/`
   - `Handlers/ClusterConfiguration/`

2. Implement all Command/Query/Response classes
3. Update Program.cs with MediatR registration
4. Create mapping profiles for commands/queries

**Validation:**
- ✅ Folder structure created
- ✅ All command classes compile
- ✅ MediatR can scan assemblies without errors
- ✅ AutoMapper profiles load correctly

### Phase 2: Handlers (Week 2)

**Deliverables:**
1. Implement ClassifyCustomerCommandHandler
2. Implement Job Category handlers (CRUD + Get All)
3. Implement Cluster Configuration handlers (CRUD + Get All)
4. Create handler test files with test cases

**Validation:**
- ✅ All handlers implement IRequestHandler<> correctly
- ✅ Unit tests for each handler pass (80%+ coverage)
- ✅ Handlers use repositories and services correctly

### Phase 3: API Layer Refactoring (Week 3)

**Deliverables:**
1. Refactor CustomerController to use mediator
2. Refactor JobCategoryController to use mediator
3. Refactor ClusterController to use mediator
4. Update AutoMapper profiles to map DTOs ↔ Commands
5. Create integration tests for controllers

**Validation:**
- ✅ Controllers only reference IMediator
- ✅ No direct service dependencies in controllers
- ✅ No domain entities exposed in responses
- ✅ Integration tests pass (80%+ coverage)

### Phase 4: Testing & Validation (Week 4)

**Deliverables:**
1. Complete handler test suite
2. Complete integration test suite
3. E2E testing via HTTP client
4. Performance benchmarking
5. Documentation review

**Validation:**
- ✅ All tests pass
- ✅ Code coverage >= 80%
- ✅ No breaking API changes
- ✅ Performance acceptable (<100ms per classification)

### Phase 5: Cleanup & Documentation (Ongoing)

**Deliverables:**
1. Remove unused code/classes
2. Update all documentation
3. Update architecture diagrams
4. Create migration guide
5. Knowledge transfer sessions

---

## Section 10: Migration Guide for Developers

### 10.1 Before (Current - Service-Based)

```csharp
// In controller (BEFORE - tight coupling)
public class CustomerController : ControllerBase
{
    private readonly IClassificationService _classificationService;

    public CustomerController(IClassificationService classificationService)
    {
        _classificationService = classificationService;
    }

    [HttpPost("classify")]
    public async Task<ActionResult<CustomerResponse>> Classify([FromBody] CustomerRequest request)
    {
        // Controller knows about domain entities
        var customer = new Customer { ... };
        
        // Calls service directly
        var result = await _classificationService.ClassifyAsync(customer);
        
        return Ok(new CustomerResponse { ... });
    }
}
```

### 10.2 After (New - Mediator Pattern)

```csharp
// In controller (AFTER - decoupled)
public class CustomerController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CustomerController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("classify")]
    public async Task<ActionResult<CustomerResponse>> Classify([FromBody] CustomerRequest request)
    {
        // Map API DTO to command
        var command = _mapper.Map<ClassifyCustomerCommand>(request);
        
        // Dispatch through mediator (no knowledge of service layer)
        var response = await _mediator.Send(command);
        
        // Map response to API DTO
        var apiResponse = _mapper.Map<CustomerResponse>(response);
        
        return Ok(apiResponse);
    }
}
```

### 10.3 Adding New Features

**To add a new command/query:**

1. **Create the request in `Watcher.Domain/Command/Requests/`**
   ```csharp
   public class MyNewCommand : IRequest<MyNewResponse>
   {
       public string Property1 { get; set; } = null!;
       // ...
   }
   ```

2. **Create the response in `Watcher.Domain/Command/Responses/`**
   ```csharp
   public class MyNewResponse
   {
       public string Result { get; set; } = null!;
       // ...
   }
   ```

3. **Create the handler in `Watcher.Domain/Handlers/YourFeature/`**
   ```csharp
   public class MyNewCommandHandler : IRequestHandler<MyNewCommand, MyNewResponse>
   {
       public async Task<MyNewResponse> Handle(MyNewCommand request, CancellationToken cancellationToken)
       {
           // Implementation
       }
   }
   ```

4. **Update AutoMapper profile in `Watcher.Api/Mappings/`**
   ```csharp
   CreateMap<MyApiRequest, MyNewCommand>();
   CreateMap<MyNewResponse, MyApiResponse>();
   ```

5. **Update controller in `Watcher.Api/Controllers/`**
   ```csharp
   [HttpPost("endpoint")]
   public async Task<ActionResult<MyApiResponse>> MyEndpoint([FromBody] MyApiRequest request)
   {
       var command = _mapper.Map<MyNewCommand>(request);
       var response = await _mediator.Send(command);
       return Ok(_mapper.Map<MyApiResponse>(response));
   }
   ```

6. **Write tests** in `tests/Watcher.Domain.UnitTests/Handlers/` and `tests/Watcher.Api.IntegrationTests/`

---

## Section 11: Risk Mitigation & Rollback Plan

### 11.1 Risks

| Risk | Impact | Mitigation |
|---|---|---|
| Breaking API changes | High | Keep API contracts identical, test integration thoroughly |
| Performance regression | Medium | Benchmark before/after, optimize handlers if needed |
| Test coverage gaps | Medium | Aim for >80% coverage, run tests in CI/CD |
| Incomplete refactoring | High | Phase-based approach, feature flags for gradual rollout |
| Team learning curve | Medium | Documentation, code reviews, knowledge sharing sessions |

### 11.2 Rollback Strategy

If issues arise during implementation:

1. **Phase 1 Rollback**: Remove new folders/files, keep original code untouched
2. **Phase 2 Rollback**: Remove handlers, handlers don't affect API layer yet
3. **Phase 3 Rollback**: Revert controller changes, restore original service injection
4. **Full Rollback**: Use git to revert all changes to last known good state

```bash
# Rollback commands
git stash                    # Stash current changes
git checkout main            # Return to main branch
git branch -D feature-branch # Delete feature branch
```

### 11.3 Validation Checkpoints

After each phase, validate:

```bash
# Compile check
dotnet build

# Run all tests
dotnet test

# Code coverage analysis
dotnet test /p:CollectCoverage=true

# Integration test against running API
dotnet test --filter "Category=Integration"

# Performance baseline
# Compare classification time before/after
```

---

## Section 12: Deployment Considerations

### 12.1 Non-Breaking Changes

✅ The refactoring is **non-breaking** because:
- API endpoints remain at same URLs
- Request/response formats are identical
- All existing functionality preserved
- No database schema changes
- No configuration changes required

### 12.2 Deployment Steps

```bash
# 1. Build
dotnet build -c Release

# 2. Run all tests
dotnet test -c Release

# 3. Generate deployment package
dotnet publish -c Release -o ./publish

# 4. Deploy (staging first)
# ... staging deployment process ...

# 5. Run smoke tests
curl http://staging-api/health
dotnet test tests/SmokeTests/ --filter "Category=Smoke"

# 6. Deploy to production
# ... production deployment process ...

# 7. Monitor metrics
# Check RequestMetrics endpoint for baseline
curl http://api/metrics
```

### 12.3 Post-Deployment Validation

```bash
# 1. Health check
GET /health

# 2. Metrics baseline
GET /metrics

# 3. Sample classification
POST /customers/classify {
  "id": "TEST001",
  "score": 750,
  ...
}

# 4. Job category CRUD
GET /api/job-categories
POST /api/job-categories { ... }

# 5. Cluster configuration CRUD
GET /clusters
POST /clusters { ... }
```

---

## Section 13: Success Criteria & Acceptance

### 13.1 Code Quality Metrics

| Metric | Target | Validation |
|---|---|---|
| Unit Test Coverage | >= 80% | `dotnet test /p:CollectCoverage=true` |
| Integration Test Coverage | >= 75% | Integration test suite execution |
| Code Duplication | < 5% | Code analysis tools |
| Cyclomatic Complexity | < 10 | SonarQube / Roslyn analyzers |
| Architecture Compliance | 100% | Manual code review |

### 13.2 Functional Acceptance

- ✅ All existing endpoints work without changes
- ✅ Classification produces identical results
- ✅ CRUD operations work correctly
- ✅ Validation works as before
- ✅ Error handling maintains same behavior
- ✅ Performance acceptable (<100ms per request)

### 13.3 Architecture Acceptance

- ✅ API layer does NOT reference domain entities
- ✅ API layer only uses IMediator interface
- ✅ All commands/queries defined in Domain
- ✅ All handlers defined in Domain
- ✅ Business logic isolated in handlers
- ✅ Repository pattern maintained
- ✅ Dependency injection properly configured

### 13.4 Documentation Acceptance

- ✅ Architecture diagrams updated
- ✅ Implementation plan documented
- ✅ Test strategy documented
- ✅ Developer guide provided
- ✅ Migration examples shown
- ✅ Deployment guide created

---

## Appendix A: Quick Reference

### Folder Structure Checklist

```
✅ Watcher.Domain/
   ├── Command/
   │   ├── Requests/
   │   │   ├── Classification/
   │   │   ├── JobCategory/
   │   │   └── ClusterConfiguration/
   │   └── Responses/
   │       ├── ClassifyCustomerResponse.cs
   │       ├── JobCategoryResponse.cs
   │       └── ClusterConfigurationResponse.cs
   ├── Handlers/
   │   ├── Classification/
   │   ├── JobCategory/
   │   └── ClusterConfiguration/
   ├── Entities/
   ├── Interfaces/
   ├── Services/
   └── Requests/ (keep existing ClassifyCustomerRequest for backwards compatibility or migrate)

✅ Watcher.Api/
   ├── Controllers/
   │   ├── CustomerController.cs (refactored)
   │   ├── JobCategoryController.cs (refactored)
   │   └── ClusterController.cs (refactored)
   ├── Mappings/
   │   ├── CustomerMappingProfile.cs
   │   ├── JobCategoryMappingProfile.cs
   │   └── ClusterConfigurationMappingProfile.cs
   ├── Models/ (API DTOs only)
   ├── Validators/
   └── Program.cs (updated with MediatR)

✅ tests/
   ├── Watcher.Domain.UnitTests/
   │   ├── Handlers/
   │   │   ├── Classification/
   │   │   ├── JobCategory/
   │   │   └── ClusterConfiguration/
   │   ├── Services/
   │   ├── Entities/
   │   └── Builders/
   └── Watcher.Api.IntegrationTests/
       ├── Endpoints/
       │   ├── ClassifyCustomerTests.cs
       │   ├── JobCategoryTests.cs
       │   └── ClusterConfigurationTests.cs
       └── Fixtures/
```

### Key Files to Modify

1. **Watcher.Api/Program.cs**
   - Add MediatR registration
   - Ensure AutoMapper scans Domain assembly

2. **Watcher.Api/Controllers/*.cs**
   - Replace service injection with IMediator
   - Change service calls to _mediator.Send(command)

3. **Watcher.Api/Mappings/**.cs
   - Add command/query mappings
   - Map responses back to API DTOs

4. **All new files** (see Section 3-4)
   - Command/Request classes
   - Response classes
   - Handler implementations
   - Test files

### Dependencies Already Available

```
✓ MediatR 11.0.0
✓ MediatR.Extensions.Microsoft.DependencyInjection 11.1.0
✓ AutoMapper 12.0.1
✓ FluentValidation.AspNetCore 11.3.1
✓ xUnit (implied by test projects)
```

---

## Appendix B: Example Files Reference

### Complete Handler Implementation

See Section 4.2-4.4 for full implementations of:
- `ClassifyCustomerCommandHandler`
- `CreateJobCategoryCommandHandler`
- `GetAllJobCategoriesQueryHandler`
- `CreateClusterConfigurationCommandHandler`
- `GetAllClusterConfigurationsQueryHandler`

### Complete Controller Implementation

See Section 5.2-5.4 for full implementations of:
- `CustomerController` (refactored)
- `JobCategoryController` (refactored)
- `ClusterController` (refactored)

### Complete Test Examples

See Section 8.3-8.4 for patterns of:
- Unit tests for handlers
- Integration tests for endpoints
- Test data builders

---

## Document Metadata

| Property | Value |
|---|---|
| Document Status | **READY FOR IMPLEMENTATION** |
| Target Branch | `005-parametrize-job-category` |
| Last Updated | 2026-04-26 |
| Estimated Implementation Time | 4 weeks (1 week per phase) |
| Team Members | Development team, QA, tech lead |
| Approval Required | Yes |
| Review Checklist | [REVIEW_CHECKLIST.md](REVIEW_CHECKLIST.md) |

---

**End of Implementation Plan Document**

