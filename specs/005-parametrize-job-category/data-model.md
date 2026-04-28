# Data Model

**Feature**: Parametrize Job Category  
**Date**: 2026-04-25  

## Entities

### JobCategory
Represents a user-defined job category for customer classification.

**Attributes**:
- `Id` (string): Unique identifier for the category
- `Name` (string): Human-readable name (e.g., "EXECUTIVE", "SENIOR_PROFESSIONAL")
- `Multiplier` (decimal): Numerical multiplier for risk calculations
- `Keywords` (List<string>): List of keywords for jobTitle matching

**Relationships**:
- None (standalone entity)

**Validation Rules**:
- Id: Required, unique
- Name: Required, non-empty
- Multiplier: Required, > 0
- Keywords: Required, at least one keyword

### Customer (Extended)
Existing Customer entity extended with jobTitle for category identification.

**New Attributes**:
- `JobTitle` (string): Customer's job title for keyword matching

**Relationships**:
- References JobCategory via identification logic

## Data Flow

1. JobCategory entities stored in in-memory repository
2. Customer.JobTitle matched against JobCategory.Keywords
3. First matching category returned
4. Category.Multiplier used in risk calculations (future feature)