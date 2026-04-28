# Tasks: Monthly income mapping for customer classification

**Input**: Design documents from `/specs/007-monthly-income-mapping/`
**Prerequisites**: `spec.md`, `plan.md`, `checklists/requirements.md`

## Phase 1: Setup (Feature discovery)

**Purpose**: Confirm the feature boundaries and the existing classification architecture before implementation.

- [x] T001 Confirm documentation exists in `specs/007-monthly-income-mapping/`
- [x] T002 [P] Review current classification request and response models in `src/Watcher.Domain/Commands/Request/ClassifyCustomerCommand.cs` and `src/Watcher.Domain/Commands/Response/ClassifyCustomerResponse.cs`
- [x] T003 [P] Review current classification flow in `src/Watcher.Domain/Handlers/ClassifyCustomerCommandHandler.cs` and `src/Watcher.Domain/Services/ClassificationService.cs`
- [x] T004 [P] Review dependency injection and existing mock repository patterns in `src/Watcher.Api/Program.cs` and `src/Watcher.Infrastructure/Mocks`

---

## Phase 2: Foundational (Domain model and repository)

**Purpose**: Add monthly income mapping domain objects, repository, and service scaffolding.

- [x] T005 Add `MonthlyIncomeMapping` entity in `src/Watcher.Domain/Entities/MonthlyIncomeMapping.cs`
- [x] T006 Add `IMonthlyIncomeMappingRepository` in `src/Watcher.Domain/Interfaces/IMonthlyIncomeMappingRepository.cs`
- [x] T007 Implement `MonthlyIncomeMappingRepository` in `src/Watcher.Infrastructure/Mocks/MonthlyIncomeMappingRepository.cs`
- [x] T008 Add `IMonthlyIncomeMappingService` in `src/Watcher.Domain/Interfaces/IMonthlyIncomeMappingService.cs`
- [x] T009 Implement `MonthlyIncomeMappingService` in `src/Watcher.Domain/Services/MonthlyIncomeMappingService.cs`
- [x] T010 Register `IMonthlyIncomeMappingRepository` and `IMonthlyIncomeMappingService` in `src/Watcher.Api/Program.cs`
- [ ] T011 Confirm `src/Watcher.Domain/Commands/Request/ClassifyCustomerCommand.cs` includes the full customer payload shape required by the endpoint
- [ ] T012 Add `monthlyIncome` to `src/Watcher.Domain/Commands/Response/ClassifyCustomerResponse.cs`

---

## Phase 3: User Story 1 - Manage monthly income mappings (Priority: P1)

**Goal**: Enable creating, editing, and deleting monthly income mappings keyed by cluster and job category.

**Independent Test**: Manage a monthly income mapping through the new API and verify the persisted mapping state.

- [ ] T013 [US1] Add monthly income mapping request models in `src/Watcher.Domain/Commands/Request/CreateMonthlyIncomeMappingCommand.cs` and `src/Watcher.Domain/Commands/Request/UpdateMonthlyIncomeMappingCommand.cs`
- [ ] T014 [US1] Add monthly income mapping controller in `src/Watcher.Api/Controllers/MonthlyIncomeMappingController.cs`
- [ ] T015 [US1] Implement add/update/delete mapping endpoints in `src/Watcher.Api/Controllers/MonthlyIncomeMappingController.cs`
- [ ] T016 [US1] Wire mapping CRUD endpoints to `IMonthlyIncomeMappingService` in `src/Watcher.Domain/Services/MonthlyIncomeMappingService.cs`
- [ ] T017 [P] [US1] Add unit tests for monthly income mapping CRUD operations in `tests/Watcher.Domain.UnitTests/MonthlyIncomeMappingServiceTests.cs`
- [ ] T018 [P] [US1] Add integration tests for mapping management endpoints in `tests/Watcher.Api.IntegrationTests/MonthlyIncomeMappingControllerTests.cs`

---

## Phase 4: User Story 2 - Apply income mapping after classification (Priority: P2)

**Goal**: Resolve monthly income after cluster and job category classification and include it in the enriched response.

**Independent Test**: Classify a customer and verify the response includes the correct `monthlyIncome` value when a mapping exists.

- [x] T019 [US2] Update `src/Watcher.Domain/Services/ClassificationService.cs` to resolve monthly income using `clusterId` and `jobCategoryId`
- [x] T020 [US2] Update `src/Watcher.Domain/Handlers/ClassifyCustomerCommandHandler.cs` to include `monthlyIncome` in `ClassifyCustomerResponse`
- [x] T021 [US2] Add integration test in `tests/Watcher.Api.IntegrationTests/CustomerClassificationTests.cs` for classification responses that include `monthlyIncome`
- [x] T022 [P] [US2] Add unit test in `tests/Watcher.Domain.UnitTests/ClassificationServiceTests.cs` or equivalent for monthly income lookup during classification

---

## Phase 5: User Story 3 - Ensure mapping uniqueness and validation (Priority: P3)

**Goal**: Prevent duplicate cluster/category mappings and validate monthly income values.

**Independent Test**: Attempt duplicate and invalid mapping requests and verify the system rejects them with validation errors.

- [ ] T023 [US3] Enforce unique `clusterId` + `jobCategoryId` mapping keys in `src/Watcher.Domain/Services/MonthlyIncomeMappingService.cs`
- [ ] T024 [US3] Enforce non-negative decimal validation for mapping `value` in `src/Watcher.Domain/Services/MonthlyIncomeMappingService.cs`
- [ ] T025 [US3] Add unit tests for duplicate mapping rejection and invalid value validation in `tests/Watcher.Domain.UnitTests/MonthlyIncomeMappingServiceTests.cs`
- [ ] T026 [US3] Add integration tests for duplicate/invalid mapping behavior in `tests/Watcher.Api.IntegrationTests/MonthlyIncomeMappingControllerTests.cs`

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finalize documentation, cleanup, and test validation.

- [ ] T027 [P] Update `specs/007-monthly-income-mapping/checklists/requirements.md` if implementation details require spec refinement
- [ ] T028 [P] Run `dotnet test` and confirm all tests pass
- [ ] T029 [P] Review and clean up any unused comments or TODOs added during implementation
