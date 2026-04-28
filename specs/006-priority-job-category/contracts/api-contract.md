# API Contract: Job Category Priority Classification

## Endpoint: Customer classification

### Request
- `POST /customers/classify`
- Body: `ClassifyCustomerCommand`

```json
{
  "customerId": "string",
  "score": 0,
  "age": 0,
  "hasMarketDebt": false,
  "marketDebtTypes": ["string"],
  "jobTitle": "string"
}
```

### Response
- `200 OK`
- Body: `ClassifyCustomerResponse`

```json
{
  "customerId": "string",
  "clusterId": "string",
  "clusterName": "string",
  "jobCategoryId": "string|null",
  "jobCategoryName": "string|null",
  "creditLimit": 0.0,
  "calculatedAt": "2026-04-26T00:00:00Z"
}
```

## New domain behavior

- Job categories now contain a `Priority` field used to order category evaluation.
- Classification selects the final matching category according to the ordered priority sequence.
- If no categories match, the response returns `jobCategoryId` and `jobCategoryName` as `null`.

## Job category model contract

```json
{
  "id": "string",
  "name": "string",
  "multiplier": 0.0,
  "keywords": ["string"],
  "priority": 0
}
```

## Repository behavior

- The job category repository is mock-based and retains categories in memory only.
- The feature does not require a database connection.
- Changes to category priority are effective only for the runtime session.
