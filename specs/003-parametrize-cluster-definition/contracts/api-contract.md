# API Contract: Parametrize Cluster Definition

## Cluster Configuration Endpoints

### Create Cluster

POST /clusters

Request:
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

Response: 201 Created
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

### Update Cluster

PUT /clusters/{id}

Request: same as create payload

Response: 200 OK with updated cluster payload

### Delete Cluster

DELETE /clusters/{id}

Response: 204 No Content

### Get All Clusters

GET /clusters

Response: 200 OK
```json
[
  {
    "id": "CLUSTER_A",
    "name": "Diamond",
    "scoreMin": 700,
    "scoreMax": 1000,
    "ageMin": 25,
    "ageMax": 60,
    "debtRule": "!customer.HasMarketDebt",
    "baseLimit": 50000,
    "cap": 100000
  }
]
```

## Customer Classification Endpoint

### Classify Customer

POST /customers/classify

Request:
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

Response: 200 OK
```json
{
  "customerId": "123",
  "cluster": {
    "idCluster": "CLUSTER_X",
    "name": "Custom",
    "baseLimit": 30000,
    "cap": 60000
  },
  "creditLimit": 36000,
  "calculatedAt": "2026-04-25T12:00:00Z"
}
```

## Validation Rules

- `id` is required and must be unique
- `scoreMin` and `scoreMax` must be in the valid score range
- `ageMin` and `ageMax` must be in the valid age range
- `baseLimit` must be less than or equal to `cap`
- `debtRule` must be a valid C# boolean expression that evaluates to a boolean when executed against a `Customer` object

## Error Responses

### 400 Bad Request
```json
{
  "error": "Invalid cluster configuration",
  "details": [
    "ScoreMin must be <= ScoreMax",
    "DebtRule must be a valid boolean expression"
  ]
}
```

### 404 Not Found
```json
{
  "error": "Cluster not found"
}
```
