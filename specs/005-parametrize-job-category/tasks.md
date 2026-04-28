# Tasks: Parametrize Job Category

**Input**: Design documents from `/specs/005-parametrize-job-category/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: The examples below include test tasks. Tests are OPTIONAL - only include them if explicitly requested in the feature specification.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Phase 1: Setup & Infrastructure (Foundational - MUST COMPLETE FIRST)

- [x] T001 Create `JobCategory` entity in `src/Watcher.Domain/Entities/JobCategory.cs` with properties: `Id`, `Name`, `Multiplier`, `Keywords`
- [x] T002 Create `IJobCategoryRepository` in `src/Watcher.Domain/Interfaces/IJobCategoryRepository.cs` with methods: `GetAllAsync()`, `GetByIdAsync(string id)`, `CreateAsync(JobCategory category)`, `UpdateAsync(JobCategory category)`, `DeleteAsync(string id)`
- [x] T003 Implement in-memory repository `src/Watcher.Infrastructure/Mocks/JobCategoryRepository.cs` with seeded job categories and CRUD operations
- [x] T004 Register `IJobCategoryRepository` and `IJobCategoryService` in `src/Watcher.Api/Program.cs` using DI

## Phase 2: Foundational Service & Model Layer (Blocking prerequisites)

- [x] T005 Create API DTOs in `src/Watcher.Api/Models/JobCategoryRequest.cs` and `src/Watcher.Api/Models/JobCategoryResponse.cs`
- [x] T006 Create `IJobCategoryService` in `src/Watcher.Domain/Interfaces/IJobCategoryService.cs` with methods: `CreateAsync`, `UpdateAsync`, `DeleteAsync`, `GetAllAsync`, `GetByIdAsync`
- [x] T007 Implement `JobCategoryService` in `src/Watcher.Domain/Services/JobCategoryService.cs` with validation and repository operations
- [x] T008 Add job category identification logic in `src/Watcher.Domain/Services/JobCategoryService.cs` for matching jobTitle against keywords
- [x] T009 Update `src/Watcher.Domain/Services/ClassificationService.cs` to use job categories for customer classification
- [x] T010 Create AutoMapper profile `src/Watcher.Api/Mappings/JobCategoryMappingProfile.cs` for DTO mapping

## Phase 3: User Story 1 - Manage Job Categories (Priority: P1)

- [x] T011 [US1] Add GET `/api/job-categories` endpoint in `src/Watcher.Api/Controllers/JobCategoryController.cs` to retrieve all categories
- [x] T012 [US1] Add POST `/api/job-categories` endpoint in `src/Watcher.Api/Controllers/JobCategoryController.cs` to create new categories
- [x] T013 [US1] Add PUT `/api/job-categories/{id}` endpoint in `src/Watcher.Api/Controllers/JobCategoryController.cs` to update categories
- [x] T014 [US1] Add DELETE `/api/job-categories/{id}` endpoint in `src/Watcher.Api/Controllers/JobCategoryController.cs` to delete categories
- [x] T015 [US1] Add input validation in `src/Watcher.Api/Validators/JobCategoryRequestValidator.cs` for required fields and constraints
- [x] T016 [US1] Add unit tests in `tests/Watcher.Domain.UnitTests/JobCategoryServiceTests.cs` for CRUD operations
- [x] T017 [US1] Add integration tests in `tests/Watcher.Api.IntegrationTests/JobCategoryTests.cs` for API endpoints

## Phase 4: User Story 2 - Identify Job Category (Priority: P1)

- [x] T018 [US2] Add `JobTitle` property to `src/Watcher.Domain/Requests/ClassifyCustomerRequest.cs`
- [x] T019 [US2] Implement job category identification in `ClassificationService` using keyword matching
- [x] T020 [US2] Add unit test in `tests/Watcher.Domain.UnitTests/ClassificationServiceTests.cs` for job category identification

## Phase 5: Domain Refactor (Priority: P1)

- [x] T021 [P] Update `src/Watcher.Api/Handlers/ClassifyCustomerHandler.cs` to accept `ClassifyCustomerRequest` directly instead of `ClassifyCustomerCommand`
- [x] T022 [P] Delete `src/Watcher.Domain/Commands/` folder and `ClassifyCustomerCommand.cs` file

## Dependencies

- `T001` through `T004` must complete before `T005` through `T010`
- `T005` through `T010` form the foundation for all user story phases
- `T011`-`T017` (US1) can be implemented independently of `T018`-`T020` (US2)
- `T021`-`T022` depend on classification service updates

## Parallel Execution Opportunities

- `T001`, `T002`, `T003` can be developed concurrently
- DTO/mapping work (`T005`, `T010`) parallelizable with repository implementation (`T003`)
- API endpoints (`T011`-`T014`) can run in parallel with validation (`T015`)
- Test creation tasks (`T016`, `T017`, `T020`) can run in parallel with endpoint implementation
- Refactor tasks (`T021`, `T022`) are independent

## Implementation Strategy

1. Build the domain model and in-memory repository first to satisfy the no-database requirement.
2. Implement job category CRUD services and API endpoints as independent user story.
3. Integrate job category identification into classification flow.
4. Perform domain refactor to remove command pattern.
5. Finish with documentation updates and testing.