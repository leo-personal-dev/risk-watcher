# Research Findings: Risk Watcher

**Feature**: Risk Watcher | **Date**: 2026-04-23

## Research Tasks Completed

### Business Rules for Customer Risk Clustering

**Task**: Research business rules for assigning customers to risk clusters (CLUSTER_A, CLUSTER_B, CLUSTER_C, CLUSTER_D) based on financial data.

**Findings**:
- Risk clustering typically considers credit score, debt history, location risk, and employment stability
- CLUSTER_A: Low risk (high score, no debt, stable regions)
- CLUSTER_B: Medium-low risk (good score, minor debt)
- CLUSTER_C: Medium-high risk (moderate score, some debt issues)
- CLUSTER_D: High risk (low score, multiple debt types, high-risk regions)

**Decision**: Implement rule-based classification using score thresholds, debt presence, and regional risk factors.
**Rationale**: Meets business requirements for reliable and flexible rules without needing ML complexity.
**Alternatives Considered**: Machine learning model (rejected due to zero engineering effort requirement), simple score-only (insufficient for comprehensive risk assessment).

### .NET Core Clean Architecture Best Practices

**Task**: Find best practices for .NET Core API with clean architecture (three projects: API, Domain, Infrastructure).

**Findings**:
- Use MediatR for CQRS pattern to separate commands/queries
- AutoMapper for object mapping between layers
- Dependency injection for loose coupling
- Repository pattern for data access (even with mocks)
- Unit tests with xUnit and Moq

**Decision**: Use MediatR, AutoMapper, and repository pattern.
**Rationale**: Standard .NET practices for maintainable, testable code.
**Alternatives Considered**: Direct service calls (rejected for testability), manual mapping (rejected for boilerplate).

### API Performance Optimization

**Task**: Research techniques to ensure <500ms response time.

**Findings**:
- Async/await for I/O operations
- In-memory caching for rules
- Minimal object mapping
- Lightweight serialization

**Decision**: Implement async operations and in-memory rule storage.
**Rationale**: Ensures performance requirements are met.
**Alternatives Considered**: Synchronous operations (rejected for scalability), database caching (rejected due to no DB requirement).