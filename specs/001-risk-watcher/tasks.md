# Tasks: Risk Watcher

**Input**: Design documents from `/specs/001-risk-watcher/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Unit and integration tests included as per constitution requirements.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- Solution at repository root: RiskWatcher.sln
- Projects: src/Watcher.Api/, src/Watcher.Domain/, src/Watcher.Infrastructure/
- Tests: tests/Watcher.Api.UnitTests/, tests/Watcher.Domain.UnitTests/, tests/Watcher.Api.IntegrationTests/

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [x] T001 Create .NET solution with three projects per plan.md
- [x] T002 Add NuGet packages: ASP.NET Core Web API, MediatR, AutoMapper, xUnit, Moq, Swashbuckle.AspNetCore

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T003 Setup dependency injection container in Watcher.Api/Program.cs
- [x] T004 [P] Create base interfaces in Watcher.Domain/Interfaces/
- [x] T005 [P] Setup AutoMapper profiles in Watcher.Domain/

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Customer Risk Classification (Priority: P1) 🎯 MVP

**Goal**: Classify customers into risk clusters using business rules

**Independent Test**: POST /customers/classify endpoint accepts customer data and returns enriched data with cluster assignment

### Tests for User Story 1 ⚠️

- [x] T006 [P] [US1] Unit tests for ClassificationService in tests/Watcher.Domain.UnitTests/
- [x] T007 [P] [US1] Integration test for POST /customers/classify in tests/Watcher.Api.IntegrationTests/

### Implementation for User Story 1

- [x] T008 [P] [US1] Create Customer entity in src/Watcher.Domain/Entities/Customer.cs
- [x] T009 [P] [US1] Create IClassificationService interface in src/Watcher.Domain/Interfaces/IClassificationService.cs
- [x] T010 [US1] Implement ClassificationService with business rules in src/Watcher.Domain/Services/ClassificationService.cs
- [x] T011 [P] [US1] Create ICustomerRepository interface in src/Watcher.Domain/Interfaces/ICustomerRepository.cs
- [x] T012 [US1] Implement mock CustomerRepository in src/Watcher.Infrastructure/Mocks/CustomerRepository.cs
- [x] T013 [P] [US1] Create ClassifyCustomerCommand and handler in src/Watcher.Domain/Commands/
- [x] T014 [US1] Create CustomerController with POST /customers/classify in src/Watcher.Api/Controllers/CustomerController.cs
- [x] T015 [P] [US1] Add AutoMapper mapping for Customer in src/Watcher.Api/Mappings/CustomerMappingProfile.cs
- [x] T016 [US1] Configure Swagger documentation in Watcher.Api/Program.cs
- [x] T017 [US1] Add input validation using FluentValidation in src/Watcher.Api/

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Quality improvements, monitoring, and production readiness

- [x] T018 [P] Add health checks endpoint in src/Watcher.Api/Program.cs
- [x] T019 [P] Configure structured logging in Watcher.Api/Program.cs
- [x] T020 [P] Add performance monitoring and metrics via /metrics in src/Watcher.Api/Program.cs
- [x] T021 Update README.md with API documentation
- [x] T022 Configure CORS if needed for web channel

---

## Dependencies

**Story Completion Order**:
1. User Story 1 (P1) - Core classification functionality
2. User Story 2 (P2) - Credit limit calculation (future implementation)
3. User Story 3 (P3) - Income estimation (future implementation)

**Task Dependencies**:
- Phase 1 tasks must complete before Phase 2
- Phase 2 tasks must complete before Phase 3
- Within Phase 3: Parallel tasks can run simultaneously, sequential tasks depend on parallel ones

**Parallel Execution Examples**:
- Setup: T001 → T002 (sequential)
- Foundation: T003, T004, T005 (parallel)
- US1 Tests: T006, T007 (parallel)
- US1 Impl: T008, T009, T011, T013, T015, T017 (parallel) → T010, T012, T014, T016 (sequential dependencies)

## Implementation Strategy

**MVP Scope**: User Story 1 only - provides core risk classification API
**Incremental Delivery**: Each user story can be deployed independently
**Testing Strategy**: Unit tests for domain logic, integration tests for API endpoints
**Quality Gates**: All tests pass, performance <500ms, constitution compliance

**Total Tasks**: 22
**Tasks per User Story**: US1: 12 tasks (including tests)
**Parallel Opportunities**: 13 tasks marked with [P]
**Independent Test Criteria**: Each story has clear test criteria for independent validation