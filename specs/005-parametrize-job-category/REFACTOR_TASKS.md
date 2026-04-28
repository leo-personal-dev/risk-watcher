# Implementation Tasks: Mediator Pattern Refactor

**Status**: Planning Phase
**Priority**: High
**Estimated Effort**: 8-10 tasks
**Dependencies**: None (can start immediately)

## Phase 1: Domain Structure Setup (Blocking)

- [ ] T001 Create folder structure in Watcher.Domain:
  - `Commands/Request/`
  - `Commands/Response/`
  - `Handlers/`

- [ ] T002 Create `ClassifyCustomerCommand` (Request) in `Watcher.Domain/Commands/Request/ClassifyCustomerCommand.cs`
  - Properties: CustomerId, Score, Age, HasMarketDebt, MarketDebtTypes, JobTitle
  - Implement IRequest<ClassifyCustomerResponse>

- [ ] T003 Create `ClassifyCustomerResponse` in `Watcher.Domain/Commands/Response/ClassifyCustomerResponse.cs`
  - Properties: CustomerId, Cluster, JobCategory, CreditLimit, CalculatedAt

- [ ] T004 Create Job Category command classes in `Watcher.Domain/Commands/Request/`:
  - `GetAllJobCategoriesCommand` → IRequest<List<JobCategoryResponse>>
  - `GetJobCategoryByIdCommand` → IRequest<JobCategoryResponse>
  - `CreateJobCategoryCommand` → IRequest<JobCategoryResponse>
  - `UpdateJobCategoryCommand` → IRequest<JobCategoryResponse>
  - `DeleteJobCategoryCommand` → IRequest<Unit>

- [ ] T005 Create corresponding response classes in `Watcher.Domain/Commands/Response/`:
  - `GetAllJobCategoriesResponse` (list wrapper)
  - `GetJobCategoryResponse`
  - `CreateJobCategoryResponse`
  - `UpdateJobCategoryResponse`

## Phase 2: Handler Implementation

- [ ] T006 Implement `ClassifyCustomerCommandHandler` in `Watcher.Domain/Handlers/`
  - Inject IClassificationService
  - Handle() method: orchestrate classification logic
  - Map ClassificationResult to ClassifyCustomerResponse

- [ ] T007 Implement Job Category handlers in `Watcher.Domain/Handlers/`:
  - `GetAllJobCategoriesCommandHandler`
  - `GetJobCategoryByIdCommandHandler`
  - `CreateJobCategoryCommandHandler`
  - `UpdateJobCategoryCommandHandler`
  - `DeleteJobCategoryCommandHandler`

## Phase 3: Watcher.Api Restructuring

- [ ] T008 Create API DTOs in `Watcher.Api/Models/Requests/`:
  - `ClassifyCustomerRequest` (API input model)
  - `CreateJobCategoryRequest`
  - `UpdateJobCategoryRequest`

- [ ] T009 Create API DTOs in `Watcher.Api/Models/Responses/`:
  - `ClassifyCustomerResponse` (API output model)
  - `JobCategoryResponse`
  - `ClusterResponse`

- [ ] T010 Create AutoMapper profile `CommandApiMappingProfile.cs`:
  - Map API Request Models → Command Requests
  - Map Command Responses → API Response Models

- [ ] T011 Update `ClassificationController.cs`:
  - Inject IMediator (remove IClassificationService)
  - Update Classify() action:
    - Validate input
    - Map ClassifyCustomerRequest (API) → ClassifyCustomerCommand (Domain)
    - Send command via mediator
    - Map response back to API model

- [ ] T012 Update `JobCategoryController.cs`:
  - Inject IMediator (remove IJobCategoryService)
  - Update GetAll(): Send GetAllJobCategoriesCommand
  - Update GetById(id): Send GetJobCategoryByIdCommand
  - Update Create(request): Send CreateJobCategoryCommand
  - Update Update(id, request): Send UpdateJobCategoryCommand
  - Update Delete(id): Send DeleteJobCategoryCommand

## Phase 4: Configuration & Dependency Injection

- [ ] T013 Update `Program.cs`:
  - Remove direct service injections
  - Add `builder.Services.AddMediatR()`
  - Ensure FluentValidation is configured
  - Verify all repositories are registered

- [ ] T014 Create or update fluent validators for command requests:
  - `ClassifyCustomerCommandValidator`
  - `CreateJobCategoryCommandValidator`
  - `UpdateJobCategoryCommandValidator`

## Phase 5: Testing

- [ ] T015 Create unit tests for handlers in `tests/Watcher.Domain.UnitTests/`:
  - `ClassifyCustomerCommandHandlerTests`
  - `CreateJobCategoryCommandHandlerTests`
  - Other handler tests

- [ ] T016 Update/create integration tests for controllers:
  - Verify commands are properly dispatched
  - Verify responses are properly mapped
  - Test validation and error cases

- [ ] T017 Run full test suite and verify all tests pass

## Phase 6: Cleanup & Documentation

- [ ] T018 Remove unused files/classes:
  - Delete direct service references from API
  - Remove old response classes if duplicated

- [ ] T019 Update project documentation:
  - Add architecture decision record (ADR)
  - Document API endpoints with updated structure
  - Update README with new architecture

- [ ] T020 Code review and validation:
  - Verify no domain entities leak into Watcher.Api
  - Verify all business logic is in handlers
  - Verify clean layering

## Dependencies

- T001 must complete before T002-T005
- T002-T005 must complete before T006-T007
- T006-T007 must complete before T008-T009
- T008-T010 must complete before T011-T012
- All of T001-T012 must complete before T013
- T013 must complete before T015-T017

## Validation Checklist

- [ ] All commands defined in Watcher.Domain.Commands.*
- [ ] All handlers defined in Watcher.Domain.Handlers
- [ ] All domain entities in Watcher.Domain.Entities
- [ ] API layer has NO references to:
  - Domain entities (except through commands/responses)
  - Business services (only MediatR)
  - Repositories (only through services/handlers)
- [ ] API models are distinct from domain entities
- [ ] All tests pass
- [ ] MediatR properly configured
- [ ] Handlers are registered and discoverable
