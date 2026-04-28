# API Contract: Customer Classification Endpoint

**Endpoint**: `POST /customers/classify`  
**Version**: 1.0  
**Status**: Extended (backward compatible)

---

## Request Contract

### HTTP Method
`POST`

### URL
```
POST /api/v1/customers/classify
```

### Headers
```
Content-Type: application/json
Accept: application/json
```

### Request Body Schema

```json
{
  "customerId": "string (required, non-empty)",
  "score": "integer (required, min: 300, max: 1000)",
  "age": "integer (required, min: 18, max: 100)",
  "hasMarketDebt": "boolean (required)",
  "marketDebtTypes": "array of strings (required, can be empty)"
}
```

### Request Example

**Scenario 1**: Customer qualifying for CLUSTER_A
```json
{
  "customerId": "CUST-001",
  "score": 750,
  "age": 45,
  "hasMarketDebt": false,
  "marketDebtTypes": []
}
```

**Scenario 2**: Customer qualifying for CLUSTER_B
```json
{
  "customerId": "CUST-002",
  "score": 600,
  "age": 35,
  "hasMarketDebt": true,
  "marketDebtTypes": ["card_debt"]
}
```

**Scenario 3**: Customer with no credit issues
```json
{
  "customerId": "CUST-003",
  "score": 550,
  "age": 28,
  "hasMarketDebt": false,
  "marketDebtTypes": []
}
```

**Scenario 4**: Customer defaulting to CLUSTER_D
```json
{
  "customerId": "CUST-004",
  "score": 250,
  "age": 55,
  "hasMarketDebt": true,
  "marketDebtTypes": ["credit_default", "loan_default"]
}
```

### Validation Rules

- `customerId`: Non-empty string, typically UUID or sequential ID
- `score`: Integer between 300 and 1000 inclusive
- `age`: Integer between 18 and 100 inclusive
- `hasMarketDebt`: Boolean (true or false)
- `marketDebtTypes`: Array of string values representing debt types:
  - Valid known values: `"credit_default"`, `"loan_default"`
  - Can contain custom debt type strings
  - Can be empty array if `hasMarketDebt` is false

### Error Responses

**400 Bad Request**: Invalid input
```json
{
  "status": 400,
  "error": "Bad Request",
  "message": "Score must be between 300 and 1000",
  "timestamp": "2026-04-25T10:30:00Z"
}
```

**422 Unprocessable Entity**: Validation failed
```json
{
  "status": 422,
  "error": "Validation Failed",
  "message": "Age must be between 18 and 100",
  "timestamp": "2026-04-25T10:30:00Z"
}
```

---

## Response Contract

### Success Response (200 OK)

**HTTP Status**: `200 OK`

**Response Body Schema**

```json
{
  "customerId": "string",
  "cluster": {
    "idCluster": "string (CLUSTER_A|CLUSTER_B|CLUSTER_C|CLUSTER_D)",
    "name": "string (Diamond|Gold|Silver|Bronze)",
    "baseLimit": "number (decimal)",
    "cap": "number (decimal)"
  },
  "creditLimit": "number (decimal)",
  "calculatedAt": "string (ISO 8601 datetime)"
}
```

### Response Examples

**Response 1**: CLUSTER_A Assignment
```json
{
  "customerId": "CUST-001",
  "cluster": {
    "idCluster": "CLUSTER_A",
    "name": "Diamond",
    "baseLimit": 50000,
    "cap": 100000
  },
  "creditLimit": 75000,
  "calculatedAt": "2026-04-25T10:30:15.123Z"
}
```

**Response 2**: CLUSTER_B Assignment
```json
{
  "customerId": "CUST-002",
  "cluster": {
    "idCluster": "CLUSTER_B",
    "name": "Gold",
    "baseLimit": 20000,
    "cap": 40000
  },
  "creditLimit": 28000,
  "calculatedAt": "2026-04-25T10:31:02.456Z"
}
```

**Response 3**: CLUSTER_C Assignment
```json
{
  "customerId": "CUST-003",
  "cluster": {
    "idCluster": "CLUSTER_C",
    "name": "Silver",
    "baseLimit": 5000,
    "cap": 10000
  },
  "creditLimit": 7500,
  "calculatedAt": "2026-04-25T10:32:10.789Z"
}
```

**Response 4**: CLUSTER_D Assignment (Default)
```json
{
  "customerId": "CUST-004",
  "cluster": {
    "idCluster": "CLUSTER_D",
    "name": "Bronze",
    "baseLimit": 0,
    "cap": 0
  },
  "creditLimit": 0,
  "calculatedAt": "2026-04-25T10:33:45.321Z"
}
```

### Response Field Descriptions

| Field | Type | Description |
|-------|------|-------------|
| `customerId` | string | Echo back the customer ID from request |
| `cluster.idCluster` | string | Assigned cluster identifier |
| `cluster.name` | string | Human-readable cluster name |
| `cluster.baseLimit` | decimal | Minimum credit limit for cluster |
| `cluster.cap` | decimal | Maximum credit limit for cluster |
| `creditLimit` | decimal | Calculated credit limit for this customer |
| `calculatedAt` | string | ISO 8601 timestamp of calculation (UTC) |

---

## Error Scenarios

### 400 Bad Request
Returned when:
- Request JSON is malformed
- Missing required fields
- Field types are incorrect

Example:
```json
{
  "status": 400,
  "error": "Bad Request",
  "message": "The customerId field is required",
  "timestamp": "2026-04-25T10:30:00Z"
}
```

### 422 Unprocessable Entity
Returned when:
- Score is outside [300, 1000]
- Age is outside [18, 100]
- Other validation rules fail

Example:
```json
{
  "status": 422,
  "error": "Validation Failed",
  "message": "Score must be between 300 and 1000",
  "errors": [
    {
      "field": "score",
      "value": 1500,
      "message": "Score must not exceed 1000"
    }
  ],
  "timestamp": "2026-04-25T10:30:00Z"
}
```

### 500 Internal Server Error
Returned when:
- Unexpected server error occurs
- Service unavailable

Example:
```json
{
  "status": 500,
  "error": "Internal Server Error",
  "message": "An unexpected error occurred",
  "timestamp": "2026-04-25T10:30:00Z"
}
```

---

## OpenAPI/Swagger Definition

```yaml
openapi: 3.0.0
info:
  title: Risk Watcher API
  version: 1.0.0
  description: Customer risk classification and credit limit calculation

paths:
  /api/v1/customers/classify:
    post:
      summary: Classify customer risk and calculate credit limit
      tags:
        - Customers
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/ClassifyCustomerRequest'
      responses:
        '200':
          description: Successfully classified customer
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/ClassifyCustomerResponse'
        '400':
          description: Bad request - malformed JSON
        '422':
          description: Validation failed - invalid input data
        '500':
          description: Internal server error

components:
  schemas:
    ClassifyCustomerRequest:
      type: object
      required:
        - customerId
        - score
        - age
        - hasMarketDebt
        - marketDebtTypes
      properties:
        customerId:
          type: string
          description: Unique customer identifier
        score:
          type: integer
          minimum: 300
          maximum: 1000
          description: Credit score
        age:
          type: integer
          minimum: 18
          maximum: 100
          description: Customer age
        hasMarketDebt:
          type: boolean
          description: Whether customer has market debt
        marketDebtTypes:
          type: array
          items:
            type: string
          description: Array of debt type strings

    ClassifyCustomerResponse:
      type: object
      properties:
        customerId:
          type: string
          description: Customer ID from request
        cluster:
          $ref: '#/components/schemas/Cluster'
        creditLimit:
          type: number
          format: decimal
          description: Calculated credit limit
        calculatedAt:
          type: string
          format: date-time
          description: Timestamp of calculation

    Cluster:
      type: object
      properties:
        idCluster:
          type: string
          enum: [CLUSTER_A, CLUSTER_B, CLUSTER_C, CLUSTER_D]
          description: Cluster identifier
        name:
          type: string
          enum: [Diamond, Gold, Silver, Bronze]
          description: Cluster name
        baseLimit:
          type: number
          format: decimal
          description: Minimum credit limit
        cap:
          type: number
          format: decimal
          description: Maximum credit limit
```

---

## Backward Compatibility

This contract extends the existing `/customers/classify` endpoint with new response fields:
- `cluster` (object with full cluster details including limits)
- `creditLimit` (calculated from customer data)

**Old clients** that only expect the cluster name continue to work - they can ignore the new fields.

**New clients** should use the complete cluster object and credit limit for full business context.

---

## Performance Requirements

- Response time: < 100ms for 95th percentile
- Availability: 99.9% uptime
- Throughput: Support 1000+ requests/second

---

## Security Considerations

- No sensitive data exposed (credit limit is business-critical but not a secret)
- Request validation prevents injection attacks
- HTTPS should be used in production
- Consider rate limiting for DOS prevention
