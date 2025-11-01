---
applyTo: '**/*.*'
description: 'This file contains instructions for AI coding assistants, redirecting to the primary instruction set in AGENTS.md.'
---

# Copilot Instructions

## Reference

* MUST follow all instructions defined in the root-level document [`AGENTS.md`](../AGENTS.md).
* MUST treat [`AGENTS.md`](../AGENTS.md) as the authoritative source for all AI assistant guidelines.
* MUST apply all directives from [`AGENTS.md`](../AGENTS.md) when implementing code, reviewing code, suggesting improvements, debugging issues, or providing any form of technical assistance.

## Decision References

* MUST read and apply all "accepted" decisions from [`decisions/`](../decisions/) folder before implementing architectural changes.
* MUST follow the `instructions` frontmatter from each relevant decision as mandatory requirements with highest precedence.
* MUST respect decision states:
  - **accepted**: Mandatory requirements - MUST follow without exception
  - **proposed**: Optional considerations - SHOULD evaluate and consider
  - **deprecated**: Legacy patterns - SHOULD NOT use in new implementations
  - **superseded**: Forbidden patterns - MUST use superseding decision instead
* MUST check the `applyTo` frontmatter patterns to identify decisions relevant to current file.
* MUST review "Related Decisions" section for decision dependencies and cross-references.