# Architecture Refactor: Mediator Pattern Implementation

## Objective
Restructure the application to use MediatR (Mediator Pattern) with clean separation of concerns, ensuring Watcher.Api has no knowledge of domain business rules or entities.

## Current State Analysis
- Direct service-based architecture with ClassificationService and JobCategoryService
- Services injected into controllers
- Controllers handle business logic orchestration
- Mixed responsibilities between API and domain layers

## Target Architecture

### 1. **Watcher.Domain** (Business Logic & Commands)
Houses all business rules, entities, handlers, and commands.

#### Structure:
```
src/Watcher.Domain/
├── Entities/                 (Domain models)
│   ├── JobCategory.cs
│   ├── Cluster.cs
│   ├── Customer.cs
│   ├── Location.cs
│   └── ClassificationResult.cs
├── Commands/
│   ├── Request/              (Command request DTOs)
│   │   ├── ClassifyCustomerCommand.cs
│   │   ├── CreateJobCategoryCommand.cs
│   │   ├── UpdateJobCategoryCommand.cs
│   │   ├── DeleteJobCategoryCommand.cs
│   │   └── GetJobCategoryCommand.cs
│   └── Response/             (Command response DTOs)
│       ├── ClassifyCustomerResponse.cs
│       ├── CreateJobCategoryResponse.cs
│       ├── UpdateJobCategoryResponse.cs
│       └── GetJobCategoryResponse.cs
├── Handlers/                 (MediatR handlers)
│   ├── ClassifyCustomerCommandHandler.cs
│   ├── CreateJobCategoryCommandHandler.cs
│   ├── UpdateJobCategoryCommandHandler.cs
│   ├── DeleteJobCategoryCommandHandler.cs
│   └── GetJobCategoryCommandHandler.cs
├── Interfaces/               (Repository & Service contracts)
│   ├── IJobCategoryRepository.cs
│   ├── IJobCategoryService.cs
│   ├── IClusterConfigurationRepository.cs
│   ├── IClusterConfigurationService.cs
│   ├── ICustomerRepository.cs
│   └── IClassificationService.cs
├── Services/                 (Business logic services)
│   ├── JobCategoryService.cs
│   ├── ClassificationService.cs
│   └── ClusterConfigurationService.cs
├── Mappers/                  (Domain object mapping)
│   └── ClassificationResultMapper.cs
├── Mappings/                 (AutoMapper profiles for internal mapping)
│   └── CommandMappingProfile.cs
└── Validations/              (Fluent validation rules)
    └── CommandValidators.cs
```

### 2. **Watcher.Api** (HTTP Interface Only)
Thin API layer - only controllers, validation, and DTOs.

#### Structure:
```
src/Watcher.Api/
├── Controllers/              (HTTP endpoints)
│   ├── ClassificationController.cs
│   └── JobCategoryController.cs
├── Models/                   (API DTOs - NOT domain entities)
│   ├── Requests/
│   │   ├── ClassifyCustomerRequest.cs
│   │   ├── CreateJobCategoryRequest.cs
│   │   ├── UpdateJobCategoryRequest.cs
│   │   └── GetJobCategoryRequest.cs
│   └── Responses/
│       ├── ClassifyCustomerResponse.cs
│       ├── JobCategoryResponse.cs
│       └── ClusterResponse.cs
├── Validators/               (Input validation)
│   ├── ClassifyCustomerRequestValidator.cs
│   └── CreateJobCategoryRequestValidator.cs
├── Mappings/                 (Map API models to/from Commands)
│   └── CommandApiMappingProfile.cs
└── Program.cs                (DI setup with MediatR)
```

### 3. **Watcher.Infrastructure** (Data Access)
In-memory repositories.

```
src/Watcher.Infrastructure/
└── Mocks/
    ├── JobCategoryRepository.cs
    ├── ClusterConfigurationRepository.cs
    └── CustomerRepository.cs
```

## Data Flow

### Classification Flow:
```
API Request
    ↓
ClassificationController.Classify()
    ↓
Validate (FluentValidation)
    ↓
Map API Model → ClassifyCustomerCommand (Request)
    ↓
_mediator.Send(command)
    ↓
ClassifyCustomerCommandHandler.Handle()
    ↓
Call IClassificationService.ClassifyAsync()
    ↓
Return ClassifyCustomerResponse
    ↓
Map Response → API ResponseModel
    ↓
Return 200 OK
```

### Job Category Management Flow:
```
API Request
    ↓
JobCategoryController (GetAll|Create|Update|Delete)
    ↓
Validate (FluentValidation)
    ↓
Map API Model → Command (Create|Update|Delete|Get)JobCategoryCommand
    ↓
_mediator.Send(command)
    ↓
Handler.Handle()
    ↓
Call IJobCategoryService method
    ↓
Return Response
    ↓
Map Response → API ResponseModel
    ↓
Return 200 OK / 201 Created / 204 NoContent
```

## Command/Handler Pairs

### 1. Classification
- **Command**: `ClassifyCustomerCommand` (Request: customer data)
- **Response**: `ClassifyCustomerResponse` (Result: cluster, job category, credit limit)
- **Handler**: `ClassifyCustomerCommandHandler`

### 2. Job Category CRUD
- **Commands**:
  - `GetAllJobCategoriesCommand` → `GetAllJobCategoriesResponse`
  - `GetJobCategoryByIdCommand` → `GetJobCategoryResponse`
  - `CreateJobCategoryCommand` → `CreateJobCategoryResponse`
  - `UpdateJobCategoryCommand` → `UpdateJobCategoryResponse`
  - `DeleteJobCategoryCommand` → No response (success)
- **Handlers**: Corresponding handlers for each command

## Key Architectural Principles

### 1. **Separation of Concerns**
- **Watcher.Api**: HTTP protocol handling, request/response mapping, validation
- **Watcher.Domain**: Business logic, commands, handlers, entities
- **Watcher.Infrastructure**: Data persistence (in-memory)

### 2. **Dependency Direction**
```
Watcher.Api 
    ↓
Watcher.Domain (IMediator)
    ├→ Commands & Responses
    ├→ Handlers (business logic)
    ├→ Entities
    ├→ Services
    └→ Interfaces
    
Watcher.Infrastructure
    ↓
Watcher.Domain (IRepository interfaces)
```

### 3. **API Layer Constraints**
- ✅ Define controllers
- ✅ Define API request/response DTOs
- ✅ Validate input contracts
- ✅ Map API models to commands
- ✅ Call MediatR
- ❌ Know about domain entities
- ❌ Know about business rules
- ❌ Know about repositories
- ❌ Know about services

### 4. **Dependency Injection Setup**
```csharp
// Program.cs
builder.Services.AddMediatR(typeof(Watcher.Domain.Entities.Customer).Assembly);
builder.Services.AddAutoMapper(typeof(CommandApiMappingProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation();

// Repositories
builder.Services.AddSingleton<IJobCategoryRepository, JobCategoryRepository>();
builder.Services.AddSingleton<IClusterConfigurationRepository, ClusterConfigurationRepository>();

// Services
builder.Services.AddScoped<IJobCategoryService, JobCategoryService>();
builder.Services.AddScoped<IClassificationService, ClassificationService>();
```

## Implementation Steps

1. **Create Command/Request structure** in Watcher.Domain/Commands/Request/
2. **Create Command/Response structure** in Watcher.Domain/Commands/Response/
3. **Implement Handlers** in Watcher.Domain/Handlers/
4. **Create API DTOs** in Watcher.Api/Models/
5. **Create AutoMapper profile** for API → Command → Response mapping
6. **Update controllers** to use MediatR instead of direct service calls
7. **Update Program.cs** with MediatR registration
8. **Create comprehensive tests** for handlers and controllers
9. **Update validation** in Watcher.Api layer

## Testing Strategy

- **Unit Tests for Handlers**: Test command handling logic
- **Unit Tests for Services**: Test business logic in isolation
- **Integration Tests for Controllers**: Test full HTTP flow
- **Validation Tests**: Test input validation rules

## Benefits

1. **Clean Architecture**: Clear separation between API and business logic
2. **Testability**: Handlers can be tested independently
3. **Maintainability**: Single responsibility principle
4. **Scalability**: Easy to add new commands without changing API structure
5. **Decoupling**: API layer decoupled from domain changes
