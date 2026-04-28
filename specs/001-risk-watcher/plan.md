# Implementation Plan: Risk Watcher

**Branch**: `001-risk-watcher` | **Date**: 2026-04-23 | **Spec**: [specs/001-risk-watcher/spec.md](specs/001-risk-watcher/spec.md)
**Input**: Feature specification from `/specs/001-risk-watcher/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Implement a .NET Core API for customer risk classification with a single POST /customers/classify endpoint. The API uses clean architecture with three projects: watcher-api, watcher-domain, and watcher-infrastructure. Classification is based on business rules with mocked data storage.

## Technical Context

**Language/Version**: .NET 8 (latest LTS version)  
**Primary Dependencies**: ASP.NET Core Web API, MediatR for CQRS, AutoMapper for mapping, xUnit for testing  
**Storage**: In-memory mocks (no database integration)  
**Testing**: xUnit with Moq for mocking  
**Target Platform**: Cross-platform web service (Linux/Windows/macOS)  
**Project Type**: REST API web service  
**Performance Goals**: API response time < 500ms  
**Constraints**: No database integration, use mocks; single endpoint only; clean architecture with three projects  
**Scale/Scope**: Single endpoint handling customer classification requests

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Constitution principles check:
- API-First Design: PASS - API is the primary channel
- RESTful Architecture: PASS - Uses RESTful POST endpoint
- Dependency Injection: PASS - Built-in DI container used
- Asynchronous Programming: PASS - Async operations implemented
- Testing: PASS - xUnit testing included

Technology stack check:
- .NET 6+: PASS - Using .NET 8
- ASP.NET Core Web API: PASS
- Entity Framework Core: NOT APPLICABLE - No database integration
- SQL Server: NOT APPLICABLE - Using mocks
- Swagger/OpenAPI: PASS - For API documentation

Development workflow check:
- Git: PASS
- Feature branches: PASS
- Pull requests: PASS
- CI/CD: ASSUMED - Can be added later

Governance: PASS - Plan complies with constitution standards.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
RiskWatcher.sln
src/
├── Watcher.Api/
│   ├── Controllers/
│   ├── Program.cs
│   ├── appsettings.json
│   └── Watcher.Api.csproj
├── Watcher.Domain/
│   ├── Entities/
│   ├── Services/
│   ├── Interfaces/
│   └── Watcher.Domain.csproj
└── Watcher.Infrastructure/
    ├── Mocks/
    ├── Services/
    └── Watcher.Infrastructure.csproj

tests/
├── Watcher.Api.UnitTests/
├── Watcher.Domain.UnitTests/
└── Watcher.Api.IntegrationTests/
```

**Structure Decision**: Clean Architecture with three projects as specified: Watcher.Api (presentation layer), Watcher.Domain (business logic), Watcher.Infrastructure (data access mocks). Solution file at root for .NET organization.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
