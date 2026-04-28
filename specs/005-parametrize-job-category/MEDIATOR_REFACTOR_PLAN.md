# Mediator Pattern Refactor - Planning Summary

## Overview

This document summarizes the planning phase for refactoring the Watcher application to use the MediatR mediator pattern while maintaining clean architecture principles and ensuring the API layer has zero knowledge of domain business logic.

## Key Decisions

### 1. **Folder Structure**
Following the separation of concerns principle:
- **Watcher.Domain**: Commands, Responses, Handlers, Entities, Services, Interfaces
- **Watcher.Api**: Controllers, Models (Requests/Responses), Validators, Mappings
- **Watcher.Infrastructure**: In-memory Repositories

### 2. **Command Organization**
```
Watcher.Domain/Commands/
├── Request/          (IRequest<TResponse> classes)
└── Response/         (Response DTOs)
```

This clearly separates incoming commands from their expected responses.

### 3. **Handler Location**
All handlers are in `Watcher.Domain/Handlers/` (not split across the application), making them easy to discover and test.

### 4. **API Layer Constraints**
The API layer MUST NOT:
- Import domain entities directly
- Know about business services
- Access repositories
- Implement business logic

The API layer MAY:
- Define controllers
- Define request/response models
- Call MediatR
- Validate input
- Map API models to/from commands

## Architecture Layers

```
┌─────────────────────────────────────────────────────────┐
│                    Watcher.Api                          │
│  (Controllers, Models, Validation, API Mapping)         │
└────────────────────┬────────────────────────────────────┘
                     │ IMediator
                     │
┌────────────────────▼────────────────────────────────────┐
│                 Watcher.Domain                          │
│  ┌──────────────────────────────────────────────────┐   │
│  │ Commands (Request/Response)                      │   │
│  │ Handlers (Business Logic Orchestration)          │   │
│  │ Entities (Domain Models)                         │   │
│  │ Services (Business Logic Implementation)         │   │
│  │ Interfaces (Contracts)                           │   │
│  └──────────────────────────────────────────────────┘   │
└────────────────┬──────────────────────┬─────────────────┘
                 │                      │
    ┌────────────▼──┐          ┌────────▼────────┐
    │ IRepository  │          │  Interfaces      │
    │ Interfaces   │          │  (Services)      │
    └────────────▲──┘          └──────────────────┘
                 │
┌────────────────┴──────────────────────────────────────┐
│            Watcher.Infrastructure                     │
│            (In-Memory Repositories)                   │
└────────────────────────────────────────────────────────┘
```

## Command/Handler Pairs

### Classification Flow
- **Command**: `ClassifyCustomerCommand`
- **Handler**: `ClassifyCustomerCommandHandler`
- **Response**: `ClassifyCustomerResponse`
- **Service**: `IClassificationService` (called by handler)

### Job Category Management
- **Commands**: 
  - `GetAllJobCategoriesCommand`
  - `GetJobCategoryByIdCommand`
  - `CreateJobCategoryCommand`
  - `UpdateJobCategoryCommand`
  - `DeleteJobCategoryCommand`
- **Service**: `IJobCategoryService` (called by handlers)

## Data Flow Example: Classification Request

```
1. API Request arrives
   POST /api/classify
   Body: { customerId, score, age, hasMarketDebt, jobTitle }

2. ClassificationController.Classify()
   - Receives ClassifyCustomerRequest (API model)

3. Validation
   - FluentValidation checks input contract

4. Mapping
   - ClassifyCustomerRequest → ClassifyCustomerCommand (Domain command)

5. MediatR Dispatch
   - _mediator.Send(classifyCustomerCommand)

6. Handler Execution
   - ClassifyCustomerCommandHandler.Handle()
   - Creates Customer entity from command
   - Calls IClassificationService.ClassifyAsync()
   - Returns ClassifyCustomerResponse

7. Response Mapping
   - ClassifyCustomerResponse → API ClassifyCustomerResponse (different class)

8. HTTP Response
   - 200 OK { customerId, cluster, jobCategory, creditLimit, calculatedAt }
```

## Implementation Approach

### Phase 1: Foundation (Blocking)
1. Create command request/response structure
2. Create handler skeleton classes

### Phase 2: Handler Implementation
3. Implement all handlers with business logic orchestration

### Phase 3: API Restructuring
4. Create API models separate from domain
5. Create AutoMapper profiles for mapping
6. Update controllers to use MediatR

### Phase 4: Configuration
7. Update Program.cs with MediatR setup
8. Verify all dependencies properly wired

### Phase 5: Testing & Validation
9. Update/create tests
10. Verify clean architecture principles

## Key Metrics

| Aspect | Before | After |
|--------|--------|-------|
| API Knowledge of Domain | Direct access to services & entities | Only commands/responses |
| Controller Complexity | High (orchestrates services) | Low (validates & dispatches) |
| Handler Count | 0 | 5+ |
| Command Count | 0 | 5+ |
| Architecture Pattern | Service-based | Mediator-based |
| Testability | Medium | High |
| Maintainability | Medium | High |

## Files Created During Planning

1. **ARCHITECTURE_PLAN.md** - Detailed architecture design
2. **REFACTOR_TASKS.md** - Implementation task list
3. **MEDIATOR_REFACTOR_PLAN.md** - This file

## Next Steps

1. Review architecture plan with team
2. Approve task list and priority
3. Begin Phase 1 implementation:
   - Create folder structure
   - Create command classes
   - Create response classes
4. Continue with sequential phases

## Success Criteria

✅ All 20 tasks completed  
✅ All tests passing  
✅ API layer has zero domain entity imports  
✅ All business logic in handlers  
✅ Clean dependency flow (Api → Domain → Infrastructure)  
✅ Code review approved  

## Risk Mitigation

| Risk | Mitigation |
|------|-----------|
| Breaking existing functionality | Comprehensive test coverage during refactor |
| Complex migrations | Phased approach with validation at each step |
| Team unfamiliarity with MediatR | Documentation and code comments |
| Performance impact | Profile and optimize after refactor |
