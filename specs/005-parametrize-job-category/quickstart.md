# Quickstart: Job Category Parametrization

**Feature**: Parametrize Job Category  
**Date**: 2026-04-25  

## Overview
This feature allows administrators to define custom job categories for customer classification based on job titles.

## Prerequisites
- ASP.NET Core Web API running
- Access to `/api/job-categories` endpoints

## Step 1: Create Job Categories

Use POST to create categories:

```bash
curl -X POST /api/job-categories \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Executive",
    "multiplier": 2.0,
    "keywords": ["CEO", "CFO", "CTO", "Chief", "President", "VP"]
  }'
```

Repeat for other categories like "Senior Professional", "Mid Professional", etc.

## Step 2: View Categories

List all categories:

```bash
curl /api/job-categories
```

## Step 3: Update Categories

Modify existing categories:

```bash
curl -X PUT /api/job-categories/EXECUTIVE \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Executive",
    "multiplier": 2.5,
    "keywords": ["CEO", "CFO", "CTO", "Chief", "President", "VP", "COO"]
  }'
```

## Step 4: Delete Categories

Remove unused categories:

```bash
curl -X DELETE /api/job-categories/EXECUTIVE
```

## Usage in Classification

Job categories are automatically identified during customer classification when jobTitle matches keywords. The multiplier is stored for future risk calculation features.