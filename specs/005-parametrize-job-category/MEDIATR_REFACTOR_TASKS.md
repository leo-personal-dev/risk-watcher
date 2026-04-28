# MediatR Refactoring Implementation Tasks

**Status**: Ready to Execute  
**Estimated Effort**: 8-10 days  
**Priority**: High  
**Target Completion**: End of Sprint  

## Overview

This document provides an actionable, dependency-ordered task list for refactoring the Watcher.Api from a direct service-based architecture to use the MediatR mediator pattern with clean architecture principles.

### Success Criteria
- ✅ All domain entities live in Watcher.Domain/Entities/
- ✅ All handlers live in Watcher.Domain/Handlers/
- ✅ All commands in Watcher.Domain/Commands/
- ✅ Watcher.Api has NO references to domain entities or services
- ✅ Watcher.Api only knows about commands, DTOs, and MediatR
- ✅ All tests pass (unit and integration)
- ✅ API functionality unchanged from external perspective

---

## Phase 1: Domain Structure Setup (Blocking)

*This phase creates the foundational folder structure and command/response classes. All subsequent work depends on these artifacts existing.*

### Directory Structure

- [x] T001 Create folder structure in Watcher.Domain
  - Create: `src/Watcher.Domain/Commands/Request/`
  - Create: `src/Watcher.Domain/Commands/Response/`
  - Create: `src/Watcher.Domain/Handlers/`
  - **Success**: Folders exist and are visible in Solution Explorer

### Classification Command Structure

- [x] T002 [P] Create `ClassifyCustomerCommand` request class
  - **File**: `src/Watcher.Domain/Commands/Request/ClassifyCustomerCommand.cs`
  - **Properties**: CustomerId (string), Score (decimal), Age (int), HasMarketDebt (bool), MarketDebtTypes (List<string>), JobTitle (string)
  - **Implements**: IRequest<ClassifyCustomerResponse>
  - **Success**: Class compiles, implements interface correctly

- [x] T003 [P] Create `ClassifyCustomerResponse` response class
  - **File**: `src/Watcher.Domain/Commands/Response/ClassifyCustomerResponse.cs`
  - **Properties**: CustomerId (string), Cluster (string), JobCategory (string), CreditLimit (decimal), CalculatedAt (DateTime)
  - **Success**: Class compiles, ready for handler usage

### Job Category Command Structure

- [x] T004 [P] Create Job Category command request classes
  - **Files** (create all in `src/Watcher.Domain/Commands/Request/`):
    - `GetAllJobCategoriesCommand.cs` → IRequest<GetAllJobCategoriesResponse>
    - `GetJobCategoryByIdCommand.cs` → IRequest<GetJobCategoryResponse> (with JobCategoryId property)
    - `CreateJobCategoryCommand.cs` → IRequest<CreateJobCategoryResponse> (with Name, Multiplier, Keywords properties)
    - `UpdateJobCategoryCommand.cs` → IRequest<UpdateJobCategoryResponse> (with Id, Name, Multiplier, Keywords)
    - `DeleteJobCategoryCommand.cs` → IRequest<Unit> (with JobCategoryId property)
  - **Success**: All 5 command classes compile, interfaces implemented correctly

- [x] T005 [P] Create Job Category command response classes
  - **Files** (create all in `src/Watcher.Domain/Commands/Response/`):
    - `GetAllJobCategoriesResponse.cs` → wrapper with JobCategories property (List<GetJobCategoryResponse>)
    - `GetJobCategoryResponse.cs` → with Id, Name, Multiplier, Keywords properties
    - `CreateJobCategoryResponse.cs` → with Id, Name, Multiplier, Keywords
    - `UpdateJobCategoryResponse.cs` → with Id, Name, Multiplier, Keywords
  - **Success**: All response classes compile, match command structure

---

## Phase 2: Handler Implementation

*Phase 2 creates the actual handler logic that processes commands and calls existing services. Handlers remain independent from API layer concerns.*

### Classification Handler

- [x] T006 [P] Implement `ClassifyCustomerCommandHandler`
  - **File**: `src/Watcher.Domain/Handlers/ClassifyCustomerCommandHandler.cs`
  - **Implements**: IRequestHandler<ClassifyCustomerCommand, ClassifyCustomerResponse>
  - **Logic**:
    - Inject `IClassificationService` (already exists)
    - In Handle() method: call service.Classify()
    - Map ClassificationResult to ClassifyCustomerResponse
    - Return response
  - **Success**: Handler compiles, no domain entity references in API layer

### Job Category Handlers

- [x] T007 [P] Implement `GetAllJobCategoriesCommandHandler`
  - **File**: `src/Watcher.Domain/Handlers/GetAllJobCategoriesCommandHandler.cs`
  - **Implements**: IRequestHandler<GetAllJobCategoriesCommand, GetAllJobCategoriesResponse>
  - **Logic**:
    - Inject `IJobCategoryService` (already exists)
    - Call service.GetAllJobCategories()
    - Map results to GetJobCategoryResponse list
    - Return wrapped response
  - **Success**: Handler compiles and is discoverable by MediatR

- [x] T008 [P] Implement `GetJobCategoryByIdCommandHandler`
  - **File**: `src/Watcher.Domain/Handlers/GetJobCategoryByIdCommandHandler.cs`
  - **Implements**: IRequestHandler<GetJobCategoryByIdCommand, GetJobCategoryResponse>
  - **Logic**:
    - Inject `IJobCategoryService`
    - Call service.GetJobCategoryById(command.JobCategoryId)
    - Map to GetJobCategoryResponse
    - Return response (null-safe handling)
  - **Success**: Handler handles not-found cases gracefully

- [x] T009 [P] Implement `CreateJobCategoryCommandHandler`
  - **File**: `src/Watcher.Domain/Handlers/CreateJobCategoryCommandHandler.cs`
  - **Implements**: IRequestHandler<CreateJobCategoryCommand, CreateJobCategoryResponse>
  - **Logic**:
    - Inject `IJobCategoryService`
    - Create JobCategory entity from command properties
    - Call service.AddJobCategory(entity)
    - Map created entity to CreateJobCategoryResponse
    - Return response
  - **Success**: New job categories are created and assigned IDs

- [x] T010 [P] Implement `UpdateJobCategoryCommandHandler`
  - **File**: `src/Watcher.Domain/Handlers/UpdateJobCategoryCommandHandler.cs`
  - **Implements**: IRequestHandler<UpdateJobCategoryCommand, UpdateJobCategoryResponse>
  - **Logic**:
    - Inject `IJobCategoryService`
    - Call service.GetJobCategoryById(command.Id)
    - Update entity properties (Name, Multiplier, Keywords)
    - Call service.UpdateJobCategory(entity)
    - Map updated entity to UpdateJobCategoryResponse
    - Return response
  - **Success**: Existing categories are updated correctly

- [x] T011 [P] Implement `DeleteJobCategoryCommandHandler`
  - **File**: `src/Watcher.Domain/Handlers/DeleteJobCategoryCommandHandler.cs`
  - **Implements**: IRequestHandler<DeleteJobCategoryCommand, Unit>
  - **Logic**:
    - Inject `IJobCategoryService`
    - Call service.DeleteJobCategory(command.JobCategoryId)
    - Return Unit.Value
  - **Success**: Categories are deleted and reflected in subsequent queries

---

## Phase 3: Watcher.Api Restructuring

*Phase 3 creates API-specific DTOs and updates controllers to use MediatR. The API layer remains ignorant of domain entities and business logic.*

### API DTO Models

- [x] T012 [P] Create API request DTOs for Classification
  - **File**: `src/Watcher.Api/Models/Requests/ClassifyCustomerRequest.cs`
  - **Properties**: CustomerId (string), Score (decimal), Age (int), HasMarketDebt (bool), MarketDebtTypes (List<string>), JobTitle (string)
  - **Note**: These mirror ClassifyCustomerCommand but are API-specific contracts
  - **Success**: DTO compiles and is distinct from domain entity

- [x] T013 [P] Create API request DTOs for Job Categories
  - **Files** (create in `src/Watcher.Api/Models/Requests/`):
    - `CreateJobCategoryRequest.cs` → with Name, Multiplier, Keywords properties
    - `UpdateJobCategoryRequest.cs` → with Name, Multiplier, Keywords properties
  - **Note**: No ID in requests (create), separate body for updates
  - **Success**: DTOs follow API conventions (camelCase in JSON)

- [x] T014 [P] Create API response DTOs
  - **Files** (create in `src/Watcher.Api/Models/Responses/`):
    - `ClassifyCustomerResponse.cs` → with CustomerId, Cluster, JobCategory, CreditLimit, CalculatedAt
    - `JobCategoryResponse.cs` → with Id, Name, Multiplier, Keywords
    - `JobCategoriesListResponse.cs` → wrapper with JobCategories property
  - **Note**: These are API contracts, not domain entities
  - **Success**: Responses are JSON-serializable and camelCase-compliant

### AutoMapper Configuration

- [x] T015 [P] Create AutoMapper profile for API ↔ Command mapping
  - **File**: `src/Watcher.Api/Mappings/CommandApiMappingProfile.cs`
  - **Mappings** (create bidirectional):
    - ClassifyCustomerRequest → ClassifyCustomerCommand
    - ClassifyCustomerResponse (domain) → ClassifyCustomerResponse (API)
    - CreateJobCategoryRequest → CreateJobCategoryCommand
    - UpdateJobCategoryRequest → UpdateJobCategoryCommand
    - GetJobCategoryResponse (domain) → JobCategoryResponse (API)
  - **Success**: All API DTOs can be mapped to/from domain commands
  - **Note**: Register profile in Program.cs (handled in Phase 4)

### Controller Updates

- [x] T016 Update `ClassificationController`
  - **File**: `src/Watcher.Api/Controllers/ClassificationController.cs`
  - **Changes**:
    - Remove dependency: `IClassificationService`
    - Add dependency: `IMediator` (from MediatR)
    - Update Classify() action (e.g., POST /api/classify):
      1. Receive ClassifyCustomerRequest
      2. Validate input (use FluentValidation or ModelState)
      3. Map request to ClassifyCustomerCommand using IMapper
      4. Send command: `var response = await _mediator.Send(command);`
      5. Map response to API model: `var apiResponse = _mapper.Map<ClassifyCustomerResponse>(response);`
      6. Return 200 OK with response
  - **Success**: Controller compiles, no domain service references

- [x] T017 Update `JobCategoryController`
  - **File**: `src/Watcher.Api/Controllers/JobCategoryController.cs`
  - **Changes**:
    - Remove dependency: `IJobCategoryService`
    - Add dependency: `IMediator`
    - Update each action:
      - **GetAll()** (GET /api/job-categories):
        1. Send GetAllJobCategoriesCommand
        2. Map GetAllJobCategoriesResponse to API model
        3. Return 200 OK
      - **GetById(id)** (GET /api/job-categories/{id}):
        1. Send GetJobCategoryByIdCommand
        2. Check for null, return 404 if not found
        3. Map to API response
        4. Return 200 OK
      - **Create(request)** (POST /api/job-categories):
        1. Validate input
        2. Map to CreateJobCategoryCommand
        3. Send command
        4. Map response to API model
        5. Return 201 Created with location header
      - **Update(id, request)** (PUT /api/job-categories/{id}):
        1. Validate input
        2. Map to UpdateJobCategoryCommand (include id)
        3. Send command
        4. Map response to API model
        5. Return 200 OK
      - **Delete(id)** (DELETE /api/job-categories/{id}):
        1. Send DeleteJobCategoryCommand
        2. Return 204 No Content
  - **Success**: All CRUD operations work through mediator

---

## Phase 4: Configuration & Dependency Injection

*Phase 4 registers MediatR, handlers, and validators in the DI container. This is the critical integration point.*

### MediatR & Validator Configuration

- [x] T018 Update `Program.cs` for MediatR registration
  - **File**: `src/Watcher.Api/Program.cs`
  - **Changes**:
    1. Add using: `using MediatR;`
    2. Add using: `using FluentValidation;`
    3. Register MediatR (after builder creation):
       ```csharp
       builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
           typeof(Program).Assembly,
           typeof(Watcher.Domain.Handlers.ClassifyCustomerCommandHandler).Assembly
       ));
       ```
    4. Register FluentValidation validators:
       ```csharp
       builder.Services.AddValidatorsFromAssemblies(
           typeof(Program).Assembly,
           typeof(Watcher.Domain.Handlers.ClassifyCustomerCommandHandler).Assembly
       );
       ```
    5. Add AutoMapper profile registration:
       ```csharp
       builder.Services.AddAutoMapper(typeof(CommandApiMappingProfile), 
           typeof(Watcher.Domain.Mappings.CommandMappingProfile));
       ```
    6. Verify existing service registrations are still present:
       - IClassificationService
       - IJobCategoryService
       - IClusterConfigurationService
       - All repositories
  - **Success**: Application starts without DI errors

### Command Validators

- [x] T019 [P] Create FluentValidation validators for commands
  - **Files** (create in `src/Watcher.Domain/Validators/`):
    - `ClassifyCustomerCommandValidator.cs`:
      - Validate CustomerId is not empty
      - Validate Score is between 0 and 1000
      - Validate Age is >= 18
      - Validate JobTitle length (not null/empty)
    - `CreateJobCategoryCommandValidator.cs`:
      - Validate Name is not empty, length 3-50
      - Validate Multiplier > 0
      - Validate Keywords has at least 1 item, max 50 items
      - Validate each keyword is not empty
    - `UpdateJobCategoryCommandValidator.cs`:
      - Validate Id is not empty (Guid)
      - Same as CreateJobCategoryCommandValidator for other properties
    - `GetJobCategoryByIdCommandValidator.cs`:
      - Validate JobCategoryId is not empty
  - **Success**: All validators compile and can be auto-discovered

### Dependency Verification

- [x] T020 Verify dependency injection configuration
  - **File**: `src/Watcher.Api/Program.cs`
  - **Checks**:
    - [ ] IMediator is resolvable
    - [ ] IMapper (AutoMapper) is resolvable
    - [ ] IClassificationService is resolvable
    - [ ] IJobCategoryService is resolvable
    - [ ] All repositories are resolvable
    - [ ] All validators are registered
  - **Success**: Run `dotnet build` without errors

---

## Phase 5: Testing

*Phase 5 validates the refactored architecture through handler tests and integration tests.*

### Handler Unit Tests

- [x] T021 [P] Create handler tests in unit test project
  - **File**: `tests/Watcher.Domain.UnitTests/Handlers/ClassifyCustomerCommandHandlerTests.cs`
  - **Tests**:
    - Test successful classification with valid command
    - Test handler receives correct service call
    - Test response mapping is correct
    - Test handler with missing customer data
  - **Success**: Tests pass and validate handler logic

- [x] T022 [P] Create handler tests for Job Categories
  - **Files** (create in `tests/Watcher.Domain.UnitTests/Handlers/`):
    - `GetAllJobCategoriesCommandHandlerTests.cs`:
      - Test returns all categories
      - Test returns empty list when no categories exist
    - `GetJobCategoryByIdCommandHandlerTests.cs`:
      - Test returns category when found
      - Test returns null when category not found
    - `CreateJobCategoryCommandHandlerTests.cs`:
      - Test creates category with correct properties
      - Test validates required fields
    - `UpdateJobCategoryCommandHandlerTests.cs`:
      - Test updates existing category
      - Test handles category not found
    - `DeleteJobCategoryCommandHandlerTests.cs`:
      - Test deletes category
      - Test subsequent query returns nothing
  - **Success**: All tests pass

### Controller Integration Tests

- [x] T023 Update ClassificationController integration tests
  - **File**: `tests/Watcher.Api.IntegrationTests/ClassificationControllerTests.cs` (or create if not exists)
  - **Tests**:
    - Test POST /api/classify with valid request
    - Test response contains all required fields
    - Test invalid request returns 400 Bad Request
    - Test validation errors are returned
  - **Success**: All tests pass without direct service mocking

- [x] T024 Update JobCategoryController integration tests
  - **File**: `tests/Watcher.Api.IntegrationTests/JobCategoryControllerTests.cs` (or `JobCategoryTests.cs`)
  - **Tests**:
    - Test GET /api/job-categories returns list
    - Test POST /api/job-categories creates new category
    - Test GET /api/job-categories/{id} returns specific category
    - Test PUT /api/job-categories/{id} updates category
    - Test DELETE /api/job-categories/{id} removes category
    - Test 404 responses for non-existent resources
  - **Success**: CRUD operations work end-to-end

### Full Test Suite Validation

- [x] T025 Run complete test suite
  - **Command**: `dotnet test` from project root
  - **Verification**:
    - [ ] All unit tests pass
    - [ ] All integration tests pass
    - [ ] No test failures or skipped tests
    - [ ] Code coverage maintained (if applicable)
  - **Success**: All tests pass, 100% success rate

---

## Phase 6: Cleanup & Documentation

*Phase 6 removes obsolete code and documents the new architecture.*

### Code Cleanup

- [x] T026 Remove obsolete service references
  - **Changes**:
    - Delete or disable old response mapping classes that are now redundant
    - Remove any direct service injection from controllers (already done in T016-T017)
    - Clean up any unused using statements
  - **Success**: No compiler warnings about unused references

- [x] T027 Verify no domain entity leakage
  - **Validation**:
    - [ ] Run semantic analysis: Search Watcher.Api project for imports of Watcher.Domain entities
    - [ ] Verify no direct use of domain models in controllers
    - [ ] Confirm all communication is through commands/responses
  - **Files to check**:
    - src/Watcher.Api/Controllers/*.cs (should only reference API models and MediatR)
    - src/Watcher.Api/Models/ (should have no inheritance from domain)
  - **Success**: Zero domain entity references in Watcher.Api

### Documentation Updates

- [x] T028 Create Architecture Decision Record (ADR)
  - **File**: `MEDIATR_ARCHITECTURE_ADR.md` (or update ARCHITECTURE_PLAN.md)
  - **Contents**:
    - Decision: Implement MediatR pattern
    - Context: Clean architecture, separation of concerns
    - Consequences: 
      - Benefits: Testable, maintainable, extensible
      - Trade-offs: Additional abstractions, small performance overhead
      - Migration path: Phased controller updates
  - **Success**: ADR is committed and accessible

- [x] T029 Update API documentation
  - **Files to update**:
    - `README.md`: Update architecture section with diagram showing MediatR flow
    - API endpoint documentation: Add notes about command-based design
  - **Contents**:
    - High-level diagram: Request → Command → Handler → Service → Response
    - Example request/response flow for Job Categories
    - Developer guide: How to add new features with handlers
  - **Success**: Documentation is clear and current

- [x] T030 Code review walkthrough
  - **Process**:
    - [ ] Review domain handlers with team (T006-T011)
    - [ ] Review controller refactoring (T016-T017)
    - [ ] Verify DI configuration (T018-T020)
    - [ ] Confirm no architectural violations
  - **Success**: Code review approved, no blocking issues

---

## Dependencies & Sequencing

### Critical Path (Must Complete in Order)

1. **T001** → Foundation (creates folders)
2. **T002-T005** → Commands defined (all depend on T001)
3. **T006-T011** → Handlers implement commands (depend on T002-T005)
4. **T012-T015** → API restructuring (depend on T006-T011)
5. **T016-T017** → Controllers updated (depend on T012-T015)
6. **T018-T020** → DI configuration (depend on T016-T017)
7. **T025** → Validate all tests pass (depends on full chain)

### Parallelizable Tasks (Can Run Simultaneously)

**After T001 completes:**
- T002 ↔ T003 (Classification command/response)
- T004 ↔ T005 (Job category command/response)

**After T002-T005 complete:**
- T006 ↔ T007 ↔ T008 ↔ T009 ↔ T010 ↔ T011 (All handlers independent)

**After T006-T011 complete:**
- T012 ↔ T013 ↔ T014 (All API DTOs independent)
- T015 (AutoMapper, depends on T012-T014)

**After T015 complete:**
- T016 ↔ T017 (Controller updates can proceed independently)

**After T016-T017 complete:**
- T018 ↔ T019 ↔ T020 (DI and validation setup independent)

**After T020 complete:**
- T021 ↔ T022 (Handler tests independent)
- T023 ↔ T024 (Integration tests independent)

**After T025 complete:**
- T026 ↔ T027 ↔ T028 ↔ T029 ↔ T030 (Cleanup and docs parallel)

---

## Parallel Execution Example

### Scenario: 2-Day Sprint with Parallel Tasks

**Day 1 - Morning (Sequential - Blocking)**
- Execute T001 (folder structure)
- Execute T002, T003 in parallel
- Execute T004, T005 in parallel (wait for T002-T005 to complete)

**Day 1 - Afternoon (Parallel)**
- Execute T006 ↔ T007 ↔ T008 ↔ T009 ↔ T010 ↔ T011 (6 developers, each takes 1 handler)
- Code review in parallel

**Day 2 - Morning (Parallel)**
- Execute T012 ↔ T013 ↔ T014 (3 developers create DTOs)
- Execute T015 (1 developer creates AutoMapper profile)

**Day 2 - Afternoon (Parallel)**
- Execute T016 ↔ T017 (2 developers update controllers)
- Execute T018 ↔ T019 ↔ T020 (DI setup)
- Execute T021 ↔ T022 (Handler tests)
- Execute T023 ↔ T024 (Integration tests)

**Day 2 - End**
- Execute T025 (validate all tests)
- Execute T026 through T030 (cleanup and documentation)

**Result**: Complete refactor in 2 days with 6 developers

---

## MVP Scope

**Minimum Viable Product** = Phases 1-4 (Recommended: Phases 1-5 for validation)

### MVP Includes:
- ✅ MediatR commands and handlers fully operational
- ✅ Controllers refactored to use mediator
- ✅ DI configuration complete
- ✅ All tests passing
- ✅ External API behavior unchanged

### Can Be Deferred (Phase 6):
- Documentation updates
- Code cleanup (non-critical)
- ADR creation
- Architecture review

**MVP Effort**: ~6-7 days with 3-4 developers  
**Full Effort**: ~8-10 days with 3-4 developers

---

## Validation Checklist

After all phases complete, verify:

- [ ] All commands defined in `Watcher.Domain/Commands/Request/`
- [ ] All command responses in `Watcher.Domain/Commands/Response/`
- [ ] All handlers in `Watcher.Domain/Handlers/` (5 minimum)
- [ ] All domain entities still in `Watcher.Domain/Entities/`
- [ ] Watcher.Api has **zero** references to domain entities
- [ ] Watcher.Api only references MediatR, AutoMapper, and DTOs
- [ ] All validators compile and are discovered by MediatR
- [ ] AutoMapper profiles are registered in Program.cs
- [ ] MediatR is registered to scan both assemblies
- [ ] Unit tests pass (T025)
- [ ] Integration tests pass (T025)
- [ ] Controllers don't inject services directly
- [ ] Controllers use IMediator for all business operations
- [ ] API response format unchanged from external perspective

---

## Quick Reference: File Checklist

### Watcher.Domain/Commands/Request/
- [ ] ClassifyCustomerCommand.cs
- [ ] GetAllJobCategoriesCommand.cs
- [ ] GetJobCategoryByIdCommand.cs
- [ ] CreateJobCategoryCommand.cs
- [ ] UpdateJobCategoryCommand.cs
- [ ] DeleteJobCategoryCommand.cs

### Watcher.Domain/Commands/Response/
- [ ] ClassifyCustomerResponse.cs
- [ ] GetAllJobCategoriesResponse.cs
- [ ] GetJobCategoryResponse.cs
- [ ] CreateJobCategoryResponse.cs
- [ ] UpdateJobCategoryResponse.cs

### Watcher.Domain/Handlers/
- [ ] ClassifyCustomerCommandHandler.cs
- [ ] GetAllJobCategoriesCommandHandler.cs
- [ ] GetJobCategoryByIdCommandHandler.cs
- [ ] CreateJobCategoryCommandHandler.cs
- [ ] UpdateJobCategoryCommandHandler.cs
- [ ] DeleteJobCategoryCommandHandler.cs

### Watcher.Domain/Validators/
- [ ] ClassifyCustomerCommandValidator.cs
- [ ] CreateJobCategoryCommandValidator.cs
- [ ] UpdateJobCategoryCommandValidator.cs
- [ ] GetJobCategoryByIdCommandValidator.cs

### Watcher.Api/Models/Requests/
- [ ] ClassifyCustomerRequest.cs
- [ ] CreateJobCategoryRequest.cs
- [ ] UpdateJobCategoryRequest.cs

### Watcher.Api/Models/Responses/
- [ ] ClassifyCustomerResponse.cs
- [ ] JobCategoryResponse.cs
- [ ] JobCategoriesListResponse.cs

### Watcher.Api/Mappings/
- [ ] CommandApiMappingProfile.cs

### Modified Files
- [ ] src/Watcher.Api/Program.cs (MediatR & validator registration)
- [ ] src/Watcher.Api/Controllers/ClassificationController.cs (MediatR injection)
- [ ] src/Watcher.Api/Controllers/JobCategoryController.cs (MediatR injection)
- [ ] tests/Watcher.Domain.UnitTests/Handlers/* (new handler tests)
- [ ] tests/Watcher.Api.IntegrationTests/* (updated controller tests)

---

## Implementation Tips

### For Each Handler (T006-T011):
1. Start with interface implementation
2. Inject required services in constructor
3. Implement Handle() method
4. Map domain model to response DTO
5. Add error handling for edge cases
6. Write unit tests before committing

### For Each Controller Update (T016-T017):
1. Change constructor injection from service to IMediator
2. Add IMapper injection for AutoMapper
3. For each action: create command → send via mediator → map response
4. Test endpoint with Postman/Swagger after update
5. Ensure backward compatibility in request/response

### For DI Configuration (T018):
1. Check existing MediatR registration (may already exist)
2. Verify handler assembly is scanned
3. Test with `dotnet build` and `dotnet run`
4. Check for circular dependencies in DI
5. Verify validators are auto-discovered

---

## Success Metrics

| Metric | Target | Validation |
|--------|--------|-----------|
| All tasks completed | 30/30 | Checklist 100% |
| Test pass rate | 100% | `dotnet test` output |
| API endpoints working | 100% | Swagger/Postman test |
| Domain entity leakage | 0 | Code review + semantic search |
| Code review approval | ✅ | Team sign-off |
| Performance (p50) | <100ms | Load test results |
| Architecture compliance | ✅ | Validation checklist |

---

## Related Documents

- [ARCHITECTURE_PLAN.md](ARCHITECTURE_PLAN.md) - High-level architecture design
- [REFACTOR_TASKS.md](REFACTOR_TASKS.md) - Initial rough task breakdown (superseded by this document)
- [specs/005-parametrize-job-category/plan.md](specs/005-parametrize-job-category/plan.md) - Feature context
- [specs/005-parametrize-job-category/spec.md](specs/005-parametrize-job-category/spec.md) - User stories being served
