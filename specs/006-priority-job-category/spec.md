# Feature Specification: Define job category priority for customer classification

**Feature Branch**: `006-priority-job-category`
**Created**: April 26, 2026
**Status**: Draft
**Input**: User description: "new feature to define the job category in customer classification that must exist a new information of priority. The priority should be used to define the category order execute the keywords validation based on jobTitle of the customer. If any job category is founded, the last category must be defined to the customer classification."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Assign job category by priority-ordered keywords (Priority: P1)

A classification analyst needs the customer classifier to choose a job category based on the customer job title and a defined category priority order.

**Why this priority**: This is the core feature: customer classification must use job category priority to make deterministic category assignments.

**Independent Test**: Submit a customer classification request with a job title that matches multiple job categories and verify the final category is the last matched category after ordered keyword validation.

**Acceptance Scenarios**:

1. **Given** a customer with a job title containing keywords from multiple categories, **When** classification runs, **Then** the system evaluates categories in priority order and returns the final matched category from that ordered sequence.
2. **Given** a customer with a single matching category, **When** classification runs, **Then** the system returns that category.
3. **Given** a customer with a job title that does not match any category, **When** classification runs, **Then** the system applies fallback classification logic and does not assign a matched category.

---

### User Story 2 - Manage category priority metadata (Priority: P2)

A product owner needs job categories to include explicit priority metadata so the classifier can sort validation order before matching keywords.

**Why this priority**: Priority metadata is required for the new text-matching algorithm and prevents ambiguous category selection.

**Independent Test**: Verify job category definitions include a priority field and that classification uses it to order category evaluation.

**Acceptance Scenarios**:

1. **Given** multiple job categories with overlapping keywords, **When** the system sorts categories by priority, **Then** categories are evaluated from highest to lowest priority.
2. **Given** categories with equal priority, **When** the system evaluates keywords, **Then** it uses a consistent secondary ordering rule or stable insertion order.

---

### User Story 3 - Persist final category in customer classification results (Priority: P3)

A business user needs classification results to include the chosen job category when a category is found, and the result must clearly indicate the final assigned category.

**Why this priority**: Visibility of the selected job category completes the classification workflow and supports reporting.

**Independent Test**: Examine classification output for a matching customer and confirm the response includes the selected job category.

**Acceptance Scenarios**:

1. **Given** a matched customer classification, **When** the result is returned, **Then** the response includes the final job category name and identifier.
2. **Given** no category matches, **When** the result is returned, **Then** the response explicitly shows no matched category assignment.

---

### Edge Cases

- When the job title is empty or null, the system must not attempt priority keyword validation and must use fallback classification behavior.
- When multiple categories share the same priority, the system must maintain a deterministic evaluation order and still return the final matched category.
- When a keyword appears in multiple categories, the category priority order must determine which match is evaluated last.
- When a category priority value is missing or invalid, the system must treat it as lowest priority and still validate keywords.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST add a priority attribute to job category definitions that determines the order of keyword validation.
- **FR-002**: The system MUST evaluate job categories using the customer's jobTitle and the configured priority order.
- **FR-003**: The system MUST determine a final job category when one or more categories match the jobTitle, using the last category found in the ordered validation sequence.
- **FR-004**: The system MUST return the selected job category in the customer classification result when a match is found.
- **FR-005**: The system MUST handle equal priority values in a deterministic way, such as by stable order or explicit secondary sorting.
- **FR-006**: The system MUST safely handle missing or empty jobTitle values without throwing classification errors.
- **FR-007**: The system MUST preserve existing classification behavior for cases where no job category matches.

### Key Entities *(include if feature involves data)*

- **Job Category**: Represents a category with attributes: `Id`, `Name`, `Keywords`, `Priority`, and optional fallback settings.
- **Customer Classification**: Represents a classification result with `CustomerId`, `JobTitle`, matched `JobCategory`, and classification metadata.
- **Job Title**: Represents the customer-provided text used to validate category keywords in priority order.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: When multiple categories match a job title, the system selects the final category from the ordered validation sequence in 100% of test cases.
- **SC-002**: 100% of job category definitions used for classification include a `Priority` value or are treated with a defined fallback priority.
- **SC-003**: Classification responses include the selected job category details for every customer where a category match exists.
- **SC-004**: The classification flow continues without failure for empty or null job titles in at least 95% of relevant test cases.

## Assumptions

- Job categories are already modeled in the system and can be extended with a new `Priority` field.
- Existing customer classification requests already include `jobTitle`, and this feature will use that field for keyword matching.
- Priority is a numeric or ordered field where lower/higher values imply evaluation order; the exact sort direction will be defined consistently in implementation.
- No changes to external API contract are required beyond adding the selected job category to classification output.
- The final matched category is the single definitive output from the ordered matching process, not a list of all matches.
