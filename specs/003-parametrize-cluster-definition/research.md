# Research: Parametrize Cluster Definition

## Decision

Use an in-memory mocked configuration table for cluster definitions and interpret debt eligibility rules as C# boolean expressions evaluated against the `Customer` object.

## Rationale

- The feature explicitly excludes database integration, so an in-memory mock aligns with the current project constraints.
- C# boolean expression syntax matches the existing codebase and supports dynamic debt rule evaluation without introducing a separate rule language.
- Using a list of `ClusterConfiguration` objects preserves the existing clean architecture while allowing CRUD operations on cluster definitions.

## Alternatives Considered

- **Database-backed cluster storage**: Rejected because the feature request specifically excludes database integration.
- **JSON rule format**: Rejected because the request requires C# boolean expression syntax and dynamic evaluation against customer input.
- **Hardcoded cluster definitions**: Rejected because the user must be able to add, remove, and edit clusters.

## Implementation Notes

- Cluster configurations can be represented as a `ClusterConfiguration` entity with fields for score range, age range, debt rule, base limit, and cap.
- Debt rules can be stored as strings and evaluated using C# expression compilation or a safe expression parser.
- A mock repository should return a predefined table-like collection of cluster definitions.
- Classification logic will choose the first matching cluster based on priority order defined by the in-memory list.
