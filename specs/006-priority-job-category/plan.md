# Implementation Plan: Priority-driven job category matching in customer classification

**Branch**: `006-priority-job-category` | **Date**: April 26, 2026 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/006-priority-job-category/spec.md`

This plan maintains the existing ASP.NET Core clean architecture, continues using the in-memory job category mock repository, and updates classification to derive a final category using explicit priority ordering.

## Summary

Extend the job category model with a new `Priority` field and update category identification to evaluate categories in priority order. The final matched category will be the last category found in the ordered keyword validation sequence. This preserves the current no-database architecture and keeps the API contract stable.

## Technical Context

**Language/Version**: .NET 8, C# 12  
**Primary Dependencies**: ASP.NET Core Web API, MediatR, AutoMapper, FluentValidation, xUnit  
**Storage**: In-memory mock repository only; no database connection  
**Testing**: xUnit unit and integration tests  
**Target Platform**: ASP.NET Core Web API  
**Project Type**: Web service API (domain-driven layers)  
**Performance Goals**: Maintain low-latency classification and deterministic priority-based category selection  
**Constraints**: No database connectivity allowed; feature relies on existing mock data  
**Scale/Scope**: Single-service API with internal mock job categories; no persistence beyond runtime  

## Constitution Check

This feature complies with the project constitution:
- API-first design: the unchanged `/customers/classify` endpoint remains the primary integration point.
- RESTful architecture: input/output contract is preserved and existing HTTP semantics remain valid.
- Dependency injection: classification and job category services remain registered in DI.
- Asynchronous programming: service calls remain async.
- Testing: xUnit coverage and integration testing are still required.

No constitution violations are introduced. The no-database requirement is explicitly maintained through the existing mock repository.

## Project Structure

### Documentation (this feature)

```text
specs/006-priority-job-category/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── api-contract.md
└── checklists/
    └── requirements.md
```

### Source Code (repository root)

```text
src/
├── Watcher.Api/
├── Watcher.Domain/
└── Watcher.Infrastructure/

tests/
├── Watcher.Domain.UnitTests/
└── Watcher.Api.IntegrationTests/
```

**Structure Decision**: The feature is implemented within the existing layered architecture, keeping priority logic in the domain service and preserving the mock repository in infrastructure.

## Complexity Tracking

No constitution violations are present, so no complexity justification is required.
