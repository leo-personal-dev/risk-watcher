# Research & Technical Decisions

**Feature**: Parametrize Job Category  
**Date**: 2026-04-25  
**Status**: Complete - No unknowns identified

## Decision Log

### Decision: In-Memory Repository Pattern
**Context**: Feature requires no database integration, using mocked table for user parametrization.  
**Options Considered**: 
- EF Core with in-memory provider (rejected: violates no DB requirement)
- Simple Dictionary/ConcurrentDictionary (chosen: simple, thread-safe)
- List with LINQ (chosen: easy CRUD operations)  
**Rationale**: Simple in-memory list provides CRUD without DB dependencies.  
**Alternatives Considered**: Custom mock framework - overkill for this scope.

### Decision: Keyword Matching Implementation
**Context**: Identify job category by matching jobTitle against keywords (case-insensitive substring).  
**Options Considered**:
- Regex matching (rejected: overkill, performance)
- LINQ Contains with ToLower (chosen: simple, performant)
- Full-text search (rejected: not needed for substring)  
**Rationale**: LINQ Contains provides efficient substring matching with case insensitivity.  
**Alternatives Considered**: Custom string matching - unnecessary complexity.

### Decision: Multiplier Usage
**Context**: Multiplier field defined but not specified how used.  
**Options Considered**:
- Risk score multiplier (assumed: common in financial systems)
- Credit limit multiplier (possible: based on cluster precedent)
- Unused for now (rejected: field exists for a reason)  
**Rationale**: Assume multiplier applies to risk calculation, stored for future use.  
**Alternatives Considered**: Clarify with user - but scope limits to implementation.

### Decision: Refactor ClassifyCustomerHandler
**Context**: Remove ClassifyCustomerCommand, use ClassifyCustomerRequest directly.  
**Options Considered**:
- Keep MediatR command pattern (rejected: user requirement)
- Direct handler with request (chosen: simpler architecture)
- Custom mediator (rejected: overkill)  
**Rationale**: Direct use of request class simplifies domain without losing functionality.  
**Alternatives Considered**: Partial refactor - but full removal required.

## Resolved Unknowns

- None identified in technical context.

## Best Practices Applied

- **Repository Pattern**: In-memory implementation follows same interface as DB version.
- **Async Operations**: All repository methods async for consistency.
- **Validation**: Input validation at service layer.
- **Error Handling**: KeyNotFoundException for missing entities, InvalidOperationException for conflicts.