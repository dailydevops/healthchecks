---
applyTo: '**/*.*'
description: 'Instructions for interpreting and applying architectural decisions from the decisions/ folder.'
---

# Decision Handling Instructions

This file provides guidance on how AI coding assistants should discover, interpret, and apply architectural decision records (ADRs) from the `decisions/` folder.

## Decision Discovery

* MUST scan `decisions/` folder for relevant ADRs before making architectural or structural changes.
* MUST prioritize decisions based on their `applyTo` glob patterns matching the current file path.
* MUST read the complete decision document, not just the `instructions` frontmatter, when making complex architectural changes.
* SHOULD use semantic search to find decisions related to the current task when file patterns don't provide clear matches.

## Decision Application Priority

Decisions MUST be applied in the following priority order:

1. **`instructions` frontmatter**: Primary AI guidance - compact, actionable rules
2. **Decision section**: Detailed implementation requirements and constraints
3. **Consequences section**: Understanding of trade-offs and implications
4. **Related Decisions**: Dependencies and cross-cutting concerns

## Decision States

Each decision has a `state` frontmatter field that determines how it MUST be treated:

### accepted (Mandatory)
* MUST follow all requirements without exception
* MUST treat as non-negotiable architectural constraints
* MUST apply `instructions` frontmatter as highest-priority rules
* MUST NOT deviate unless explicitly overridden by user request

### proposed (Optional)
* SHOULD consider as recommendations and best practices
* MAY apply based on context and user requirements
* SHOULD discuss with user before implementing significant proposed patterns
* MUST NOT treat as mandatory requirements

### deprecated (Legacy)
* SHOULD NOT use in new implementations
* MAY remain in existing code until refactored
* SHOULD suggest alternatives when encountered
* MUST warn user when deprecated patterns are detected

### superseded (Forbidden)
* MUST NOT use in any circumstances
* MUST use the superseding decision referenced in the document
* MUST refactor existing code using superseded patterns when encountered
* SHOULD inform user about the superseding decision

## Decision Metadata

Each decision contains frontmatter metadata that MUST be used for proper application:

* **`applyTo`**: Glob patterns indicating which files the decision affects
* **`state`**: Current status (accepted, proposed, deprecated, superseded)
* **`instructions`**: Compact AI guidance for immediate application
* **`created`**: Original decision date (informational)
* **`lastModified`**: Most recent update date (MUST check for currency)
* **`authors`**: Decision contributors (informational)

## Workflow for Code Changes

When making code changes, follow this workflow:

1. **Identify Affected Files**: Determine which files will be modified
2. **Match Decision Patterns**: Find decisions with `applyTo` patterns matching those files
3. **Check Decision State**: Verify state (only "accepted" decisions are mandatory)
4. **Read Instructions**: Extract and parse the `instructions` frontmatter
5. **Apply Requirements**: Implement code following the decision requirements
6. **Verify Compliance**: Ensure changes align with all applicable decisions
7. **Check Related Decisions**: Review linked decisions for dependencies

## Examples

### Example 1: Creating a New Test Project

**File to create**: `tests/Core.Tests.Unit/UserServiceTests.cs`

**Relevant Decisions**:
1. `2025-07-10-folder-structure-and-naming-conventions.md` (matches `**/*.*`)
2. `2025-07-11-english-as-project-language.md` (matches `**/*.*`)

**Applied Instructions**:
- Test project MUST be named `Core.Tests.Unit`
- Test project MUST be placed under `tests/` folder
- All code, comments, and identifiers MUST be in English

### Example 2: Adding a NuGet Package

**File to modify**: `Directory.Packages.props`

**Relevant Decisions**:
1. `2025-07-10-centralized-package-version-management.md` (matches `Directory.Packages.props`)
2. `2025-07-10-dependabot-automated-dependency-updates.md` (matches `Directory.Packages.props`)

**Applied Instructions**:
- Package version MUST be defined in `Directory.Packages.props`
- Package reference in project file MUST NOT include version attribute
- Dependabot will automatically manage updates

### Example 3: Making a Commit

**Context**: All commit operations

**Relevant Decisions**:
1. `2025-07-10-conventional-commits.md` (matches `**/*.*`)
2. `2025-07-11-english-as-project-language.md` (matches `**/*.*`)

**Applied Instructions**:
- Commit message MUST follow Conventional Commits 1.0.0 format
- Commit message MUST be in English
- Type MUST be one of: feat, fix, docs, style, refactor, test, chore, build, ci, perf, revert
- Breaking changes MUST be indicated with `!` or `BREAKING CHANGE:` footer

## Related Decisions Handling

When a decision references related decisions:

* MUST read all related decisions to understand full context
* MUST apply requirements from all related decisions that match current file
* MUST respect decision dependency chains (if Decision A depends on B, read B first)
* SHOULD inform user about decision dependencies when making significant changes

## Conflict Resolution

If decisions appear to conflict:

1. **Check states**: "accepted" decisions override "proposed" ones
2. **Check dates**: More recent decisions (`lastModified`) take precedence
3. **Check specificity**: More specific `applyTo` patterns override generic ones
4. **Decisions vs. Instructions**: Accepted decisions from `decisions/` folder override technical instructions from `.github/instructions/` when both apply to the same situation
5. **Ask user**: If conflict remains unclear, request user clarification

## Decision Updates

When suggesting changes to decisions:

* MUST update the `lastModified` field to current date
* MUST update the `instructions` frontmatter if core guidance changes
* MUST maintain backward compatibility notes if changing "accepted" decisions
* SHOULD add to "Related Decisions" if new dependencies are introduced
