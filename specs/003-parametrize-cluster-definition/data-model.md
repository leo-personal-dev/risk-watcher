# Data Model: Parametrize Cluster Definition

**Feature**: Parametrize Cluster Definition  
**Date**: 2026-04-25  
**Purpose**: Define the data structures and relationships for user-parametrizable cluster configurations

## Domain Entities

### ClusterConfiguration
Represents a user-defined cluster with all parametrizable attributes.

**Attributes**:
- `Id` (string): Unique identifier for the cluster (user-defined)
- `Name` (string): Human-readable name (e.g., "Diamond", "Gold")
- `ScoreMin` (int): Minimum credit score for eligibility (300-1000)
- `ScoreMax` (int): Maximum credit score for eligibility (300-1000)
- `AgeMin` (int): Minimum age for eligibility (18-100)
- `AgeMax` (int): Maximum age for eligibility (18-100)
- `DebtRule` (string): Boolean expression in C# notation for debt eligibility (e.g., "!customer.HasMarketDebt || !customer.MarketDebtTypes.Contains(\"credit_default\")")
- `BaseLimit` (decimal): Base credit limit for the cluster
- `Cap` (decimal): Maximum credit limit cap for the cluster

**Validation Rules**:
- ScoreMin <= ScoreMax
- AgeMin <= AgeMax
- BaseLimit <= Cap
- DebtRule must be valid C# boolean expression syntax
- Id must be unique across all configurations

**Relationships**:
- Used by ClassificationService to determine customer cluster assignment
- Referenced by ClassificationResult to store assigned cluster

### Customer (existing, unchanged)
Existing entity with score, age, debt information used in cluster evaluation.

## Data Flow

1. **Configuration Storage**: ClusterConfiguration entities stored in mocked in-memory repository
2. **Classification Process**: 
   - Load all active ClusterConfiguration
   - Evaluate each cluster's conditions (score range, age range, debt rule) against customer data
   - Select highest priority matching cluster
   - Calculate credit limit within cluster's base/cap range
3. **API Operations**: CRUD operations on ClusterConfiguration through REST endpoints

## Mock Data Structure

Since no database integration, cluster configurations will be stored in an in-memory collection:

```csharp
private static readonly List<ClusterConfiguration> _clusters = new()
{
    new ClusterConfiguration
    {
        Id = "CLUSTER_A",
        Name = "Diamond",
        ScoreMin = 700,
        ScoreMax = 1000,
        AgeMin = 25,
        AgeMax = 60,
        DebtRule = "!customer.HasMarketDebt",
        BaseLimit = 50000m,
        Cap = 100000m
    },
    // ... other clusters
};
```

## Debt Rule Evaluation

Debt rules are C# boolean expressions evaluated against customer data:

- Available variables: `customer` (Customer entity)
- Expression must return bool
- Examples:
  - `"!customer.HasMarketDebt"`: No market debt
  - `"!customer.MarketDebtTypes.Contains(\"credit_default\")"`: No credit default in debt types
  - `"customer.Age >= 30 && !customer.MarketDebtTypes.Any()"`: Age 30+ with no debt types

Evaluation will use dynamic compilation or expression parsing to execute the boolean logic safely.