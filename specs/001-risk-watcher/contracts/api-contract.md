# API Contract: Risk Watcher

**Feature**: Risk Watcher | **Date**: 2026-04-23

## Overview

The Risk Watcher API provides customer risk classification functionality through a single REST endpoint.

## Endpoints

### POST /customers/classify

Classifies a customer into a risk cluster based on their financial and personal data.

**Request**:
- Method: POST
- Content-Type: application/json
- Body: Customer data (see Customer schema below)

**Response**:
- Status: 200 OK
- Content-Type: application/json
- Body: Customer data enriched with cluster assignment

**Error Responses**:
- 400 Bad Request: Invalid input data
- 500 Internal Server Error: Server error

## Schemas

### Customer (Request/Response)

```json
{
  "id": "string",
  "name": "string",
  "age": "integer",
  "score": "integer (0-1000)",
  "has_market_debt": "boolean",
  "market_debt_types": ["string"],
  "location": {
    "city": "string",
    "state": "string",
    "region": "string"
  },
  "job_title": "string",
  "cluster": "string (response only)"
}
```

**Field Constraints**:
- `market_debt_types`: Allowed values: "credit_card", "personal_loan", "mortgage", "credit_default", "loan_default"
- `location.region`: Allowed values: "Norte", "Nordeste", "Centro-Oeste", "Sudeste", "Sul"
- `cluster`: Allowed values: "CLUSTER_A", "CLUSTER_B", "CLUSTER_C", "CLUSTER_D"

## Example Request

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
  "job_title": "Software Engineer"
}
```

## Example Response

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