# Feature Specification: monthly-income-mapping

**Feature Branch**: `007-monthly-income-mapping`  
**Created**: April 27, 2026  
**Status**: Draft  
**Input**: User description: "new feature to identify the monthly income of customers based on CLUSTER_ID and job category identified in previous step in customer classification. The requirements for the users are: 1) User can add, remove, or edit a monthly income; 2) User can define the value of the monthly income based on cluster id and job category id. The structure of the monthly income data are: - CLUSTER_ID (string); - JOB_CATEGORY_ID (string); - VALUE (decimal)"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Manage monthly income mappings (Priority: P1)

A business user can create, edit, and remove monthly income mappings that link a customer’s cluster and job category to a monthly income value.

**Why this priority**: This is the core feature that enables the system to associate a specific income value with a classified customer and keeps the mapping data maintainable.

**Independent Test**: A user can create a mapping, update its value, remove it, and verify that the stored mapping reflects the latest state without requiring a classification request.

**Acceptance Scenarios**:

1. **Given** a valid cluster ID and job category ID, **When** a user adds a new monthly income mapping, **Then** the system stores the mapping with the provided decimal value.
2. **Given** an existing monthly income mapping, **When** a user updates the value, **Then** the system returns the updated value for that cluster and category combination.
3. **Given** an existing monthly income mapping, **When** a user deletes it, **Then** the mapping is removed and no longer returned for that cluster and category combination.

---

### User Story 2 - Apply income mapping after classification (Priority: P2)

After classifying a customer by cluster and job category, the system identifies the matching monthly income and includes it in the classification result.

**Why this priority**: This delivers the business value that the classification pipeline can produce a financial indicator based on the classification result.

**Independent Test**: Classify a customer and verify the response includes the monthly income value from the corresponding cluster/job category mapping.

**Acceptance Scenarios**:

1. **Given** a classified customer with `clusterId` and `jobCategoryId`, **When** the classification is completed, **Then** the response includes the corresponding `monthlyIncome` value.
2. **Given** a classified customer without a matching income mapping, **When** classification is completed, **Then** the response indicates the monthly income is not available or is null.

---

### User Story 3 - Ensure mapping uniqueness and validation (Priority: P3)

The system prevents duplicate monthly income mappings for the same cluster and job category, and it validates the income value before saving.

**Why this priority**: Preventing duplicates and invalid values protects data quality and ensures classification uses a single authoritative income source.

**Independent Test**: Attempt to add a duplicate mapping or invalid value and verify the system rejects the request with a clear validation error.

**Acceptance Scenarios**:

1. **Given** an existing mapping for a cluster/category combination, **When** a user attempts to add a second mapping for the same combination, **Then** the system rejects it or updates the existing mapping according to the chosen behavior.
2. **Given** a mapping request with a negative or malformed value, **When** the request is submitted, **Then** the system returns a validation error and does not save the mapping.

---

### Edge Cases

- What happens when `clusterId` or `jobCategoryId` is missing, empty, or whitespace?
- How does the system behave when a monthly income value is zero, negative, or exceeds expected ranges?
- How are stale or deleted classifications handled if the income mapping is removed after a customer has been classified?
- How does the system handle cluster/category combinations that have no mapping defined?
- How will duplicate mappings be detected and resolved if a user submits the same cluster/category more than once?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow users to create a monthly income mapping keyed by `clusterId` and `jobCategoryId`.
- **FR-002**: System MUST allow users to update an existing monthly income mapping value.
- **FR-003**: System MUST allow users to delete an existing monthly income mapping.
- **FR-004**: System MUST store monthly income mappings with the attributes `clusterId`, `jobCategoryId`, and `value`.
- **FR-005**: System MUST use the cluster ID and job category ID from customer classification results to look up the monthly income.
- **FR-006**: System MUST ensure `value` is a valid non-negative decimal when creating or updating a mapping.
- **FR-007**: System MUST enforce uniqueness of the `clusterId` and `jobCategoryId` combination for monthly income mappings.
- **FR-008**: System MUST return the monthly income in the customer classification response when a matching mapping exists.
- **FR-009**: System MUST return an explicit empty or null monthly income value when no mapping exists for the classified customer.

### Key Entities *(include if feature involves data)*

- **MonthlyIncomeMapping**: Represents a configured income value for a specific `clusterId` and `jobCategoryId`.
  - `clusterId` (string)
  - `jobCategoryId` (string)
  - `value` (decimal)
- **CustomerClassificationResult**: Represents the classification output for a customer and now includes a `monthlyIncome` attribute when a mapping is found.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of classification results with a matching monthly income mapping include the correct `monthlyIncome` value.
- **SC-002**: Users can add, update, or delete a monthly income mapping and receive a success or validation response within 2 seconds.
- **SC-003**: The system supports at least 100 distinct `clusterId`/`jobCategoryId` monthly income mappings without data conflicts.
- **SC-004**: Duplicate mappings for the same cluster/category combination are prevented or resolved consistently.
- **SC-005**: At least one end-to-end test covers a classified customer returning a monthly income and one covers no mapping found.

## Assumptions

- This feature will reuse the existing in-memory mock repository pattern used by classification and category configuration features.
- Monthly income mappings are managed through an administrative configuration interface or API, not through customer self-service.
- The feature is limited to cluster and job category combinations; it does not infer income from other customer attributes.
- The classification pipeline can access the resulting `clusterId` and `jobCategoryId` before income lookup.
- The feature does not require persistent storage beyond the current runtime session unless the underlying system already supports it.
