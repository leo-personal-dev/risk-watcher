

**Tools**: Github Copilot + Spec-Driven Development approach

# Prompts

In following sections are an example of a full cycle to define specifications, plan, taks, and implementation using spec-driven development in VS Code copilot.
This approach was used to refine functionalities, generate the source code, and tests.

## Generate the constitution file:

Input:
```text
Fill the constitution file with the bare minimum requirements for a dotnet core api based on the template
```

Output:
```text
Fill the constitution file based on the instructions
```

## Generate the specifications (the what and why of the api):

Input:
```text
/speckit.specify The product is called risk watcher. The customer segment of the product is credit risk department in banks. In this segment the credit risk director is the customer, customer facing products are users and credit risk specialists are users. The existing alternative considered is the product FICO Score. The problems to be solved with the product are classify customers in risk clusters, calculate personalized credit limits and estimate monthly incomes. The solution is use a data-driven classification engine. The value proposition is reliable and flexible product to manage credit risk rules.  The channels are the risk watcher api and risk watcher website. The key metrics considered are response time less than 500ms and zero engineering effort to manage classification rules. The revenue stream is credit risk classification service. The cost structure are aws insfrastructure and service operacional team.
```

Output:
```text
Fill the spec.md file with the instructions
```

## Generate the solution plan (the how of the api)

Input:
```text
/speckit.plan Create the api channel and consider the use of dotnet core in last version. Dont integrate the api with databases, use mocks if necessary. The api must have only one endpoint that is POST /customers/classify. The body of this endpoint contains the customer data that have id (string), name (string), age (integer), score (integer from 0 to 1000), has_market_debt (boolean), market_debt_types (string[] with the possible values: credit_card, personal_loan, mortgage, credit_default, loan_default), location.city (string), location.state (string with the following examples: SP, RJ, MG, MA, RS, ...), location.region (string with the possible values: Norte, Nordeste, Centro-Oeste, Sudeste, Sul), job_title (string). The response must have the same data received in the request and enriched with the cluster (string with the possible values: CLUSTER_A, CLUSTER_B, CLUSTER_C, CLUSTER_D) derived from business rules. Organize the api with a dotnet solution that contains threee projects: watcher-api, watcher-domain, and watcher-infrastructure.
```

Output:
```text
Fill the plan.md file with the instructions
```

## Generate the tasks to implement the plan to achieve the specifications

Input:
```text
/speckit.tasks break the plan into tasks
```

Output:
```text
Fill the tasks.md file with the instructions
```

## Implement the tasks to achieve the specifications

Input:
```text
Implement the tasks for this project, and update the task list as you go
```

Output:
```text
Generate the RiskWatcher.sln and /src folder with the source code to implement the plan to achieve the specifications
```