# Implementation Tasks: Parametrize Cluster Definition

**Feature**: Parametrize Cluster Definition  
**Branch**: `003-parametrize-cluster-definition`  
**Date**: 2026-04-25  
**Status**: Implementation Complete and Validated

---

## Task Execution Summary

- **Total Tasks**: 30
- **Phase 1 (Setup)**: 4 tasks
- **Phase 2 (Foundation)**: 6 tasks
- **Phase 3 (User Story 1 - Add New Cluster)**: 5 tasks
- **Phase 4 (User Story 2 - Edit Existing Cluster)**: 4 tasks
- **Phase 5 (User Story 3 - Remove Cluster)**: 4 tasks
- **Phase 6 (User Story 4 - View Clusters)**: 3 tasks
- **Phase 7 (Cross-cutting & Polish)**: 4 tasks

---

## Phase 1: Setup & Infrastructure (Foundational - MUST COMPLETE FIRST)

- [x] T001 Create `ClusterConfiguration` entity in `src/Watcher.Domain/Entities/ClusterConfiguration.cs` with properties: `Id`, `Name`, `ScoreMin`, `ScoreMax`, `AgeMin`, `AgeMax`, `DebtRule`, `BaseLimit`, `Cap`

- [x] T002 Create `IClusterConfigurationRepository` in `src/Watcher.Domain/Interfaces/IClusterConfigurationRepository.cs` with methods: `GetAllAsync()`, `GetByIdAsync(string id)`, `AddAsync(ClusterConfiguration cluster)`, `UpdateAsync(ClusterConfiguration cluster)`, `DeleteAsync(string id)`

- [x] T003 Implement mocked in-memory repository `src/Watcher.Infrastructure/Mocks/ClusterConfigurationRepository.cs` that returns a hardcoded table of user-defined cluster configurations and supports CRUD operations

- [x] T004 Register `IClusterConfigurationRepository` and `IClusterConfigurationService` in `src/Watcher.Api/Program.cs` using DI

---

## Phase 2: Foundational Service & Model Layer (Blocking prerequisites)

- [x] T005 Create API request/response DTOs in `src/Watcher.Api/Models/ClusterConfigurationRequest.cs` and `src/Watcher.Api/Models/ClusterConfigurationResponse.cs`

- [x] T006 Create `IClusterConfigurationService` in `src/Watcher.Domain/Interfaces/IClusterConfigurationService.cs` with methods: `CreateAsync`, `UpdateAsync`, `DeleteAsync`, `GetAllAsync`, `GetByIdAsync`

- [x] T007 Implement `ClusterConfigurationService` in `src/Watcher.Domain/Services/ClusterConfigurationService.cs` to validate configurations, route CRUD operations to the repository, and expose cluster rules

- [x] T008 Add debt rule evaluation support in `src/Watcher.Domain/Services/ClusterRuleEvaluator.cs` (or inside `ClusterConfigurationService.cs`) to evaluate debt Boolean expressions against the input `Customer` object using C# notation

- [x] T009 Update `src/Watcher.Domain/Services/ClassificationService.cs` to load cluster definitions from `IClusterConfigurationService` instead of hardcoded cluster constants and evaluate score/age/debt rules from configuration

- [x] T010 Create AutoMapper profile `src/Watcher.Api/Mappings/ClusterConfigurationMappingProfile.cs` to map between API DTOs and `ClusterConfiguration` domain entities

---

## Phase 3: User Story 1 - Add New Cluster (Priority: P1)

- [x] T011 [US1] Add POST `/clusters` endpoint in `src/Watcher.Api/Controllers/ClusterController.cs` to create new cluster configurations

- [x] T012 [US1] Implement `CreateAsync` workflow in `ClusterConfigurationService` to validate and save new cluster definitions

- [x] T013 [US1] Add input validation for cluster configuration in `src/Watcher.Api/Validators/ClusterConfigurationRequestValidator.cs` or service layer, including range checks and `BaseLimit <= Cap`

- [x] T014 [US1] Add unit test in `tests/Watcher.Domain.UnitTests/ClusterConfigurationServiceTests.cs` for successful cluster creation with valid configuration

- [x] T015 [US1] Add unit test in `tests/Watcher.Domain.UnitTests/ClusterConfigurationServiceTests.cs` for invalid cluster input returning validation errors

---

## Phase 4: User Story 2 - Edit Existing Cluster (Priority: P2)

- [x] T016 [US2] Add PUT `/clusters/{id}` endpoint in `src/Watcher.Api/Controllers/ClusterController.cs` to update existing cluster configurations

- [x] T017 [US2] Implement `UpdateAsync` workflow in `ClusterConfigurationService` with conflict validation and not-found handling

- [x] T018 [US2] Add unit test in `tests/Watcher.Domain.UnitTests/ClusterConfigurationServiceTests.cs` for successful cluster update

- [x] T019 [US2] Add unit test in `tests/Watcher.Domain.UnitTests/ClusterConfigurationServiceTests.cs` for updating a missing cluster returning not found behavior

---

## Phase 5: User Story 3 - Remove Cluster (Priority: P3)

- [x] T020 [US3] Add DELETE `/clusters/{id}` endpoint in `src/Watcher.Api/Controllers/ClusterController.cs`

- [x] T021 [US3] Implement `DeleteAsync` workflow in `ClusterConfigurationService` to remove unused cluster definitions from the mocked repository

- [x] T022 [US3] Add unit/integration test in `tests/Watcher.Api.IntegrationTests/ClusterConfigurationTests.cs` for successful cluster deletion

- [x] T023 [US3] Add unit/integration test for deleting a non-existing cluster and returning a 404 response

---

## Phase 6: User Story 4 - View Cluster Configurations (Priority: P4)

- [x] T024 [US4] Add GET `/clusters` endpoint in `src/Watcher.Api/Controllers/ClusterController.cs` to retrieve all configured clusters

- [x] T025 [US4] Implement `GetAllAsync` in `ClusterConfigurationService` and return mapped response DTOs

- [x] T026 [US4] Add integration test in `tests/Watcher.Api.IntegrationTests/ClusterConfigurationTests.cs` for listing configured clusters

---

## Phase 7: Cross-Cutting & Polish

- [x] T027 [P] Update `src/Watcher.Api/Controllers/CustomerController.cs` classification flow to use cluster configurations from `IClusterConfigurationService` and evaluate `DebtRule` expressions in C# notation

- [x] T028 [P] Add integration test in `tests/Watcher.Api.IntegrationTests/CustomerClassificationTests.cs` verifying classification uses a user-defined cluster rule from mocked configuration

- [x] T029 [P] Document the new cluster API in `specs/003-parametrize-cluster-definition/contracts/api-contract.md` and quickstart instructions in `specs/003-parametrize-cluster-definition/quickstart.md`

- [x] T030 Review and tidy `specs/003-parametrize-cluster-definition/plan.md`, `data-model.md`, and `spec.md` for consistency with implementation

---

## Dependencies

- `T001` through `T004` must complete before `T005` through `T010`
- `T005` through `T010` form the foundation for all user story phases
- `T011`-`T015` should be implemented before `T016`-`T026` where possible
- `T027`-`T028` depend on cluster configuration and classification integration work

## Parallel Execution Opportunities

- `T005`, `T006`, `T007`, `T008`, and `T010` can be developed concurrently once foundational entities exist
- DTO/mapping work (`T005`, `T010`) is parallelizable with repository implementation (`T003`)
- Test creation tasks (`T014`-`T019`, `T022`-`T026`) can run in parallel with endpoint implementation once service methods exist

## Implementation Strategy

1. Build the domain model and mock repository first to satisfy the no-database requirement.  
2. Implement cluster CRUD services and API endpoints as independent user stories.  
3. Integrate classification logic last so the new cluster definitions drive customer classification and debt-rule evaluation.  
4. Finish with integration tests, documentation, and plan/spec alignment.
