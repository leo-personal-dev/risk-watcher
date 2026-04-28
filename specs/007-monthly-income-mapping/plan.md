# Implementation Plan: Monthly income mapping for customer classification

**Branch**: `007-monthly-income-mapping` | **Date**: April 27, 2026 | **Spec**: [spec.md](./spec.md)  
**Input**: Feature specification from `/specs/007-monthly-income-mapping/spec.md`

This plan extends the existing Watcher classification pipeline to support monthly income mappings keyed by `clusterId` and `jobCategoryId`. The feature uses a repository-backed mock configuration and enriches the `/customers/classify` response with the original customer payload plus classification metadata and the resolved monthly income.

## Summary

Add a new `MonthlyIncomeMapping` domain concept and an in-memory repository to store cluster/category income values. Update the classification flow so that after a customer is assigned a cluster and job category, the system looks up the matching monthly income and includes it in the classification response.

The endpoint request remains a customer payload with `id`, `name`, `age`, `score`, `has_market_debt`, `market_debt_types`, `location.city`, `location.state`, `location.region`, and `job_title`. The response returns the same customer data plus enriched classification output.

## Technical Context

**Language/Version**: .NET 8, C# 12  
**Primary Dependencies**: ASP.NET Core Web API, MediatR, AutoMapper, FluentValidation, xUnit  
**Storage**: In-memory mock repository only; monthly income parametrization is stored in a mock repository within the repo  
**Testing**: xUnit unit and integration tests  
**Target Platform**: ASP.NET Core Web API  
**Project Type**: Web service API with clean architecture layering  
**Performance Goals**: Maintain existing low-latency classification behavior with minimal added lookup overhead  
**Constraints**: No database connectivity allowed; runtime-only mock configuration  
**Scale/Scope**: Support configuration of monthly income values for cluster/category combinations within the single service runtime

## Constitution Check

- API-first design: The classification endpoint remains the core integration point.  
- RESTful architecture: The request and response shape remain stable; classification enrichment is additive.  
- Dependency injection: New mapping repository and service layers will be registered in DI.  
- Asynchronous programming: Domain/service methods remain async.  
- Testing: xUnit unit and integration coverage will be required.  
- No database requirement: Explicitly maintained through a mock repository implementation.

No constitution violations are introduced.

## Project Structure

### Documentation (this feature)

```text
specs/007-monthly-income-mapping/
├── plan.md
├── spec.md
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

**Structure Decision**: Continue using the existing layered repository structure. Add a new domain entity for monthly income mapping and a mock repository in `Watcher.Infrastructure.Mocks`. Enrich classification response models in `Watcher.Domain.Commands.Response` and preserve the customer request payload shape in `Watcher.Api`.

## Complexity Tracking

No constitution violations detected; no additional complexity justification required.
