# Feature Specification: Parametrize Job Category

**Feature Branch**: `005-parametrize-job-category`  
**Created**: 2026-04-25  
**Status**: Draft  
**Input**: User description: "New feature to identify the job category of the customer based on jobTitle information. The identification is based on a user parametrization with the following requirements:

1) User Can add, remove or edit a job category
2) User can define the the category name (e.g. EXECUTIVE, SENIOR_PROFESSIONAL, MID_PROFESSIONAL)
3) User can define a multiplier number (e.g. 0.8, 2.0, 1.5, ...)
4) User can define a list of keywords for each category (e.g. for EXECUTIVE: CEO, CFO, CTO, COO, CIO, CMO, Chief, President, Vice President, VP, Director)

The identification of the job category is based on the match of the customer jobTitle founded in keyword list in the specific category."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Manage Job Categories (Priority: P1)

As a system administrator, I want to add, edit, and remove job categories so that I can customize the categorization rules for customers.

**Why this priority**: This is the foundation for the entire feature, enabling user parametrization of job categories.

**Independent Test**: Can be fully tested by CRUD operations on job categories and verifying they are persisted.

**Acceptance Scenarios**:

1. **Given** no job categories exist, **When** I add a new category with name "EXECUTIVE", multiplier 2.0, and keywords ["CEO", "CFO"], **Then** the category is created and stored.
2. **Given** a job category exists, **When** I update its multiplier to 1.5, **Then** the change is saved.
3. **Given** a job category exists, **When** I delete it, **Then** it is removed from the system.

---

### User Story 2 - Identify Job Category (Priority: P1)

As a system, I want to identify the job category of a customer based on their jobTitle matching defined keywords so that categorization is automated and user-configurable.

**Why this priority**: This delivers the core value of the feature - automatic job category identification.

**Independent Test**: Can be fully tested by providing a customer jobTitle and verifying the correct category is returned based on keyword matches.

**Acceptance Scenarios**:

1. **Given** a customer with jobTitle "Chief Executive Officer", **When** the system identifies the category, **Then** it returns "EXECUTIVE" if keywords include "Chief".
2. **Given** a customer with jobTitle "Software Engineer", **When** the system identifies the category, **Then** it returns the appropriate category based on matching keywords.
3. **Given** a customer with jobTitle that doesn't match any keywords, **When** the system identifies the category, **Then** it returns a default or null category.

---

### Edge Cases

- What happens when multiple categories have overlapping keywords?
- How does system handle case sensitivity in jobTitle matching?
- What if jobTitle is null or empty?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow users to create new job categories with name, multiplier, and list of keywords.
- **FR-002**: System MUST allow users to update existing job categories.
- **FR-003**: System MUST allow users to delete job categories.
- **FR-004**: System MUST allow users to retrieve all job categories.
- **FR-005**: System MUST identify the job category for a customer by matching their jobTitle against the keywords of each category (case-insensitive substring match).
- **FR-006**: System MUST return the first matching category when multiple categories have matching keywords.
- **FR-007**: System MUST handle customers with no matching jobTitle by returning a default category or null.

### Key Entities *(include if feature involves data)*

- **JobCategory**: Represents a user-defined job category with attributes: name (string), multiplier (decimal), keywords (list of strings). Used to categorize customers based on jobTitle matching.
- **[Entity 2]**: [What it represents, relationships to other entities]

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can complete job category CRUD operations in under 30 seconds.
- **SC-002**: System correctly identifies job categories for 95% of test jobTitles.
- **SC-003**: Job category identification completes in under 100ms for typical jobTitle lengths.
- **SC-004**: System supports at least 100 job categories with up to 50 keywords each.

## Assumptions

- JobTitle matching is case-insensitive and uses substring matching (e.g., "Chief" matches "Chief Executive Officer").
- When multiple categories match, the system returns the first one found (no specific priority order).
- Multiplier is a decimal value used for risk calculation or scoring, but its exact application is defined in future features.
- Keywords are simple strings without regex support.
- Job categories are managed by system administrators via API.
