# MediatR Mediator Pattern Adoption ADR

## Context

The Watcher.Api application was initially built with a direct service injection pattern where controllers directly called domain services. This created tight coupling between the API layer and business logic, making the code harder to test, maintain, and extend.

The application needed to implement clean architecture principles with proper separation of concerns, where the API layer acts only as an HTTP interface without knowledge of domain entities or business logic.

## Decision

We decided to adopt the MediatR mediator pattern to implement Command Query Responsibility Segregation (CQRS) within the application. This involves:

- Creating command and response DTOs in the domain layer
- Implementing request handlers that orchestrate business logic
- Using MediatR to decouple controllers from handlers
- Maintaining clean architecture boundaries

## Implementation

### Phase 1: Domain Structure Setup
- Created Commands/Request and Commands/Response directories
- Defined command classes for classification and job category operations
- Set up handler directory structure

### Phase 2: Handler Implementation
- Implemented IRequestHandler classes for all commands
- Handlers inject domain services and map results to responses
- Maintained existing business logic while providing clean interfaces

### Phase 3: API Restructuring
- Created API-specific DTOs for requests and responses
- Updated controllers to use IMediator instead of direct service injection
- Implemented AutoMapper profiles for API ↔ Domain mapping

### Phase 4: Configuration & Dependency Injection
- Registered MediatR in Program.cs
- Added FluentValidation for command validation
- Configured AutoMapper with new profiles

### Phase 5: Testing
- Created comprehensive unit tests for all handlers
- Updated integration tests for new API contracts
- Verified end-to-end functionality

### Phase 6: Cleanup & Documentation
- Removed obsolete mapping profiles
- Verified no domain entity leakage in API layer
- Created this ADR and updated documentation

## Consequences

### Benefits
- **Clean Architecture**: API layer is ignorant of domain entities and business logic
- **Testability**: Handlers can be unit tested independently of HTTP concerns
- **Maintainability**: Changes to business logic don't affect API contracts
- **Extensibility**: New features can be added by creating new commands/handlers
- **Separation of Concerns**: Clear boundaries between HTTP, commands, and business logic

### Trade-offs
- **Additional Abstractions**: More classes and interfaces to maintain
- **Learning Curve**: Team needs to understand MediatR patterns
- **Small Performance Overhead**: Extra indirection through mediator
- **Increased Complexity**: More moving parts in the architecture

### Migration Path
The refactoring was done in phases to minimize risk:
1. Domain structure created without breaking existing code
2. Handlers implemented alongside existing services
3. Controllers updated incrementally
4. Old code removed only after verification

## Alternatives Considered

### Option 1: Direct Service Calls (Status Quo)
- Pros: Simple, less code
- Cons: Tight coupling, hard to test, violates clean architecture

### Option 2: Repository Pattern Only
- Pros: Less abstraction than MediatR
- Cons: Still couples API to domain entities

### Option 3: Full CQRS with Separate Write/Read Models
- Pros: Maximum scalability and flexibility
- Cons: Overkill for current application size and complexity

MediatR was chosen as the sweet spot between simplicity and clean architecture benefits.

## Compliance

This decision aligns with:
- Clean Architecture principles
- SOLID design principles
- CQRS patterns
- Domain-Driven Design concepts

## References

- MediatR Documentation: https://github.com/jbogard/MediatR
- Clean Architecture (Robert C. Martin)
- CQRS Pattern (Martin Fowler)