# Implementation Tasks: Update Risk Rules

**Feature**: Update Risk Rules  
**Branch**: `002-update-risk-rules`  
**Date**: 2026-04-25  
**Status**: Task Generation Complete

---

## Task Execution Summary

- **Total Tasks**: 24
- **Setup/Foundation Phase**: 4 tasks
- **User Story 1 (Classification)**: 9 tasks
- **User Story 2 (Credit Limits)**: 8 tasks
- **Polish & Integration**: 3 tasks
- **Dependencies**: Minimal - can execute phases in parallel after foundational setup

---

## Phase 1: Setup & Infrastructure (Foundational - MUST COMPLETE FIRST)

Setup tasks ensure the domain project is restructured and dependencies are in place.

### Folder Structure & Organization

- [ ] T001 Create domain project folder structure: `/Handlers`, `/Services`, `/Interfaces`, `/Entities`, `/Mappers`, `/Requests`, `/Responses` directories in `src/Watcher.Domain/`

- [ ] T002 Move existing Commands to organized folder (if not already in Commands/): `src/Watcher.Domain/Commands/ClassifyCustomerCommand.cs`

- [ ] T003 Verify MediatR registration in DI container: `src/Watcher.Api/Program.cs` includes `builder.Services.AddMediatR()`

- [ ] T004 Verify AutoMapper registration in DI container: `src/Watcher.Api/Program.cs` includes `builder.Services.AddAutoMapper()`

---

## Phase 2: Core Domain Entities (Foundation for both user stories - PARALLELIZABLE after Phase 1)

These tasks create the new domain entities required for classification and credit limit calculation.

### Cluster Entity Creation

- [ ] T005 [P] Create Cluster entity as immutable record in `src/Watcher.Domain/Entities/Cluster.cs` with properties: IdCluster (string), Name (string), BaseLimit (decimal), Cap (decimal)

- [ ] T006 [P] Create ClusterDefinitions static class in `src/Watcher.Domain/Entities/Cluster.cs` with four predefined cluster instances (CLUSTER_A: Diamond 50000-100000, CLUSTER_B: Gold 20000-40000, CLUSTER_C: Silver 5000-10000, CLUSTER_D: Bronze 0-0)

- [ ] T007 [P] Create ClassificationResult entity as immutable record in `src/Watcher.Domain/Entities/ClassificationResult.cs` with properties: CustomerId (string), Cluster (Cluster), CreditLimit (decimal), CalculatedAt (DateTime)

### Request/Response DTOs Creation

- [ ] T008 [P] Create ClassifyCustomerRequest DTO in `src/Watcher.Domain/Requests/ClassifyCustomerRequest.cs` with properties: CustomerId, Score, Age, HasMarketDebt, MarketDebtTypes

- [ ] T009 [P] Create ClusterResponse DTO in `src/Watcher.Domain/Responses/ClusterResponse.cs` with properties: IdCluster, Name, BaseLimit, Cap

- [ ] T010 [P] Create ClassifyCustomerResponse DTO in `src/Watcher.Domain/Responses/ClassifyCustomerResponse.cs` with properties: CustomerId, Cluster (ClusterResponse), CreditLimit, CalculatedAt

---

## Phase 3: User Story 1 - Customer Risk Classification (Priority: P1)

Implement priority-based classification rules returning the assigned cluster.

### Classification Service Enhancement

- [ ] T011 [US1] Update ClassificationService interface: `src/Watcher.Domain/Services/IClassificationService.cs` to define method `Task<ClassificationResult> ClassifyAsync(Customer customer)`

- [ ] T012 [US1] Implement priority-based cluster evaluation in ClassificationService.EvaluateCluster(): `src/Watcher.Domain/Services/ClassificationService.cs` - Check CLUSTER_A (Score >= 700 AND Age 25-60 AND no market debt)

- [ ] T013 [US1] Implement CLUSTER_B evaluation logic in ClassificationService: Score >= 500 AND Age 18-65 AND NO credit_default/loan_default in MarketDebtTypes

- [ ] T014 [US1] Implement CLUSTER_C and CLUSTER_D fallback logic in ClassificationService: Score >= 300 → CLUSTER_C, else CLUSTER_D

- [ ] T015 [US1] Add input validation to ClassificationService.ClassifyAsync(): Verify Score 300-1000, Age 18-100

- [ ] T016 [US1] Create AutoMapper mapping profile in `src/Watcher.Domain/Mappers/ClusterMapper.cs` to map Cluster → ClusterResponse

- [ ] T017 [US1] Create MediatR command handler in `src/Watcher.Domain/Handlers/ClassifyCustomerHandler.cs`: Orchestrate command → service → response mapping

- [ ] T018 [US1] Update CustomerController.Classify() in `src/Watcher.Api/Controllers/CustomerController.cs` to map HTTP request → ClassifyCustomerCommand

- [ ] T019 [US1] Register ClassificationService and ClusterMapper in DI container: `src/Watcher.Api/Program.cs` with `builder.Services.AddScoped<>()`

---

## Phase 4: User Story 2 - Credit Limit Calculation (Priority: P2)

Implement credit limit calculation based on customer score and cluster ranges.

### Credit Limit Calculation Logic

- [ ] T020 [P] [US2] Implement CalculateCreditLimit() method in ClassificationService: Calculate normalized score position (300-1000 range) and scale to cluster's BaseLimit-Cap range

- [ ] T021 [P] [US2] Add bounds checking in credit limit calculation: Ensure CreditLimit >= Cluster.BaseLimit AND CreditLimit <= Cluster.Cap

- [ ] T022 [P] [US2] Handle CLUSTER_D special case: Credit limit always returns 0 (BaseLimit = Cap = 0)

- [ ] T023 [P] [US2] Return complete ClassificationResult in ClassifyAsync(): Include CustomerId, Cluster, CreditLimit, CalculatedAt (UTC timestamp)

- [ ] T024 [US2] Map ClassificationResult → ClassifyCustomerResponse in handler for API response: `src/Watcher.Domain/Handlers/ClassifyCustomerHandler.cs`

---

## Phase 5: Testing (Parallelizable with implementation)

### Unit Tests

- [ ] T025 Test ClassificationService.EvaluateCluster() for CLUSTER_A assignment: Score 750, Age 45, no market debt

- [ ] T026 Test ClassificationService.EvaluateCluster() for CLUSTER_B assignment: Score 600, Age 35, no bad debt types

- [ ] T027 Test ClassificationService.EvaluateCluster() for CLUSTER_C assignment: Score 350, Age 70

- [ ] T028 Test ClassificationService.EvaluateCluster() for CLUSTER_D assignment: Score 250 or fails other rules

- [ ] T029 Test credit limit boundaries: CLUSTER_A customer with score 700 → credit limit between 50000-100000

- [ ] T030 Test credit limit boundaries: CLUSTER_B customer with score 500 → credit limit between 20000-40000

- [ ] T031 Test input validation: Score < 300 or > 1000 throws exception

- [ ] T032 Test input validation: Age < 18 or > 100 throws exception

### Integration Tests

- [ ] T033 Test POST /customers/classify endpoint with valid CLUSTER_A request → 200 OK with cluster object

- [ ] T034 Test POST /customers/classify endpoint with valid CLUSTER_D request → 200 OK with credit limit 0

- [ ] T035 Test POST /customers/classify endpoint with invalid score → 422 Unprocessable Entity

- [ ] T036 Test POST /customers/classify response includes all fields: customerId, cluster, creditLimit, calculatedAt

---

## Phase 6: Polish & Cross-Cutting Concerns

### Boundary Testing & Edge Cases

- [ ] T037 Test boundary values: Age exactly 25, 60, 18, 65 (inclusive ranges) in `tests/Watcher.Domain.UnitTests/ClassificationServiceTests.cs`

- [ ] T038 Test boundary values: Score exactly 700, 500, 300 (inclusive comparisons) in unit tests

- [ ] T039 Test edge case: MarketDebtTypes array contains both allowed and disallowed debt types → customer only qualifies if NO bad debts

### Documentation & Verification

- [ ] T040 Verify API response contract matches OpenAPI schema in `specs/002-update-risk-rules/contracts/api-contract.md`

- [ ] T041 Verify project structure matches plan in `specs/002-update-risk-rules/plan.md`

- [ ] T042 Run full test suite and verify all tests pass (unit + integration)

- [ ] T043 Verify backward compatibility: Existing clients can still read cluster name from response

---

## Task Dependencies

### Critical Path

```
T001-T004 (Setup)
    ↓
T005-T010 (Domain Entities) 
    ↓
T011-T019 (User Story 1: Classification)
    ↓
T020-T024 (User Story 2: Credit Limits)
    ↓
T025-T036 (Testing)
    ↓
T037-T043 (Polish & Verification)
```

### Parallelizable Tasks

**After T001-T004 complete**:
- T005-T010 can run in parallel (entity creation)

**After T005-T010 complete**:
- T011-T019 and T020-T024 can run in parallel (different concerns)
- T025-T036 can start immediately (testing can parallel implementation)

**After T037-T043 complete**:
- Verify all phases complete successfully

---

## Implementation Hints

### Task Groups by Component

**Service Layer**:
- T011-T015: Update IClassificationService and ClassificationService
- T020-T023: Add credit limit calculation logic

**Data Layer**:
- T005-T007: Create Cluster entities
- T008-T010: Create DTOs

**Application Layer**:
- T016: AutoMapper profile
- T017-T018: MediatR handler and controller updates
- T019: DI registration

**Testing**:
- T025-T036: Unit and integration tests
- T037-T039: Boundary and edge case testing

---

## Phase Allocation Table

| Phase | Tasks | Duration | Dependencies | Parallelizable |
|-------|-------|----------|--------------|-----------------|
| 1: Setup | T001-T004 | 30 min | None | Yes (independent ops) |
| 2: Entities | T005-T010 | 45 min | Phase 1 | Yes (6 independent entities) |
| 3: US1 Classification | T011-T019 | 90 min | Phase 2 | Partial (service then handler) |
| 4: US2 Credit Limits | T020-T024 | 60 min | Phase 3 (Classification) | Partial (depends on classification) |
| 5: Testing | T025-T036 | 120 min | Phases 3-4 | Yes (tests independent) |
| 6: Polish | T037-T043 | 45 min | Phase 5 | Partial (verification steps) |
| **TOTAL** | **43** | **~390 min (6.5 hrs)** | - | - |

---

## Acceptance Criteria Mapping

### User Story 1 Requirements → Tasks

| Requirement | Tasks | Status |
|-------------|-------|--------|
| Classify to CLUSTER_A | T012, T025, T033 | ✓ Testable |
| Classify to CLUSTER_B | T013, T026, T033 | ✓ Testable |
| Classify to CLUSTER_C | T014, T027 | ✓ Testable |
| Classify to CLUSTER_D | T014, T028 | ✓ Testable |
| Boundary conditions | T037-T038 | ✓ Testable |

### User Story 2 Requirements → Tasks

| Requirement | Tasks | Status |
|-------------|-------|--------|
| Credit limit CLUSTER_A | T020-T021, T029 | ✓ Testable |
| Credit limit CLUSTER_B | T020-T021, T030 | ✓ Testable |
| Credit limit CLUSTER_C | T020-T021 | ✓ Testable |
| Credit limit CLUSTER_D | T022 | ✓ Testable |
| Bounds compliance | T021 | ✓ Verified |

---

## Quick Start Implementation

**Fastest Path** (6.5 hours):

1. Complete Phase 1 (30 min) - folders and DI setup
2. Complete Phase 2 in parallel (45 min) - create all entities and DTOs
3. Complete Phase 3 (90 min) - implement classification rules
4. Complete Phase 4 (60 min) - add credit limit calculation
5. Run Phase 5 tests while polishing (120 min) - TDD approach
6. Complete Phase 6 verification (45 min) - validate and document

---

## Implementation Status Tracking

Use this checklist to track progress:

```markdown
## Progress Checklist

### Phase 1: Setup ✓
- [ ] T001: Folder structure created
- [ ] T002: Commands organized
- [ ] T003: MediatR verified
- [ ] T004: AutoMapper verified

### Phase 2: Entities
- [ ] T005: Cluster entity created
- [ ] T006: ClusterDefinitions created
- [ ] T007: ClassificationResult created
- [ ] T008: ClassifyCustomerRequest DTO
- [ ] T009: ClusterResponse DTO
- [ ] T010: ClassifyCustomerResponse DTO

### Phase 3: Classification (US1)
- [ ] T011: IClassificationService updated
- [ ] T012: CLUSTER_A logic
- [ ] T013: CLUSTER_B logic
- [ ] T014: CLUSTER_C/D logic
- [ ] T015: Input validation
- [ ] T016: AutoMapper profile
- [ ] T017: MediatR handler
- [ ] T018: Controller update
- [ ] T019: DI registration

### Phase 4: Credit Limits (US2)
- [ ] T020: CalculateCreditLimit() method
- [ ] T021: Bounds checking
- [ ] T022: CLUSTER_D special case
- [ ] T023: Return ClassificationResult
- [ ] T024: Response mapping

### Phase 5: Testing
- [ ] T025-T036: All tests passing

### Phase 6: Polish
- [ ] T037-T043: Verification complete
```

---

## Notes for Implementation

- **No Breaking Changes**: API endpoint URL and request format remain the same
- **Backward Compatibility**: Existing clients continue to work with extended response
- **In-Memory Data**: No database changes needed - classification is stateless
- **Error Handling**: Implement proper 400/422/500 error responses per API contract
- **Performance**: Ensure response time < 100ms (classification is O(4) evaluation)
- **Timezone**: Use UTC for all DateTime fields (CalculatedAt)

---

## Next Steps

1. **Start with Phase 1**: Create folder structure (T001-T004)
2. **Build entities**: Implement all domain entities (T005-T010)
3. **Implement classification**: Priority-based rules (T011-T019)
4. **Add credit limits**: Score-based calculation (T020-T024)
5. **Test thoroughly**: Unit and integration tests (T025-T036)
6. **Verify**: Boundary cases and polish (T037-T043)

**Ready to implement?** Begin with T001 in Phase 1.
