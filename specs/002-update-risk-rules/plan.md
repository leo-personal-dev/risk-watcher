# Implementation Plan: Update Risk Rules

**Branch**: `002-update-risk-rules` | **Date**: 2026-04-25 | **Spec**: [specs/002-update-risk-rules/spec.md](specs/002-update-risk-rules/spec.md)
**Input**: Feature specification from `/specs/002-update-risk-rules/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Update the customer classification business rules to use priority-based rule execution and return complete structured cluster information (ID, NAME, BASE_LIMIT, CAP) in the API response. Reorganize Watcher.Domain project into dedicated folders for commands/requests/responses, handlers, services, interfaces, mappers, and entities to improve maintainability and separation of concerns.

## Technical Context

**Language/Version**: C# with .NET 8 (LTS)  
**Primary Dependencies**: ASP.NET Core Web API, MediatR 11.0.0, AutoMapper 12.0.1, xUnit 2.4.2  
**Storage**: In-memory ConcurrentDictionary (no database required for this feature)  
**Testing**: xUnit with Moq for mocking  
**Target Platform**: Cross-platform (Windows, Linux, macOS)
**Project Type**: Web service (REST API)  
**Performance Goals**: API classification requests complete in under 100ms for 95% of requests  
**Constraints**: Maintains 99.9% uptime for classification endpoint  
**Scale/Scope**: Existing solution with 3 project structure (Api, Domain, Infrastructure); extend to handle new cluster information in responses

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### API-First Design ✅
- Feature returns structured cluster information via REST API
- Clear data contracts defined in spec (Cluster entity with ID, NAME, BASE_LIMIT, CAP)
- Will be documented using Swagger/OpenAPI

### RESTful Architecture ✅
- Classification endpoint remains POST /customers/classify
- Appropriate HTTP status codes and JSON payloads
- Resource-based design following REST principles

### Dependency Injection ✅
- Uses ASP.NET Core DI container
- MediatR handlers registered for command handling
- Existing pattern: IMediator, IMapper in controllers

### Asynchronous Programming ✅
- Classification service uses async/await
- All I/O operations non-blocking
- API endpoints marked as async Task<>

### Testing ✅
- Unit tests using xUnit framework
- Integration tests for API contracts
- High code coverage requirement maintained

**Status**: ✅ **PASSES** - All constitutional principles satisfied. No violations to justify.

## Project Structure

### Documentation (this feature)

```text
specs/002-update-risk-rules/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── checklists/          # Quality validation artifacts
```

### Source Code (repository root)

Existing 3-project clean architecture solution: The feature extends the existing RiskWatcher API and domain projects with updated classification business rules and domain project reorganization.

```text
src/
├── Watcher.Api/                     # REST API layer
│   ├── Controllers/
│   │   └── CustomerController.cs
│   ├── Program.cs                   # DI configuration
│   └── Watcher.Api.csproj
├── Watcher.Domain/                  # Business logic layer (REORGANIZED)
│   ├── Commands/                    # CQRS commands (existing)
│   │   └── ClassifyCustomerCommand.cs
│   ├── Queries/                     # CQRS queries (existing)
│   ├── Handlers/                    # NEW: Command/Query handlers
│   │   └── ClassifyCustomerHandler.cs
│   ├── Services/                    # Domain services
│   │   └── ClassificationService.cs
│   ├── Interfaces/                  # Service and repository contracts
│   │   └── IClassificationService.cs
│   ├── Entities/                    # Domain entities
│   │   ├── Customer.cs
│   │   ├── Cluster.cs               # NEW: Cluster entity
│   │   └── ClassificationResult.cs  # NEW: Result entity
│   ├── Mappers/                     # NEW: Domain mapper layer
│   │   └── ClusterMapper.cs
│   ├── Request/                     # NEW: Request DTOs
│   │   └── ClassifyCustomerRequest.cs
│   ├── Response/                    # NEW: Response DTOs
│   │   ├── ClassifyCustomerResponse.cs
│   │   └── ClusterResponse.cs
│   └── Watcher.Domain.csproj
└── Watcher.Infrastructure/          # External services
    ├── Mocks/
    └── Watcher.Infrastructure.csproj

tests/
├── Watcher.Domain.UnitTests/
│   ├── ClassificationServiceTests.cs
│   └── Watcher.Domain.UnitTests.csproj
└── Watcher.Api.IntegrationTests/
    ├── CustomerControllerTests.cs
    └── Watcher.Api.IntegrationTests.csproj
```

**Structure Decision**: Extended existing 3-project clean architecture with new domain entity `Cluster` and reorganized Watcher.Domain project to include dedicated folders for handlers, requests, responses, mappers, and entities as specified. This separation improves maintainability and follows MediatR/CQRS patterns.

## Complexity Tracking

No constitutional violations - all design patterns align with established DotNet Core API Constitution. No justification required.
