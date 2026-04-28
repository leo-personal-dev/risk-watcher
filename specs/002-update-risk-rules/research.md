# Phase 0: Research & Design Decisions

**Date**: 2026-04-25  
**Feature**: Update Risk Rules  
**Status**: Complete

## Research Summary

All technical context items clarified. No ambiguities remain. This document consolidates design decisions and best practices research for the feature.

---

## 1. Priority-Based Classification Rules Engine

### Decision
Implement classification rules as a sequential priority-based evaluation system in the `ClassificationService` domain service.

### Rationale
- **Simplicity**: Direct if-else evaluation is more maintainable than a complex rules engine for 4 discrete rules
- **Performance**: Sequential evaluation is O(n) with n=4, completing well within the <100ms target
- **Clarity**: Business rules are explicit and testable
- **MediatR Integration**: Handler chains rules to service for execution

### Alternatives Considered
- **Rules Engine (Drools.NET, Easy Rules)**: Would add external dependencies and complexity for simple sequential rules; rejected for feature scope
- **Reflection-based Dynamic Rules**: Over-engineered for 4 static business rules; rejected for maintainability concerns

### Implementation Pattern
```csharp
// Service evaluates priority order (1-4):
1. Check CLUSTER_A conditions first
2. If not matched, check CLUSTER_B
3. If not matched, check CLUSTER_C
4. Default to CLUSTER_D
```

---

## 2. Structured Cluster Information (New Entity)

### Decision
Create new `Cluster` entity (immutable record) with properties: ID_CLUSTER, NAME, BASE_LIMIT, CAP.

### Rationale
- **Type Safety**: Cluster properties are well-defined and immutable
- **API Contracts**: Structured response ensures clear OpenAPI/Swagger documentation
- **Reusability**: Cluster definition can be referenced throughout the domain and API layers
- **Testability**: Clear data contracts make unit tests more maintainable

### Alternatives Considered
- **String-only responses** (current state): Returns only cluster name; rejected because loss of BASE_LIMIT/CAP data
- **Dynamic objects**: Using `JObject` or untyped dictionaries; rejected for type safety and API contract clarity

### Implementation Pattern
```csharp
public record Cluster(
    string IdCluster,
    string Name,
    decimal BaseLimit,
    decimal Cap
);

// Pre-defined cluster instances:
public static class ClusterDefinitions
{
    public static readonly Cluster CLUSTER_A = new("CLUSTER_A", "Diamond", 50000, 100000);
    public static readonly Cluster CLUSTER_B = new("CLUSTER_B", "Gold", 20000, 40000);
    public static readonly Cluster CLUSTER_C = new("CLUSTER_C", "Silver", 5000, 10000);
    public static readonly Cluster CLUSTER_D = new("CLUSTER_D", "Bronze", 0, 0);
}
```

---

## 3. Credit Limit Calculation Strategy

### Decision
Calculate credit limits as a value within the cluster's BASE_LIMIT to CAP range using a simple percentage-based formula applied to customer score.

### Rationale
- **Fairness**: Links credit limit to customer score (higher score = higher limit within cluster range)
- **Boundaries**: Ensures compliance with cluster's BASE_LIMIT and CAP constraints
- **Performance**: O(1) calculation, no external calls needed
- **Determinism**: Same input always produces same output

### Alternatives Considered
- **Fixed limit per cluster**: Would not differentiate customers within same cluster; rejected
- **External credit bureau API**: Would introduce latency and external dependencies; not required by spec; rejected

### Implementation Pattern
```csharp
// Formula: calculate position within score range, then scale to limit range
decimal normalized = (customer.Score - 300) / 700; // normalize 300-1000 score to 0-1
decimal creditLimit = cluster.BaseLimit + (normalized * (cluster.Cap - cluster.BaseLimit));
return Math.Min(Math.Max(creditLimit, cluster.BaseLimit), cluster.Cap); // ensure bounds
```

---

## 4. Domain Project Reorganization

### Decision
Restructure `Watcher.Domain` project into dedicated folders: Handlers, Services, Interfaces, Entities, Mappers, Requests, Responses.

### Rationale
- **Separation of Concerns**: Each concern (handling, mapping, entities) has its own folder
- **MediatR/CQRS Pattern**: Aligns with modern .NET patterns for command/query handling
- **Scalability**: Easy to add new commands/queries without cluttering the project
- **Team Convention**: Follows industry best practices for domain-driven design

### Alternatives Considered
- **Flat structure**: All files in root; rejected for poor navigation as project grows
- **Feature-based folders**: Organize by feature (Classification, etc.); could be used later; not needed at this scale

### Folder Structure
```
Watcher.Domain/
├── Commands/          # CQRS commands (queries for execution)
├── Queries/           # CQRS queries (data read operations)
├── Handlers/          # Command/Query handler implementations
├── Services/          # Domain service implementations
├── Interfaces/        # Service and repository contracts
├── Entities/          # Domain model classes
├── Mappers/           # DTO mapping logic
├── Requests/          # Command request DTOs
├── Responses/         # Command response DTOs
└── Watcher.Domain.csproj
```

---

## 5. API Response Contract (OpenAPI)

### Decision
Extend POST /customers/classify response to include complete cluster information and calculated credit limit.

### Rationale
- **Client Information**: Clients need full cluster context for business decisions
- **API Versioning**: Extending response maintains backward compatibility (old clients ignore new fields)
- **Documentation**: OpenAPI automatically documents new response shape
- **Usability**: All relevant data returned in single request (no need for secondary calls)

### Response Shape
```json
{
  "customerId": "string",
  "cluster": {
    "idCluster": "CLUSTER_A",
    "name": "Diamond",
    "baseLimit": 50000,
    "cap": 100000
  },
  "creditLimit": 75000,
  "calculatedAt": "2026-04-25T10:30:00Z"
}
```

---

## 6. Testing Strategy

### Decision
- Unit tests: Test `ClassificationService` with all cluster scenarios and boundary conditions
- Integration tests: Test `/customers/classify` endpoint with valid and edge-case payloads
- Test coverage goal: Maintain 80%+ code coverage for domain logic

### Test Scenarios
1. **Happy Path**: Customer matching each cluster (A, B, C, D)
2. **Boundary Values**: Ages 25, 60, 18, 65; Scores 700, 500, 300
3. **Edge Cases**: 
   - Customer with no market debt types (empty array)
   - Customer with has_market_debt=true but no bad debts in types
   - Score exactly at boundary (e.g., 700.00)
4. **Error Cases**: Invalid input (null, missing fields, negative values)

### Rationale
- Comprehensive coverage ensures rule correctness
- Boundary testing catches off-by-one errors in range comparisons
- Integration tests validate API contract and HTTP behavior

---

## 7. Domain Service Pattern (Existing)

### Decision
Continue using `ClassificationService` domain service for business logic, called via MediatR handler.

### Rationale
- **Existing Pattern**: Aligns with already-implemented architecture
- **Separation**: Service logic is separate from HTTP concerns (controller)
- **Reusability**: Service can be called from different handlers or contexts
- **Testability**: Service is easy to unit test in isolation

### Pattern
```csharp
// Handler receives command
public class ClassifyCustomerHandler : IRequestHandler<ClassifyCustomerCommand, ClassifyCustomerResponse>
{
    public async Task<ClassifyCustomerResponse> Handle(ClassifyCustomerCommand request, CancellationToken cancellationToken)
    {
        // Delegate to domain service
        var cluster = await _classificationService.ClassifyAsync(request.Customer);
        // Map and return response
        return _mapper.Map<ClassifyCustomerResponse>(cluster);
    }
}
```

---

## 8. Backward Compatibility

### Decision
Response enhancement maintains backward compatibility. Existing clients will receive new fields (idCluster, creditLimit) in addition to existing cluster name field.

### Rationale
- **Non-Breaking**: Old clients that only read cluster name continue to work
- **Safe Rollout**: No coordination needed with dependent systems
- **Migration Path**: Clients can optionally migrate to new response format

### Migration Timeline
- Phase 1: Extend response with new fields (both old and new formats present)
- Future: Can deprecate old format with proper versioning

---

## 9. Data Model Decisions

### Decision
No database required. Use in-memory `ConcurrentDictionary<string, Customer>` for mock data (continuation from Phase 1 of first feature).

### Rationale
- **Feature Scope**: Classification is stateless; no persistence required
- **Performance**: In-memory access is < 1ms (well within 100ms target)
- **Simplicity**: No database setup, migrations, or infrastructure needed
- **Testing**: Easy to mock and control test data

### Alternative for Future
- When real data persistence needed: Introduce Entity Framework Core + SQL Server/PostgreSQL
- Repository pattern already designed for this transition

---

## Summary

All design decisions align with the DotNet Core API Constitution and existing project patterns. Implementation can proceed to Phase 1 (data modeling and contract definition) with confidence.

**Next Phase**: Data modeling of Cluster entity, API contracts, and quickstart guide.
