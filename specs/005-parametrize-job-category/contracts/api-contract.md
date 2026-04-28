# API Contract: Job Category Management

**Feature**: Parametrize Job Category  
**Base URL**: `/api/job-categories`  
**Date**: 2026-04-25  

## Overview
RESTful API for managing user-parametrizable job categories used in customer classification.

## Endpoints

### GET /api/job-categories
Retrieve all job categories.

**Response**: 200 OK
```json
[
  {
    "id": "EXECUTIVE",
    "name": "Executive",
    "multiplier": 2.0,
    "keywords": ["CEO", "CFO", "CTO", "Chief", "President"]
  }
]
```

### POST /api/job-categories
Create a new job category.

**Request Body**:
```json
{
  "name": "Senior Professional",
  "multiplier": 1.5,
  "keywords": ["Senior", "Manager", "Director"]
}
```

**Response**: 201 Created
```json
{
  "id": "SENIOR_PROFESSIONAL",
  "name": "Senior Professional",
  "multiplier": 1.5,
  "keywords": ["Senior", "Manager", "Director"]
}
```

### PUT /api/job-categories/{id}
Update an existing job category.

**Request Body**: Same as POST, id in URL.

**Response**: 200 OK with updated category.

### DELETE /api/job-categories/{id}
Delete a job category.

**Response**: 204 No Content

## Error Responses

- 400 Bad Request: Invalid input data
- 404 Not Found: Category not found
- 409 Conflict: Category name already exists

## Data Types

### JobCategory
- id: string (auto-generated)
- name: string (required)
- multiplier: decimal (required, > 0)
- keywords: string[] (required, non-empty)