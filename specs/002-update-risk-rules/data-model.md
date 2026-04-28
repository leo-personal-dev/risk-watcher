# Phase 1: Data Model Design

**Date**: 2026-04-25  
**Feature**: Update Risk Rules  
**Status**: Complete

## Entity Relationship Diagram

```
┌─────────────┐                    ┌──────────────────┐
│  Customer   │                    │ ClassificationResult │
├─────────────┤                    ├──────────────────┤
│ Id          │◄──────────────────►│ CustomerId       │
│ Score       │  belongs_to        │ Cluster          │
│ Age         │                    │ CreditLimit      │
│ HasMarketDebt│                   │ CalculatedAt     │
│ DebtTypes[] │                    └──────────────────┘
└─────────────┘
      │                                   ▲
      │                                   │
      │ classified_into                   │ returns
      │                                   │
      └──────────────────────────────────┐
                                         │
                                    ┌────┴─────────┐
                                    │    Cluster   │
                                    ├──────────────┤
                                    │ IdCluster    │
                                    │ Name         │
                                    │ BaseLimit    │
                                    │ Cap          │
                                    └──────────────┘
```

## Entity Definitions

### 1. Customer Entity

**Purpose**: Represents a customer with credit risk profile information.

**Location**: `src/Watcher.Domain/Entities/Customer.cs`

**Properties**:
| Property | Type | Required | Description | Constraints |
|----------|------|----------|-------------|-------------|
| Id | string | Yes | Unique customer identifier | Non-empty |
| Score | int | Yes | Credit score | 300-1000 |
| Age | int | Yes | Customer age | 18-100 |
| HasMarketDebt | bool | Yes | Has any market debt | - |
| MarketDebtTypes | string[] | Yes | Types of debt held | Empty array if no debt |

**Validation Rules**:
- Score must be between 300 and 1000
- Age must be between 18 and 100
- MarketDebtTypes is an array of debt type strings (e.g., "credit_default", "loan_default")

**Existing**: Already implemented in Phase 1 of first feature. Enhance with clarified validation.

---

### 2. Cluster Entity (NEW)

**Purpose**: Represents a risk cluster definition with associated credit limits.

**Location**: `src/Watcher.Domain/Entities/Cluster.cs`

**Definition**:
```csharp
public record Cluster(
    string IdCluster,
    string Name,
    decimal BaseLimit,
    decimal Cap
);

// Pre-defined cluster instances
public static class ClusterDefinitions
{
    public static readonly Cluster CLUSTER_A = new("CLUSTER_A", "Diamond", 50000, 100000);
    public static readonly Cluster CLUSTER_B = new("CLUSTER_B", "Gold", 20000, 40000);
    public static readonly Cluster CLUSTER_C = new("CLUSTER_C", "Silver", 5000, 10000);
    public static readonly Cluster CLUSTER_D = new("CLUSTER_D", "Bronze", 0, 0);
}
```

**Properties**:
| Property | Type | Required | Description | Constraints |
|----------|------|----------|-------------|-------------|
| IdCluster | string | Yes | Cluster identifier | CLUSTER_A, CLUSTER_B, CLUSTER_C, CLUSTER_D |
| Name | string | Yes | Human-readable name | Diamond, Gold, Silver, Bronze |
| BaseLimit | decimal | Yes | Minimum credit limit | Non-negative |
| Cap | decimal | Yes | Maximum credit limit | >= BaseLimit |

**Validation Rules**:
- IdCluster must be one of the predefined cluster IDs
- BaseLimit must be >= 0
- Cap must be >= BaseLimit
- All clusters are immutable (read-only records)

---

### 3. ClassificationResult Entity (NEW)

**Purpose**: Represents the output of the customer classification process.

**Location**: `src/Watcher.Domain/Entities/ClassificationResult.cs`

**Definition**:
```csharp
public record ClassificationResult(
    string CustomerId,
    Cluster Cluster,
    decimal CreditLimit,
    DateTime CalculatedAt
);
```

**Properties**:
| Property | Type | Required | Description | Constraints |
|----------|------|----------|-------------|-------------|
| CustomerId | string | Yes | Reference to classified customer | Non-empty |
| Cluster | Cluster | Yes | Assigned cluster | One of four defined clusters |
| CreditLimit | decimal | Yes | Calculated credit limit | Within cluster's BaseLimit and Cap |
| CalculatedAt | DateTime | Yes | Timestamp of calculation | UTC timezone |

**Validation Rules**:
- CreditLimit must be >= Cluster.BaseLimit
- CreditLimit must be <= Cluster.Cap
- CalculatedAt must be in UTC timezone

---

## State Transitions

### Customer Classification Flow

```
┌──────────────────────────────────────┐
│  Customer data received              │
│  (Id, Score, Age, Debt info)         │
└──────────────────┬───────────────────┘
                   │
                   ▼
┌──────────────────────────────────────┐
│  Priority 1: CLUSTER_A Check         │
│  Score >= 700 AND                    │
│  Age 25-60 AND                       │
│  has_market_debt = false             │
└──────────────────┬───────────────────┘
                   │
          ┌────────┴────────┐
          │                 │
       YES                  NO
       │                    │
       ▼                    ▼
   CLUSTER_A        ┌──────────────────────────────────────┐
                    │  Priority 2: CLUSTER_B Check         │
                    │  Score >= 500 AND                    │
                    │  Age 18-65 AND                       │
                    │  NO credit_default/loan_default      │
                    └──────────────────┬───────────────────┘
                                       │
                          ┌────────────┴────────────┐
                          │                         │
                       YES                          NO
                       │                            │
                       ▼                            ▼
                   CLUSTER_B            ┌──────────────────────────────────────┐
                                        │  Priority 3: CLUSTER_C Check         │
                                        │  Score >= 300                        │
                                        └──────────────────┬───────────────────┘
                                                           │
                                          ┌────────────────┴────────────────┐
                                          │                                 │
                                       YES                                  NO
                                       │                                    │
                                       ▼                                    ▼
                                   CLUSTER_C                            CLUSTER_D
                                                    (Default)

┌──────────────────────────────────────┐
│  Calculate Credit Limit              │
│  Within Cluster Range                │
│  [BaseLimit, Cap]                    │
└──────────────────┬───────────────────┘
                   │
                   ▼
┌──────────────────────────────────────┐
│  Return ClassificationResult         │
│  (Cluster + CreditLimit)             │
└──────────────────────────────────────┘
```

---

## Relationships and Constraints

### Customer → Cluster (N:1)
- Many customers can be assigned to the same cluster
- Customer cluster assignment is determined by business rules
- Assignment is **deterministic**: same customer data always produces same cluster

### Cluster Hierarchy (Priority Order)
- CLUSTER_A (highest tier, strictest criteria)
- CLUSTER_B (tier 2, moderate criteria)
- CLUSTER_C (tier 3, basic score threshold)
- CLUSTER_D (default tier, catch-all)

### Credit Limit Constraints
- CreditLimit is always within [Cluster.BaseLimit, Cluster.Cap]
- CreditLimit is calculated from customer score and cluster range
- For CLUSTER_D: CreditLimit is always 0 (BaseLimit = Cap = 0)

---

## Domain Invariants

1. **Every customer is assigned exactly one cluster**: Classification is mutually exclusive
2. **Cluster assignments are priority-ordered**: First matching rule wins (no fallthrough)
3. **Credit limits respect cluster boundaries**: CreditLimit ∈ [BaseLimit, Cap]
4. **Score ranges are inclusive**: Ages and scores use >= and <= (not > and <)
5. **No customer exists outside clusters**: Classification always completes (CLUSTER_D catch-all)

---

## Data Validation Rules

### Customer Validation
- Score: Must be integer, 300-1000 inclusive
- Age: Must be integer, 18-100 inclusive
- HasMarketDebt: Boolean flag (true/false)
- MarketDebtTypes: Array of strings or empty array
  - Valid values: "credit_default", "loan_default", or custom types
  - Empty array is valid (represents no debt)

### Cluster Validation
- IdCluster: Must match predefined cluster IDs
- BaseLimit: Must be non-negative decimal
- Cap: Must be >= BaseLimit
- Clusters are immutable after creation

### Classification Result Validation
- CustomerId: Must reference valid customer
- Cluster: Must be one of four predefined clusters
- CreditLimit: Must satisfy BaseLimit ≤ CreditLimit ≤ Cap
- CalculatedAt: Must be valid DateTime in UTC

---

## Boundary Conditions

### Age Boundaries
- **Inclusive at 25**: Customer with age=25 qualifies for age range [25, 60]
- **Inclusive at 60**: Customer with age=60 qualifies for age range [25, 60]
- **Inclusive at 18**: Customer with age=18 qualifies for age range [18, 65]
- **Inclusive at 65**: Customer with age=65 qualifies for age range [18, 65]

### Score Boundaries
- **Inclusive at 700**: Customer with score=700 qualifies for score >= 700 check (CLUSTER_A)
- **Inclusive at 500**: Customer with score=500 qualifies for score >= 500 check (CLUSTER_B)
- **Inclusive at 300**: Customer with score=300 qualifies for score >= 300 check (CLUSTER_C)

### Debt Types Boundary
- **No credit_default OR loan_default**: Customer is eligible for CLUSTER_B only if MarketDebtTypes array does NOT contain these strings
- **Empty array**: If MarketDebtTypes is empty, customer can qualify for CLUSTER_B (assuming other conditions met)

---

## Migration Notes

The data model builds on existing Customer entity and extends domain with new Cluster and ClassificationResult entities. No breaking changes to existing Customer structure. All new entities use C# record types for immutability and value-based equality.
