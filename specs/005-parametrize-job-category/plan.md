# Implementation Plan: Parametrize Job Category

**Branch**: `005-parametrize-job-category` | **Date**: 2026-04-25 | **Spec**: [specs/005-parametrize-job-category/spec.md](specs/005-parametrize-job-category/spec.md)
**Input**: Feature specification from `/specs/005-parametrize-job-category/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Implement user-parametrizable job categories with CRUD operations and automated identification based on jobTitle keyword matching. Use in-memory mocking for data persistence, no database integration. Refactor domain to use ClassifyCustomerRequest directly in handler, removing Commands folder and ClassifyCustomerCommand class.

## Technical Context

**Language/Version**: C# .NET 8.0  
**Primary Dependencies**: ASP.NET Core Web API, AutoMapper, FluentValidation, MediatR  
**Storage**: In-memory mock repository (no database integration)  
**Testing**: xUnit for unit and integration tests  
**Target Platform**: ASP.NET Core Web API service  
**Project Type**: Web API application  
**Performance Goals**: Job category identification in <100ms  
**Constraints**: No database integration, use mocked in-memory table for user parametrization  
**Scale/Scope**: Support 100 job categories with up to 50 keywords each

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- ✅ API-First Design: Feature designed with RESTful API contracts
- ✅ RESTful Architecture: Uses HTTP methods and resource-based URLs
- ✅ Dependency Injection: ASP.NET Core DI container used
- ✅ Asynchronous Programming: Async/await for all operations
- ✅ Testing: xUnit for unit and integration tests
- ❌ Storage: Constitution requires Entity Framework Core + SQL Server/SQLite, but feature uses in-memory mock (no database integration as specified)
- ✅ Technology Stack: .NET 8, ASP.NET Core Web API, xUnit (EF Core not used due to no DB requirement)

**Gate Status**: BLOCKED - Storage violation requires justification

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
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
# [REMOVE IF UNUSED] Option 1: Single project (DEFAULT)
src/
├── models/
├── services/
├── cli/
└── lib/

tests/
├── contract/
├── integration/
└── unit/

# [REMOVE IF UNUSED] Option 2: Web application (when "frontend" + "backend" detected)
backend/
├── src/
│   ├── models/
│   ├── services/
│   └── api/
└── tests/

frontend/
├── src/
│   ├── components/
│   ├── pages/
│   └── services/
└── tests/

# [REMOVE IF UNUSED] Option 3: Mobile + API (when "iOS/Android" detected)
api/
└── [same as backend above]

ios/ or android/
└── [platform-specific structure: feature modules, UI flows, platform tests]
```

**Structure Decision**: [Document the selected structure and reference the real
directories captured above]

## Complexity Tracking

**Storage Violation Justification**: Constitution requires EF Core + database, but feature specification explicitly requires no database integration and in-memory mocking. This is justified as the feature is designed for user parametrization without persistence requirements. Future features may add database support if needed.

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
