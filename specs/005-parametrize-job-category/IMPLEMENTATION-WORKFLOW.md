# Implementation Workflow & Checklist

**Generated**: 2026-04-26 | **Reference**: [IMPLEMENTATION-PLAN.md](IMPLEMENTATION-PLAN.md) | **Status**: Ready for Execution

## Workflow Overview

This document provides a structured workflow for executing the mediator pattern refactoring implementation plan.

### Document Map

| Document | Purpose | Audience |
|---|---|---|
| [IMPLEMENTATION-PLAN.md](IMPLEMENTATION-PLAN.md) | **Comprehensive reference** - Complete architecture, code examples, testing strategy | Architects, Team Leads, Senior Developers |
| [IMPLEMENTATION-WORKFLOW.md](IMPLEMENTATION-WORKFLOW.md) | **This document** - Step-by-step execution workflow | Developers, QA |
| [quickstart.md](quickstart.md) | Quick reference - Sprint setup | Team Leads |

---

## Phase-Based Implementation Workflow

### Phase 1: Domain Layer Foundation

**Duration**: 2-3 days | **Owner**: Senior Developer

#### Step 1.1: Create Folder Structure

Execute in project root:

```bash
# Domain Command/Query structure
mkdir -p src/Watcher.Domain/Command/Requests/{Classification,JobCategory,ClusterConfiguration}
mkdir -p src/Watcher.Domain/Command/Responses
mkdir -p src/Watcher.Domain/Handlers/{Classification,JobCategory,ClusterConfiguration}

# Verification
ls -la src/Watcher.Domain/Command/
ls -la src/Watcher.Domain/Handlers/
```

**Expected Output:**
```
Command/
├── Requests/
│   ├── Classification/
│   ├── JobCategory/
│   └── ClusterConfiguration/
└── Responses/

Handlers/
├── Classification/
├── JobCategory/
└── ClusterConfiguration/
```

**Task Status**: [ ] Complete

#### Step 1.2: Create Response Classes

**Files to create**: 3 files in `src/Watcher.Domain/Command/Responses/`

**Reference**: IMPLEMENTATION-PLAN.md Section 3.3, 3.5, 3.7

| File | Classes | Status |
|---|---|---|
| `ClassifyCustomerResponse.cs` | ClassifyCustomerResponse, ClusterDto, JobCategoryDto | [ ] |
| `JobCategoryResponse.cs` | JobCategoryResponse | [ ] |
| `ClusterConfigurationResponse.cs` | ClusterConfigurationResponse | [ ] |

**Validation Checklist**:
- [ ] All files exist
- [ ] All classes have parameterless constructors
- [ ] All classes use auto-properties with `{ get; set; }`
- [ ] Build succeeds: `dotnet build`

**Task Status**: [ ] Complete

#### Step 1.3: Create Command/Query Classes

**Files to create**: 11 files in `src/Watcher.Domain/Command/Requests/`

**Reference**: IMPLEMENTATION-PLAN.md Section 3.2, 3.4, 3.6

| Folder | Files | Status |
|---|---|---|
| **Classification** | ClassifyCustomerCommand.cs | [ ] |
| **JobCategory** | CreateJobCategoryCommand.cs | [ ] |
| | UpdateJobCategoryCommand.cs | [ ] |
| | DeleteJobCategoryCommand.cs | [ ] |
| | GetJobCategoryByIdQuery.cs | [ ] |
| | GetAllJobCategoriesQuery.cs | [ ] |
| **ClusterConfiguration** | CreateClusterConfigurationCommand.cs | [ ] |
| | UpdateClusterConfigurationCommand.cs | [ ] |
| | DeleteClusterConfigurationCommand.cs | [ ] |
| | GetClusterConfigurationByIdQuery.cs | [ ] |
| | GetAllClusterConfigurationsQuery.cs | [ ] |

**Key Requirements**:
- ✅ All commands/queries implement `IRequest<ResponseType>`
- ✅ All classes have parameterless constructors
- ✅ Commands use properties with auto-initialization

**Validation**:
```bash
dotnet build
# Should output: Build succeeded. 0 Warning(s), 0 Error(s)
```

**Task Status**: [ ] Complete

#### Step 1.4: Update Program.cs with MediatR

**File**: `src/Watcher.Api/Program.cs`

**Reference**: IMPLEMENTATION-PLAN.md Section 6.1

**Changes Required**:

1. Add MediatR registration for API project
   ```csharp
   builder.Services.AddMediatR(cfg => 
       cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
   ```

2. Add MediatR registration for Domain project
   ```csharp
   builder.Services.AddMediatR(cfg => 
       cfg.RegisterServicesFromAssemblyContaining(
           typeof(Watcher.Domain.Entities.Customer).Assembly));
   ```

**Validation**:
```bash
dotnet build
# Should output: Build succeeded. 0 Warning(s), 0 Error(s)
```

**Task Status**: [ ] Complete

#### Phase 1 Completion Checklist

- [ ] All 14 command/query files created
- [ ] All 3 response files created
- [ ] Folder structure verified
- [ ] Program.cs updated with MediatR registration
- [ ] Build succeeds without warnings/errors
- [ ] No compilation errors in MediatR scanning

**Phase 1 Status**: [ ] COMPLETE ✅

---

### Phase 2: Handler Implementation

**Duration**: 3-4 days | **Owner**: Developer Team

#### Step 2.1: Implement ClassifyCustomerCommandHandler

**File**: `src/Watcher.Domain/Handlers/Classification/ClassifyCustomerCommandHandler.cs`

**Reference**: IMPLEMENTATION-PLAN.md Section 4.2

**Requirements**:
- Implements `IRequestHandler<ClassifyCustomerCommand, ClassifyCustomerResponse>`
- Has access to:
  - `IClusterConfigurationService`
  - `IJobCategoryService`
- Performs validation
- Builds Customer entity from command
- Evaluates cluster matching
- Calculates credit limit
- Returns ClassifyCustomerResponse

**Implementation Checklist**:
- [ ] Class created in correct folder
- [ ] Implements IRequestHandler interface correctly
- [ ] Injector configured for dependencies
- [ ] Handle method has correct signature
- [ ] Business logic copied from ClassificationService
- [ ] Response object properly constructed
- [ ] Compiles without errors
- [ ] Can be scanned by MediatR

**Validation**:
```bash
dotnet build
```

**Task Status**: [ ] Complete

#### Step 2.2: Implement Job Category Handlers

**Files**: 5 handlers in `src/Watcher.Domain/Handlers/JobCategory/`

**Reference**: IMPLEMENTATION-PLAN.md Section 4.3

| Handler | Request | Response | Notes |
|---|---|---|---|
| CreateJobCategoryCommandHandler | CreateJobCategoryCommand | JobCategoryResponse | Creates new category |
| UpdateJobCategoryCommandHandler | UpdateJobCategoryCommand | JobCategoryResponse | Updates existing |
| DeleteJobCategoryCommandHandler | DeleteJobCategoryCommand | Unit | Returns nothing |
| GetJobCategoryByIdQueryHandler | GetJobCategoryByIdQuery | JobCategoryResponse? | May return null |
| GetAllJobCategoriesQueryHandler | GetAllJobCategoriesQuery | IEnumerable<JobCategoryResponse> | Returns all |

**Implementation Pattern**:
```csharp
public class CreateJobCategoryCommandHandler : IRequestHandler<CreateJobCategoryCommand, JobCategoryResponse>
{
    private readonly IJobCategoryService _service;
    
    public CreateJobCategoryCommandHandler(IJobCategoryService service) => _service = service;
    
    public async Task<JobCategoryResponse> Handle(CreateJobCategoryCommand request, CancellationToken cancellationToken)
    {
        // 1. Create domain entity from command
        // 2. Call service to persist
        // 3. Map entity to response
        // 4. Return response
    }
}
```

**Implementation Checklist** (for each handler):
- [ ] File created
- [ ] Correct interface implemented
- [ ] Service injected via constructor
- [ ] Handle method implemented
- [ ] Entity mapping to response correct
- [ ] Compiles without errors
- [ ] Follows naming conventions

**Validation**:
```bash
dotnet build
```

**Task Status**: [ ] Complete

#### Step 2.3: Implement ClusterConfiguration Handlers

**Files**: 5 handlers in `src/Watcher.Domain/Handlers/ClusterConfiguration/`

**Reference**: IMPLEMENTATION-PLAN.md Section 4.4

Follow same pattern as Job Category handlers using `IClusterConfigurationService`.

**Implementation Checklist** (for each handler):
- [ ] File created
- [ ] Correct interface implemented
- [ ] Service injected via constructor
- [ ] Handle method implemented
- [ ] Entity mapping to response correct
- [ ] Compiles without errors
- [ ] Follows naming conventions

**Validation**:
```bash
dotnet build
dotnet test
```

**Task Status**: [ ] Complete

#### Phase 2 Completion Checklist

- [ ] 1 classification handler implemented
- [ ] 5 job category handlers implemented
- [ ] 5 cluster configuration handlers implemented
- [ ] All 11 handlers compile without errors
- [ ] All handlers follow naming conventions
- [ ] All handlers properly map entities to responses
- [ ] MediatR can scan and discover all handlers

**Phase 2 Status**: [ ] COMPLETE ✅

---

### Phase 3: Controller Refactoring

**Duration**: 2-3 days | **Owner**: API Developer

#### Step 3.1: Update AutoMapper Profiles

**Files to update/create**: `src/Watcher.Api/Mappings/`

**Reference**: IMPLEMENTATION-PLAN.md Section 7.2

| File | Responsibility |
|---|---|
| CustomerMappingProfile.cs | CustomerRequest ↔ ClassifyCustomerCommand / Response |
| JobCategoryMappingProfile.cs | JobCategoryRequest ↔ Job Category Commands |
| ClusterConfigurationMappingProfile.cs | ClusterConfigurationRequest ↔ Cluster Commands |

**Key Mappings**:

**CustomerMappingProfile**:
```csharp
CreateMap<CustomerRequest, ClassifyCustomerCommand>()
    .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.Location, opt => opt.MapFrom(src => new LocationDto { ... }));

CreateMap<ClassifyCustomerResponse, CustomerResponse>();
```

**JobCategoryMappingProfile**:
```csharp
CreateMap<JobCategoryRequest, CreateJobCategoryCommand>();
CreateMap<JobCategoryRequest, UpdateJobCategoryCommand>();
CreateMap<JobCategoryResponse, JobCategoryResponse>();
```

**ClusterConfigurationMappingProfile**:
```csharp
CreateMap<ClusterConfigurationRequest, CreateClusterConfigurationCommand>();
CreateMap<ClusterConfigurationRequest, UpdateClusterConfigurationCommand>();
CreateMap<ClusterConfigurationResponse, ClusterConfigurationResponse>();
```

**Implementation Checklist**:
- [ ] CustomerMappingProfile updated with command mappings
- [ ] JobCategoryMappingProfile created/updated
- [ ] ClusterConfigurationMappingProfile created/updated
- [ ] All bidirectional mappings defined
- [ ] Compiles without errors
- [ ] No mapping exceptions at runtime

**Validation**:
```bash
dotnet build
```

**Task Status**: [ ] Complete

#### Step 3.2: Refactor CustomerController

**File**: `src/Watcher.Api/Controllers/CustomerController.cs`

**Reference**: IMPLEMENTATION-PLAN.md Section 5.2

**Changes Required**:

1. **Dependency Injection**
   ```csharp
   // BEFORE
   private readonly IClassificationService _classificationService;
   
   // AFTER
   private readonly IMediator _mediator;
   private readonly IMapper _mapper;
   ```

2. **Constructor**
   ```csharp
   public CustomerController(IMediator mediator, IMapper mapper, RequestMetrics metrics)
   {
       _mediator = mediator;
       _mapper = mapper;
       _metrics = metrics;
   }
   ```

3. **Classify Action**
   ```csharp
   [HttpPost("classify")]
   public async Task<ActionResult<CustomerResponse>> Classify([FromBody] CustomerRequest request)
   {
       var command = _mapper.Map<ClassifyCustomerCommand>(request);
       var response = await _mediator.Send(command);
       var apiResponse = _mapper.Map<CustomerResponse>(response);
       _metrics.ClassificationRequests++;
       return Ok(apiResponse);
   }
   ```

**Implementation Checklist**:
- [ ] Removed IClassificationService injection
- [ ] Added IMediator injection
- [ ] Removed direct Customer entity creation
- [ ] Added command mapping
- [ ] Added mediator dispatch
- [ ] Added response mapping
- [ ] No direct service calls
- [ ] Compiles without errors

**Validation**:
```bash
dotnet build
```

**Task Status**: [ ] Complete

#### Step 3.3: Refactor JobCategoryController

**File**: `src/Watcher.Api/Controllers/JobCategoryController.cs`

**Reference**: IMPLEMENTATION-PLAN.md Section 5.3

**Changes Required**:

| Method | Old | New |
|---|---|---|
| GetAll() | `_service.GetAllAsync()` | `_mediator.Send(new GetAllJobCategoriesQuery())` |
| GetById(id) | `_service.GetByIdAsync(id)` | `_mediator.Send(new GetJobCategoryByIdQuery { Id = id })` |
| Create(request) | `_service.CreateAsync()` | `_mediator.Send(_mapper.Map<CreateJobCategoryCommand>(request))` |
| Update(id, request) | `_service.UpdateAsync()` | `_mediator.Send(_mapper.Map<UpdateJobCategoryCommand>(request))` |
| Delete(id) | `_service.DeleteAsync(id)` | `_mediator.Send(new DeleteJobCategoryCommand { Id = id })` |

**Implementation Checklist**:
- [ ] Removed IJobCategoryService injection
- [ ] Added IMediator injection
- [ ] All 5 endpoints updated
- [ ] All queries/commands created and mapped
- [ ] All responses mapped back to DTOs
- [ ] Error handling preserved
- [ ] Compiles without errors

**Validation**:
```bash
dotnet build
```

**Task Status**: [ ] Complete

#### Step 3.4: Refactor ClusterController

**File**: `src/Watcher.Api/Controllers/ClusterController.cs`

**Reference**: IMPLEMENTATION-PLAN.md Section 5.4

Follow same pattern as JobCategoryController:

**Implementation Checklist**:
- [ ] Removed IClusterConfigurationService injection
- [ ] Added IMediator injection
- [ ] All 5 endpoints updated
- [ ] All queries/commands created and mapped
- [ ] All responses mapped back to DTOs
- [ ] Error handling preserved (KeyNotFoundException handling)
- [ ] Compiles without errors

**Validation**:
```bash
dotnet build
```

**Task Status**: [ ] Complete

#### Phase 3 Completion Checklist

- [ ] AutoMapper profiles updated
- [ ] CustomerController refactored
- [ ] JobCategoryController refactored
- [ ] ClusterController refactored
- [ ] All controllers only use IMediator (no services)
- [ ] All command/query dispatching correct
- [ ] All response mappings correct
- [ ] Build succeeds without warnings
- [ ] No direct entity references in controllers

**Phase 3 Status**: [ ] COMPLETE ✅

---

### Phase 4: Testing Implementation

**Duration**: 2-3 days | **Owner**: QA + Developers

#### Step 4.1: Create Handler Unit Tests

**Location**: `tests/Watcher.Domain.UnitTests/Handlers/`

**Reference**: IMPLEMENTATION-PLAN.md Section 8.3

| Test Class | Handler Tested | Test Cases |
|---|---|---|
| ClassifyCustomerCommandHandlerTests | ClassifyCustomerCommandHandler | Valid request, Invalid score, Invalid age |
| CreateJobCategoryCommandHandlerTests | CreateJobCategoryCommandHandler | Valid create, Proper mapping |
| GetAllJobCategoriesQueryHandlerTests | GetAllJobCategoriesQueryHandler | Returns all categories |
| CreateClusterConfigurationCommandHandlerTests | CreateClusterConfigurationCommandHandler | Valid create, Proper mapping |
| GetAllClusterConfigurationsQueryHandlerTests | GetAllClusterConfigurationsQueryHandler | Returns all configurations |

**Test Template**:
```csharp
[TestClass]
public class HandlerNameTests
{
    private Mock<IServiceInterface> _mockService;
    private HandlerName _handler;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IServiceInterface>();
        _handler = new HandlerName(_mockService.Object);
    }

    [TestMethod]
    public async Task Handle_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var request = new RequestClass { /* properties */ };
        _mockService.Setup(x => x.Method()).ReturnsAsync(/* result */);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expected, result.Property);
    }
}
```

**Implementation Checklist**:
- [ ] Test class for each handler
- [ ] Happy path test case
- [ ] Error/validation test case
- [ ] Mocked dependencies
- [ ] Uses Moq for mocking
- [ ] Assertions verify behavior
- [ ] All tests pass: `dotnet test --filter "Handlers"`

**Task Status**: [ ] Complete

#### Step 4.2: Create Integration Tests

**Location**: `tests/Watcher.Api.IntegrationTests/Endpoints/`

**Reference**: IMPLEMENTATION-PLAN.md Section 8.4

| Test Class | Endpoint Tested | Test Cases |
|---|---|---|
| ClassifyCustomerEndpointTests | POST /customers/classify | Valid request (200), Invalid score (400) |
| JobCategoryEndpointTests | Job Category CRUD | GetAll (200), GetById (200/404), Create (201), Update (200), Delete (204) |
| ClusterConfigurationEndpointTests | Cluster Configuration CRUD | GetAll (200), GetById (200/404), Create (201), Update (200), Delete (204) |

**Test Template**:
```csharp
[TestClass]
public class EndpointTests
{
    private HttpClient _client;
    private WebApplicationFactory<Program> _factory;

    [TestInitialize]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task Post_Endpoint_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new { /* properties */ };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/endpoint", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
```

**Implementation Checklist**:
- [ ] Integration test for each endpoint
- [ ] Happy path test case (200 response)
- [ ] Error case test case (400/404 response)
- [ ] Uses WebApplicationFactory
- [ ] Serializes/deserializes JSON correctly
- [ ] Verifies status codes
- [ ] All tests pass: `dotnet test --filter "Integration"`

**Task Status**: [ ] Complete

#### Step 4.3: Verify Test Coverage

**Commands**:
```bash
# Run all tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura

# Run specific test categories
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"

# Detailed coverage report
dotnet test /p:CollectCoverage=true /p:CoverageFormat=lcov
```

**Coverage Goals**:
- Handlers: >= 90% coverage
- Controllers: >= 80% coverage
- Overall: >= 80% coverage

**Implementation Checklist**:
- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] Code coverage >= 80%
- [ ] Handlers coverage >= 90%
- [ ] No code coverage gaps
- [ ] Build succeeds

**Task Status**: [ ] Complete

#### Phase 4 Completion Checklist

- [ ] Handler unit tests created (5 test classes)
- [ ] Integration tests created (3 test classes)
- [ ] All tests pass
- [ ] Code coverage >= 80%
- [ ] Handler coverage >= 90%
- [ ] No coverage gaps
- [ ] Test classes follow naming conventions
- [ ] Tests use proper mocking patterns

**Phase 4 Status**: [ ] COMPLETE ✅

---

### Phase 5: Final Validation

**Duration**: 1-2 days | **Owner**: QA + DevOps

#### Step 5.1: Comprehensive Build & Test

```bash
# Clean build
dotnet clean
dotnet build

# Test output
dotnet test

# Coverage verification
dotnet test /p:CollectCoverage=true
```

**Validation Checklist**:
- [ ] Build succeeds (0 warnings)
- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] Code coverage >= 80%
- [ ] No code analysis warnings

**Task Status**: [ ] Complete

#### Step 5.2: Manual API Testing

**Start the application**:
```bash
dotnet run --project src/Watcher.Api/Watcher.Api.csproj
```

**Test Cases**:

1. Health Endpoint
   ```bash
   curl http://localhost:5000/health
   # Expected: 200 OK, { "status": "Healthy" }
   ```

2. Classification Endpoint
   ```bash
   curl -X POST http://localhost:5000/customers/classify \
     -H "Content-Type: application/json" \
     -d '{
       "id": "TEST001",
       "name": "John Doe",
       "score": 750,
       "age": 35,
       "hasMarketDebt": false,
       "marketDebtTypes": [],
       "location": {"country": "US", "state": "CA", "city": "SF"},
       "jobTitle": "Engineer"
     }'
   # Expected: 200 OK with classification result
   ```

3. Job Categories
   ```bash
   curl http://localhost:5000/api/job-categories
   # Expected: 200 OK with list of categories
   ```

4. Clusters
   ```bash
   curl http://localhost:5000/clusters
   # Expected: 200 OK with list of clusters
   ```

**Test Checklist**:
- [ ] Health endpoint returns 200
- [ ] Classification produces results
- [ ] Job category list returns data
- [ ] Cluster list returns data
- [ ] No console errors
- [ ] Metrics endpoint works

**Task Status**: [ ] Complete

#### Step 5.3: Code Review Validation

**Review Checklist**:

**Architecture**:
- [ ] No domain entities in API responses
- [ ] API layer only uses IMediator
- [ ] All business logic in handlers
- [ ] Clean separation of concerns
- [ ] Proper layering maintained

**Code Quality**:
- [ ] Naming conventions consistent
- [ ] Comments on complex logic
- [ ] No code duplication
- [ ] No unused imports
- [ ] Proper error handling

**Testing**:
- [ ] Tests follow patterns
- [ ] Mocking done correctly
- [ ] Edge cases covered
- [ ] Assertions are meaningful
- [ ] No flaky tests

**Documentation**:
- [ ] Code is self-documenting
- [ ] XML docs where appropriate
- [ ] Architecture documented
- [ ] Testing strategy documented

**Task Status**: [ ] Complete

#### Phase 5 Completion Checklist

- [ ] Build succeeds (0 warnings, 0 errors)
- [ ] All tests pass (100%)
- [ ] Code coverage >= 80%
- [ ] Manual testing passes
- [ ] Code review passed
- [ ] Architecture validated
- [ ] Documentation complete
- [ ] Ready for merge

**Phase 5 Status**: [ ] COMPLETE ✅

---

## Git Workflow

### Branch Strategy

```bash
# Create feature branch
git checkout -b feat/mediator-refactor

# Make changes, commit frequently
git add <files>
git commit -m "feat: specific change"

# When ready, push to remote
git push origin feat/mediator-refactor

# Create Pull Request on GitHub
```

### Commit Message Pattern

```
feat: add mediator command/query infrastructure
fix: update mapping configuration
refactor: migrate controller to mediator
test: add comprehensive handler tests
```

### PR Checklist

- [ ] Branch created from main
- [ ] All commits have clear messages
- [ ] Build passes
- [ ] All tests pass
- [ ] Code coverage maintained
- [ ] Code review approved
- [ ] Ready to merge

---

## Troubleshooting Guide

### Issue: "MediatR handlers not found"

**Solution**:
```csharp
// Ensure Program.cs has both registrations:
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
    
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssemblyContaining(
        typeof(Watcher.Domain.Entities.Customer).Assembly));
```

### Issue: "AutoMapper mapping not found"

**Solution**:
```bash
# Rebuild AutoMapper profiles
dotnet build
dotnet test
```

### Issue: "Tests fail with 'handler not registered'"

**Solution**:
1. Verify handler implements `IRequestHandler<>`
2. Ensure MediatR assembly scanning includes handler assembly
3. Check handler namespace matches filter

### Issue: "Controllers still reference services"

**Solution**:
```bash
# Search for remaining service references
grep -r "IClassificationService" src/Watcher.Api/Controllers/
grep -r "IJobCategoryService" src/Watcher.Api/Controllers/
grep -r "IClusterConfigurationService" src/Watcher.Api/Controllers/

# Should return empty
```

---

## Success Metrics

| Metric | Target | Status |
|---|---|---|
| Build Success | 0 warnings, 0 errors | [ ] |
| Test Pass Rate | 100% | [ ] |
| Code Coverage | >= 80% | [ ] |
| Handler Coverage | >= 90% | [ ] |
| Controller Integration | All endpoints working | [ ] |
| No Regressions | API returns same results | [ ] |

---

## Sign-Off

### Team Lead Approval

- [ ] Architecture review passed
- [ ] Code review completed
- [ ] Tests verified
- [ ] Deployment ready

**Date**: ____________
**Approved By**: ____________

### QA Verification

- [ ] Manual testing passed
- [ ] Integration tests passed
- [ ] No regressions found
- [ ] Performance acceptable

**Date**: ____________
**Verified By**: ____________

### Deployment Ready

- [ ] All phases completed
- [ ] All validations passed
- [ ] Documentation complete
- [ ] Team trained

**Date**: ____________
**Status**: [ ] READY FOR PRODUCTION

---

**End of Implementation Workflow Document**

