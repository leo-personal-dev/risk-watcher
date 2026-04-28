# Data Model: Priority-based Job Category Matching

## Entities

### JobCategory
- `Id` (string): Unique identifier.
- `Name` (string): Human-readable category name.
- `Multiplier` (decimal): Credit limit multiplier.
- `Keywords` (List<string>): Terms used to match against `jobTitle`.
- `Priority` (int): Defines evaluation order for keyword matching. Higher values or lower values should be chosen consistently; implementation will treat this as the primary sort key.

### Customer
- `Id` (string): Unique customer identifier.
- `Score` (int): Credit score used for cluster evaluation.
- `Age` (int): Customer age.
- `HasMarketDebt` (bool): Market debt indicator.
- `MarketDebtTypes` (List<string>): Types of market debt.
- `JobTitle` (string): Customer job title used for category keyword validation.

### ClassificationResult
- `CustomerId` (string)
- `Cluster` (`Cluster` entity): Selected cluster based on score and age.
- `JobCategory` (`JobCategory?`): Selected job category if matched.
- `CreditLimit` (decimal): Computed credit limit.
- `CalculatedAt` (DateTime): Timestamp of classification.

## Relationships

- `ClassificationService` depends on `IJobCategoryService` for category identification.
- `JobCategoryService` reads from the mock `JobCategoryRepository` and selects the final category by ordered priority.
- The `Customer` entity is consumed by `ClassificationService` and does not persist through a database.

## Validation Rules

- `JobCategory.Priority` MUST exist for categories used in ordered matching.
- `JobCategory.Keywords` MUST contain at least one non-empty keyword.
- `Customer.JobTitle` MAY be empty, but matching then returns no category.
- If `JobCategory.Priority` is missing or invalid, the system MUST treat it as the lowest effective priority.

## Notes

- Storage remains mock-based, so `JobCategory` priority values are stored in-memory during runtime only.
- The new `Priority` field is an ordering attribute and not a business score.
