---
agent: 'agent'
description: 'Create an Architectural Decision Record (ADR) document for AI-optimized decision documentation.'
tools: ['changes', 'search/codebase', 'edit/editFiles', 'extensions', 'fetch', 'githubRepo', 'openSimpleBrowser', 'problems', 'runTasks', 'search', 'search/searchResults', 'runCommands/terminalLastCommand', 'runCommands/terminalSelection', 'testFailure', 'usages', 'vscodeAPI']
---
# Create Architectural Decision Record

Create an ADR document for `${input:DecisionTitle}` using structured formatting optimized for AI consumption and human readability.

## Inputs

- **Decision Title**: `${input:DecisionTitle}`
- **Context**: `${input:Context}`
- **Decision**: `${input:Decision}`
- **Consequences**: `${input:Consequences}`
- **Alternatives Considered**: `${input:Alternatives}`
- **Authors**: `${input:Authors}`

## Input Validation

If any of the required inputs are not provided or cannot be determined from the conversation history, ask the user to provide the missing information before proceeding with ADR generation.

## Requirements

- Use precise, unambiguous language
- Follow the standardized ADR template format with front matter
- Include both positive and negative consequences
- Document all alternatives with rejection rationale
- Structure for machine parsing and human reference
- Provide meaningful values for all front matter fields (`authors`, `applyTo`, `created`, `lastModified`, `state`, `instructions`)

The ADR must be saved in the `decisions/` directory using the naming convention: `YYYY-MM-DD-Title.md`, where the date is the creation date and the title uses hyphens instead of spaces (e.g., `2025-07-10-database-selection.md`).

## Required Documentation Structure

The documentation file must follow the structure defined in `./templates/adr.md`: