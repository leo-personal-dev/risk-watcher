# Quickstart: Parametrize Cluster Definition

## Goal

Create, update, remove, and evaluate user-defined cluster configurations using in-memory mocked storage. Debt rules are C# boolean expressions evaluated against `Customer` input.

## Setup

1. Open the solution in Visual Studio or VS Code.
2. Ensure the `src/Watcher.Api` project is selected as the startup project.
3. Run the API using `dotnet run --project src/Watcher.Api`.

## Workflow

### 1. Define a cluster configuration

Create a new cluster payload:

```json
{
  "id": "CLUSTER_X",
  "name": "Custom",
  "scoreMin": 600,
  "scoreMax": 800,
  "ageMin": 25,
  "ageMax": 65,
  "debtRule": "!customer.HasMarketDebt || !customer.MarketDebtTypes.Contains(\"credit_default\")",
  "baseLimit": 30000,
  "cap": 60000
}
```

POST this payload to `/clusters`.

### 2. Update a cluster configuration

PUT an updated payload to `/clusters/{id}` with the modified fields.

### 3. Remove a cluster configuration

DELETE `/clusters/{id}` to remove a configuration from the mock store.

### 4. Classify a customer

POST a customer payload to `/customers/classify` and the API will evaluate configured clusters in-memory.

Example customer request:

```json
{
  "id": "123",
  "name": "Jane Doe",
  "age": 35,
  "score": 720,
  "hasMarketDebt": false,
  "marketDebtTypes": ["credit_card"],
  "location": { "city": "São Paulo", "state": "SP", "region": "Sudeste" },
  "jobTitle": "Analyst"
}
```

### 5. Verify debt rule evaluation

With `debtRule` expressed in C# syntax, the classification engine can evaluate expressions like:

- `!customer.HasMarketDebt`
- `customer.MarketDebtTypes.Contains("credit_default")`
- `customer.Age >= 30 && !customer.MarketDebtTypes.Any()`

## Notes

- The implementation uses mocked cluster data instead of a database.
- Rules are evaluated against the customer object and must be valid C# boolean expressions.
- The first matching configuration in the mock list determines the assigned cluster.
