# Quickstart: Priority-based job category classification

## Goal
Implement job category priority ordering for customer classification while keeping the existing no-database mock repository.

## Run the application

From the repository root:

```bash
cd /Users/leonardosouza/dev/study-sdd/first-project
dotnet run --project src/Watcher.Api/Watcher.Api.csproj
```

## What changed

- `JobCategory` now includes a `Priority` field.
- `JobCategoryService.IdentifyCategoryAsync` orders categories by priority before matching keywords.
- The final category chosen is the last one found in ordered validation.
- No database connection is required; the feature uses the existing in-memory mock repository.

## Example usage

### Request

```http
POST /customers/classify
Content-Type: application/json

{
  "customerId": "123",
  "score": 620,
  "age": 35,
  "hasMarketDebt": false,
  "marketDebtTypes": [],
  "jobTitle": "Senior Software Engineer"
}
```

### Expected response

```json
{
  "customerId": "123",
  "clusterId": "CLUSTER_B",
  "clusterName": "Stable",
  "jobCategoryId": "SENIOR_PROFESSIONAL",
  "jobCategoryName": "Senior Professional",
  "creditLimit": 750.00,
  "calculatedAt": "2026-04-26T12:00:00Z"
}
```

## Verify with tests

```bash
dotnet build
dotnet test
```

## Notes for developers

- The feature is implemented in the domain and infrastructure layers.
- Priority ordering is intentionally separated from cluster logic.
- Mock categories are updated in `src/Watcher.Infrastructure/Mocks/JobCategoryRepository.cs`.
