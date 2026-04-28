# Research: Priority-driven job category classification

## Decision

Add a `Priority` field to the existing `JobCategory` entity and use it to order keyword validation during customer classification. The classification algorithm will evaluate categories in priority order and select the last matched category from that ordered sequence.

## Rationale

- The current `IdentifyCategoryAsync` implementation returns the first matching category, which is insufficient when multiple categories share overlapping keywords.
- Priority metadata gives business users explicit control over category evaluation order without changing the API surface or introducing database dependencies.
- The repository remains in-memory mock-based, preserving the current architecture constraint of no real database connection.
- Using a stable, ordered matching algorithm keeps the classification deterministic and easy to reason about.

## Alternatives Considered

1. **First-match evaluation**
   - Would preserve current behavior but not satisfy the new requirement for priority-driven category assignment.
   - Rejected because it cannot guarantee the final category will follow explicit priority order.

2. **Weighted keyword scoring**
   - Could compute a score per category and choose the highest score.
   - Rejected because it adds complexity and makes category selection less transparent for users.

3. **Separate fallback category list**
   - Could add a secondary fallback rule when multiple matches exist.
   - Rejected because the new requirement explicitly calls for priority-based matching and a final matched category.

## Chosen Implementation Pattern

- Extend `JobCategory` with `Priority`.
- Update the mock `JobCategoryRepository` seed data with priority values.
- Change `IJobCategoryService.IdentifyCategoryAsync` to order categories by priority and return the last matched category.
- Preserve the current `ClassificationService` flow and the existing mock-only storage strategy.

## Key Findings

- The feature can be implemented entirely inside the domain and infrastructure layers, without requiring API route changes.
- No database connection is needed; the existing in-memory mock repository remains the authoritative source for job categories.
- Adding priority is a minimal, backward-compatible change to the job category model.
