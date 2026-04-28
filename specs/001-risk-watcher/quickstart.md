# Quickstart: Risk Watcher API

**Feature**: Risk Watcher | **Date**: 2026-04-23

## Prerequisites

- .NET 8 SDK installed
- Git (for cloning if needed)

## Setup

1. Clone or navigate to the project repository
2. Navigate to the solution directory
3. Restore dependencies:
   ```bash
   dotnet restore
   ```

## Running the API

1. Start the API:
   ```bash
   dotnet run --project src/Watcher.Api
   ```

2. The API will start on `https://localhost:5001` (HTTPS) and `http://localhost:5000` (HTTP)

3. Swagger UI available at: `https://localhost:5001/swagger`

## Testing the API

Use the following curl command to test the classification endpoint:

```bash
curl -X POST "https://localhost:5001/customers/classify" \
     -H "Content-Type: application/json" \
     -d '{
       "id": "12345",
       "name": "John Doe",
       "age": 35,
       "score": 750,
       "has_market_debt": true,
       "market_debt_types": ["credit_card"],
       "location": {
         "city": "São Paulo",
         "state": "SP",
         "region": "Sudeste"
       },
       "job_title": "Software Engineer"
     }'
```

Expected response:
```json
{
  "id": "12345",
  "name": "John Doe",
  "age": 35,
  "score": 750,
  "has_market_debt": true,
  "market_debt_types": ["credit_card"],
  "location": {
    "city": "São Paulo",
    "state": "SP",
    "region": "Sudeste"
  },
  "job_title": "Software Engineer",
  "cluster": "CLUSTER_B"
}
```

## Running Tests

Execute unit tests:
```bash
dotnet test
```

## Development

- API project: `src/Watcher.Api`
- Domain logic: `src/Watcher.Domain`
- Infrastructure (mocks): `src/Watcher.Infrastructure`
- Tests: `tests/` directory

## Configuration

The API uses `appsettings.json` for configuration. Key settings:
- `Kestrel` endpoints
- Logging levels
- Swagger configuration