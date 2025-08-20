<!-- List of authors who contributed to this decision. Include full names and roles if applicable. -->
authors:
- Name Surname <!-- Replace with actual name -->
- Another Name Surname <!-- Add more authors as needed -->

<!--
The patterns this decision applies to. Each entry is a glob pattern that matches files affected by this decision.
Example:
applyTo:
- "**/*.cs"          # Applies to all C# files
- "src/**/*.razor"   # Applies to all Blazor components in src folder
- "tests/**/*.sql"   # Applies to all SQL files in tests folder
-->
applyTo:
- "**/*" <!-- Replace with specific glob patterns -->

<!-- The date this ADR was initially created in YYYY-MM-DD format. -->
created: YYYY-MM-DD

<!--
The most recent date this ADR was updated in YYYY-MM-DD format.
IMPORTANT: Update this field whenever the decision is modified.
-->
lastModified: YYYY-MM-DD

<!--
The current state of this ADR. If superseded, include references to the superseding ADR.
Valid values: proposed, accepted, deprecated, superseded
-->
state: proposed

<!--
A compact AI LLM compatible definition of this decision.
This should be a precise, structured description that AI systems can easily parse and understand.
Include the core decision, key rationale, and primary impact in 1-2 concise sentences.
-->
instructions: |
  Compact definition of the decision made and its core purpose.
  Key rationale and primary impact on the project or development process.
---
<!-- REQUIRED: Filename MUST follow the format: YYYY-MM-DD-Title (replace all spaces with hyphens) -->
# Title <!-- A concise title that summarizes the decision. Use a format like "Decision: [Short Description of Decision]". -->

<!--
A brief summary of the decision. This should be a short paragraph that captures the essence of the decision made.
-->

## Context

<!--
Provide a detailed explanation of the problem or issue that led to this decision. Include background information, constraints, and any relevant context to help readers understand why this decision was necessary.
-->

## Decision

<!--
Clearly state the decision made. Describe the chosen solution or approach in detail, including any specific technologies, tools, or methods involved.
-->

## Consequences

<!--
Explain the implications of this decision. What are the expected benefits, trade-offs, and potential risks? How will this decision impact the project or organization?
-->

## Alternatives Considered

<!--
List and describe other options that were considered. For each alternative, explain why it was not chosen. Include pros and cons, feasibility, and any other relevant factors.
-->

## Related Decisions (Optional)

<!--
Provide links or references to other ADRs that are related to this decision. Explain how they are connected and why they are relevant.
Use markdown link syntax to reference other decisions: [Decision Title](./YYYY-MM-DD-decision-filename.md)
If there are no related decisions, this section may be omitted or include a note stating "None at this time."

Example:
- [Centralized Package Version Management](./2025-07-10-centralized-package-version-management.md) - Related because this decision impacts how we manage dependencies
- [Conventional Commits](./2025-07-10-conventional-commits.md) - This decision affects our commit message format which impacts versioning
-->
