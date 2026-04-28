# Risk Watcher

Risk Watcher is a .NET Core API for classifying customers into risk clusters based on financial and market data.

## Architecture

This application follows Clean Architecture principles with MediatR mediator pattern for CQRS implementation:

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Watcher.Api   │───▶│  Watcher.Domain │───▶│   Services      │
│                 │    │                 │    │                 │
│ • Controllers   │    │ • Commands      │    │ • Classification│
│ • DTOs          │    │ • Handlers      │    │ • Job Categories│
│ • AutoMapper    │    │ • Validators    │    │ • Repositories  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   HTTP Request  │───▶│   MediatR       │───▶│   Business      │
│                 │    │   Commands      │    │   Logic         │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

### Key Principles
- **Clean Architecture**: API layer knows nothing about domain entities
- **CQRS**: Commands for writes, queries for reads via MediatR
- **Dependency Injection**: All dependencies injected through DI container
- **Validation**: FluentValidation for input validation
- **Testing**: Comprehensive unit and integration tests

## Projects

- `src/Watcher.Api/`: ASP.NET Core Web API project (HTTP interface)
- `src/Watcher.Domain/`: Business domain with commands, handlers, and validators
- `src/Watcher.Infrastructure/`: In-memory mock repository implementation
- `tests/Watcher.Domain.UnitTests/`: Domain unit tests
- `tests/Watcher.Api.IntegrationTests/`: API integration tests

## Running the API

```bash
dotnet run --project src/Watcher.Api
```

The API is available at:
- `https://localhost:5001`
- `http://localhost:5000`

Swagger UI:
- `https://localhost:5001/swagger`

## API Endpoints

### Customer Classification

#### POST /customers/classify

Classify a customer into a risk cluster.

**Request body:**

```json
{
  "customerId": "12345",
  "score": 750,
  "age": 35,
  "hasMarketDebt": true,
  "marketDebtTypes": ["credit_card"],
  "jobTitle": "Software Engineer"
}
```

**Response body:**

```json
{
  "customerId": "12345",
  "clusterId": "CLUSTER_B",
  "clusterName": "Gold",
  "jobCategoryId": "TECH",
  "jobCategoryName": "Technology",
  "creditLimit": 25000.00,
  "calculatedAt": "2024-01-15T10:30:00Z"
}
```

### Job Categories

#### GET /api/job-categories

Get all job categories.

**Response body:**

```json
{
  "jobCategories": [
    {
      "id": "EXECUTIVE",
      "name": "Executive",
      "multiplier": 2.0,
      "keywords": ["CEO", "CFO", "director"]
    }
  ]
}
```

#### GET /api/job-categories/{id}

Get a specific job category by ID.

#### POST /api/job-categories

Create a new job category.

**Request body:**

```json
{
  "name": "Technology",
  "multiplier": 1.5,
  "keywords": ["engineer", "developer", "tech"]
}
```

#### PUT /api/job-categories/{id}

Update an existing job category.

#### DELETE /api/job-categories/{id}

Delete a job category.

## Health and Metrics

- `GET /health`
- `GET /metrics`

## Adding New Features

To add new functionality with the MediatR pattern:

1. **Create Command/Response classes** in `src/Watcher.Domain/Commands/`
2. **Implement Handler** in `src/Watcher.Domain/Handlers/`
3. **Add Validator** in `src/Watcher.Domain/Validators/` (optional)
4. **Create API DTOs** in `src/Watcher.Api/Models/`
5. **Add AutoMapper mappings** in `src/Watcher.Api/Mappings/CommandApiMappingProfile.cs`
6. **Update Controller** to send commands via MediatR
7. **Add Tests** for handler and integration

## Tests

Run all tests:

```bash
dotnet test
```

Run specific test projects:

```bash
dotnet test tests/Watcher.Domain.UnitTests/
dotnet test tests/Watcher.Api.IntegrationTests/
```
