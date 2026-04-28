# Data Model: Risk Watcher

**Feature**: Risk Watcher | **Date**: 2026-04-23

## Entities

### Customer

Represents a bank customer with financial and personal data used for risk classification.

**Attributes**:
- `id` (string): Unique customer identifier
- `name` (string): Customer full name
- `age` (integer): Customer age in years
- `score` (integer): Credit score (0-1000 range)
- `has_market_debt` (boolean): Whether customer has any market debt
- `market_debt_types` (string[]): Array of debt types, possible values: credit_card, personal_loan, mortgage, credit_default, loan_default
- `location.city` (string): Customer city
- `location.state` (string): Customer state (e.g., SP, RJ, MG, MA, RS)
- `location.region` (string): Customer region, possible values: Norte, Nordeste, Centro-Oeste, Sudeste, Sul
- `job_title` (string): Customer job title
- `cluster` (string): Assigned risk cluster, possible values: CLUSTER_A, CLUSTER_B, CLUSTER_C, CLUSTER_D

**Validation Rules**:
- `score`: Must be between 0 and 1000
- `age`: Must be positive integer
- `market_debt_types`: Each value must be from allowed list
- `location.region`: Must be from allowed list
- `cluster`: Assigned by business logic, not user input

**Relationships**:
- None (single entity system)

## Business Rules for Clustering

The `cluster` is derived from the following rules:

- **CLUSTER_A (Low Risk)**: score >= 800 AND has_market_debt = false AND region in (Sudeste, Sul)
- **CLUSTER_B (Medium-Low Risk)**: score >= 600 AND (has_market_debt = false OR market_debt_types contains only 'credit_card')
- **CLUSTER_C (Medium-High Risk)**: score >= 400 AND has_market_debt = true AND market_debt_types not contains 'credit_default' or 'loan_default'
- **CLUSTER_D (High Risk)**: All other cases (score < 400 OR has_market_debt with defaults OR high-risk regions with debt)