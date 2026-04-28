# Tasks: Priority-driven job category matching in customer classification

**Input**: Design documents from `/specs/006-priority-job-category/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/api-contract.md`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Confirm the feature scope and preserve the existing in-memory architecture before implementation.

- [x] T001 Confirm feature documentation exists in `specs/006-priority-job-category/`
- [x] T002 [P] Review existing classification and job category code paths in `src/Watcher.Domain/Services/ClassificationService.cs` and `src/Watcher.Domain/Services/JobCategoryService.cs`
- [x] T003 [P] Verify the mock repository `src/Watcher.Infrastructure/Mocks/JobCategoryRepository.cs` is the only job category persistence mechanism and no database connection is introduced

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Add the priority field and seed data so the ordered matching behavior has a stable domain base.

- [x] T004 [P] Add `Priority` property to `src/Watcher.Domain/Entities/JobCategory.cs`
- [x] T005 [P] Update `src/Watcher.Infrastructure/Mocks/JobCategoryRepository.cs` seed data to include `Priority` values for all existing categories
- [x] T006 [P] Update `src/Watcher.Domain/Interfaces/IJobCategoryService.cs` comments or contract docs to reflect that category identification uses priority-ordered evaluation

---

## Phase 3: User Story 1 - Assign job category by priority-ordered keywords (Priority: P1) 🎯 MVP

**Goal**: Implement the core classification behavior so the system evaluates categories in priority order and returns the final matched category.

**Independent Test**: Submit a classification request with a job title matching multiple categories and verify the last matched priority-ordered category is returned.

### Implementation for User Story 1

- [x] T007 [US1] Implement priority-ordered keyword validation in `src/Watcher.Domain/Services/JobCategoryService.cs`
- [x] T008 [US1] Ensure `IdentifyCategoryAsync` returns the last matching category after ordering categories by `Priority`
- [x] T009 [US1] Add deterministic tie-breaking for equal `Priority` values in `src/Watcher.Domain/Services/JobCategoryService.cs`

### Tests for User Story 1

- [x] T010 [P] [US1] Add or extend unit tests in `tests/Watcher.Domain.UnitTests/JobCategoryServiceTests.cs` for multiple matches and final-category selection
- [x] T011 [P] [US1] Add integration test in `tests/Watcher.Api.IntegrationTests/CustomerClassificationTests.cs` verifying a multi-match job title returns the final priority-ordered category

---

## Phase 4: User Story 2 - Manage category priority metadata (Priority: P2)

**Goal**: Make category priority explicit in the mock repository and ensure classification behavior uses the configured metadata.

**Independent Test**: Verify job categories include priority values and those values affect matching order.

- [x] T012 [US2] Add explicit `Priority` values for seeded job categories in `src/Watcher.Infrastructure/Mocks/JobCategoryRepository.cs`
- [x] T013 [US2] Add unit tests in `tests/Watcher.Domain.UnitTests/JobCategoryServiceTests.cs` that verify categories are ordered by `Priority` before matching
- [x] T014 [US2] Add a test in `tests/Watcher.Domain.UnitTests/JobCategoryServiceTests.cs` for equal-priority categories to confirm deterministic ordering

---

## Phase 5: User Story 3 - Persist final category in customer classification results (Priority: P3)

**Goal**: Ensure the final matched job category is included in the classification response when a category is found.

**Independent Test**: Run a classification request and verify the response contains `jobCategoryId` and `jobCategoryName` for matched customers.

- [x] T015 [US3] Confirm `src/Watcher.Domain/Services/ClassificationService.cs` assigns the result from `IJobCategoryService.IdentifyCategoryAsync`
- [x] T016 [US3] Confirm `src/Watcher.Api/Controllers/CustomerController.cs` returns the classification response including category fields
- [x] T017 [US3] Add integration test in `tests/Watcher.Api.IntegrationTests/CustomerClassificationTests.cs` asserting a category match is present in the response body

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finalize documentation, cleanup, and validation after implementation.

- [x] T018 [P] Update `specs/006-priority-job-category/quickstart.md` with the implemented priority matching behavior
- [x] T019 [P] Update `specs/006-priority-job-category/contracts/api-contract.md` with the category priority and final matched category behavior
- [x] T020 [P] Run `dotnet test` and confirm all tests pass
- [x] T021 [P] Review and clean up any unused comments or TODOs added during implementation
