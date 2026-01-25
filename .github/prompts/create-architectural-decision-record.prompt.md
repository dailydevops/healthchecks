---
agent: 'agent'
description: 'Create an Architectural Decision Record (ADR) document optimized for decision documentation.'
tools: ['vscode', 'execute', 'read', 'edit', 'search', 'web', 'deepwiki/*', 'fetch/*', 'microsoft-docs/*', 'nuget-server/*', 'agent', 'todo']
---
# Create Architectural Decision Record

Create an ADR document for `${input:DecisionTitle}` using structured formatting optimized for machine parsing and human readability.

## Inputs

* **Decision Title**: `${input:DecisionTitle}`
* **Context**: `${input:Context}`
* **Decision**: `${input:Decision}`
* **Consequences**: `${input:Consequences}`
* **Alternatives Considered**: `${input:Alternatives}`
* **Authors**: `${input:Authors}`

## Input Validation

* MUST request missing information from the user if any required inputs are not provided or cannot be determined from conversation history.
* MUST obtain all required inputs before proceeding with ADR generation.

## Requirements

* MUST use precise, unambiguous language.
* MUST follow the standardized ADR template format with front matter.
* MUST include both positive and negative consequences.
* MUST document all alternatives with rejection rationale.
* MUST structure the document for machine parsing and human reference.
* MUST provide meaningful values for all front matter fields (`authors`, `applyTo`, `created`, `lastModified`, `state`, `instructions`).
* MUST save the ADR in the `decisions/` directory using the naming convention: `YYYY-MM-DD-Title.md`, where the date is the creation date and the title uses hyphens instead of spaces (for example, `2025-07-10-database-selection.md`).

## Required Documentation Structure

The documentation file MUST follow the structure defined in `./templates/adr.md`: