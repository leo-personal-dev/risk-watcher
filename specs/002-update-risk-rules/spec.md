# Feature Specification: Update Risk Rules

**Feature Branch**: `002-update-risk-rules`  
**Created**: 2026-04-25  
**Status**: Draft  
**Input**: User description: "The business rule to classify customers in clusters must satisfy the following requirements and must be executed in sequence based on priority number:

CLUSTER_A (Priority 1): Score >= 700 AND Age between 25 and 60 AND has_market_debt = false

CLUSTER_B (Priority 2): Score >= 500 AND Age between 18 and 65 AND NO credit_default OR loan_default IN market_debt_types

CLUSTER_C (Priority 3): Score >= 300

CLUSTER_D (Priority 4): All cases that not match in last clusters

The CLUSTER when defined must be structured with the following informations:

ID_CLUSTER: CLUSTER_A
NAME: Diamond
BASE_LIMIT: 50000
CAP: 100000

ID_CLUSTER: CLUSTER_B
NAME: Gold
BASE_LIMIT: 20000
CAP: 40000

ID_CLUSTER: CLUSTER_C
NAME: Silver
BASE_LIMIT: 5000
CAP: 10000

ID_CLUSTER: CLUSTER_D
NAME: Bronze
BASE_LIMIT: 0
CAP: 0"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Customer Risk Classification (Priority: P1)

As a credit risk specialist, I want to classify customers into risk clusters using updated business rules so that I can assess credit risk more accurately and make informed lending decisions.

**Why this priority**: This is the core functionality that addresses the primary problem of classifying customers with refined rules, enabling better risk assessment.

**Independent Test**: Can be fully tested by submitting customer data via API and verifying the returned risk cluster assignment matches the priority-based rules.

**Acceptance Scenarios**:

1. **Given** a customer with score >= 700, age between 25-60, and no market debt, **When** classification is performed, **Then** the customer is assigned to CLUSTER_A (Diamond).
2. **Given** a customer with score >= 500, age between 18-65, and no credit_default or loan_default in debt types, **When** classification is performed, **Then** the customer is assigned to CLUSTER_B (Gold).
3. **Given** a customer with score >= 300 that doesn't match higher priorities, **When** classification is performed, **Then** the customer is assigned to CLUSTER_C (Silver).
4. **Given** a customer that doesn't match any criteria, **When** classification is performed, **Then** the customer is assigned to CLUSTER_D (Bronze).

---

### User Story 2 - Personalized Credit Limit Calculation (Priority: P2)

As a credit risk specialist, I want to calculate personalized credit limits based on cluster information so that I can offer appropriate lending amounts based on the customer's risk profile and cluster limits.

**Why this priority**: Credit limit calculation is essential for operational lending decisions and directly impacts revenue through optimized loan offerings.

**Independent Test**: Can be tested by providing customer data, getting cluster assignment, then verifying the calculated credit limit falls within the cluster's BASE_LIMIT and CAP ranges.

**Acceptance Scenarios**:

1. **Given** a customer assigned to CLUSTER_A, **When** credit limit is calculated, **Then** the limit is set between BASE_LIMIT (50000) and CAP (100000).
2. **Given** a customer assigned to CLUSTER_B, **When** credit limit is calculated, **Then** the limit is set between BASE_LIMIT (20000) and CAP (40000).
3. **Given** a customer assigned to CLUSTER_C, **When** credit limit is calculated, **Then** the limit is set between BASE_LIMIT (5000) and CAP (10000).
4. **Given** a customer assigned to CLUSTER_D, **When** credit limit is calculated, **Then** the limit is set to BASE_LIMIT (0) with CAP (0).

---

### Edge Cases

- What happens when customer age is exactly 25, 60, 18, or 65 (boundary values)?
- How does system handle customers with score exactly 700, 500, or 300?
- What happens when market_debt_types contains both allowed and disallowed values?
- How does system handle customers with no debt types specified but has_market_debt = true?

## Requirements *(mandatory)*

<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right functional requirements.
-->

### Functional Requirements

- **FR-001**: System MUST classify customers into risk clusters using priority-based rules executed in sequence (Priority 1: CLUSTER_A for Score >= 700 AND Age 25-60 AND no market debt; Priority 2: CLUSTER_B for Score >= 500 AND Age 18-65 AND no credit_default/loan_default in debt types; Priority 3: CLUSTER_C for Score >= 300; Priority 4: CLUSTER_D for all other cases)
- **FR-002**: System MUST structure each cluster with ID_CLUSTER, NAME, BASE_LIMIT, and CAP (CLUSTER_A: Diamond, 50000-100000; CLUSTER_B: Gold, 20000-40000; CLUSTER_C: Silver, 5000-10000; CLUSTER_D: Bronze, 0-0)
- **FR-003**: System MUST calculate personalized credit limits within the assigned cluster's BASE_LIMIT and CAP range
- **FR-004**: System MUST return structured cluster information including calculated credit limit in API response

### Key Entities *(include if feature involves data)*

- **Customer**: Represents a customer with attributes score (numeric), age (numeric), has_market_debt (boolean), market_debt_types (array of strings)
- **Cluster**: Represents a risk cluster with attributes id_cluster (string), name (string), base_limit (numeric), cap (numeric)
- **ClassificationResult**: Represents the result of classification with cluster assignment and calculated credit limit

## Success Criteria *(mandatory)*

<!--
  ACTION REQUIRED: Define measurable success criteria.
  These must be technology-agnostic and measurable.
-->

### Measurable Outcomes

- **SC-001**: 100% of customer classifications match the priority-based business rules
- **SC-002**: API classification requests complete in under 100ms for 95% of requests
- **SC-003**: 100% of calculated credit limits fall within their cluster's BASE_LIMIT and CAP ranges
- **SC-004**: System maintains 99.9% uptime for classification endpoint
- **SC-005**: Credit risk specialists report improved decision-making confidence due to refined cluster rules

## Assumptions

<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right assumptions based on reasonable defaults
  chosen when the feature description did not specify certain details.
-->

- Existing customer data structure includes score, age, has_market_debt, and market_debt_types fields
- API endpoint structure remains compatible with existing integrations
- Credit limit calculation uses simple range-based approach within BASE_LIMIT and CAP
- Boundary values (exact ages 25, 60, 18, 65 and scores 700, 500, 300) are inclusive in the ranges
- market_debt_types is an array/list that can be checked for presence of credit_default or loan_default
