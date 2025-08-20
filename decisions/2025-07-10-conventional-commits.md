<!-- List of authors who contributed to this decision. Include full names and roles if applicable. -->
authors:
- Martin Stühmer

<!--
The patterns this decision applies to. Each entry is a glob pattern that matches files affected by this decision.
-->
applyTo:
- "**/*.*"

<!-- The date this ADR was initially created in YYYY-MM-DD format. -->
created: 2025-07-10

<!--
The most recent date this ADR was updated in YYYY-MM-DD format.
IMPORTANT: Update this field whenever the decision is modified.
-->
lastModified: 2025-07-14

<!--
The current state of this ADR. If superseded, include references to the superseding ADR.
Valid values: proposed, accepted, deprecated, superseded
-->
state: accepted

<!--
A compact AI LLM compatible definition of this decision.
This should be a precise, structured description that AI systems can easily parse and understand.
Include the core decision, key rationale, and primary impact in 1-2 concise sentences.
-->
instructions: |
  Adopt Conventional Commits 1.0.0 specification for structured commit messages with type, scope, and description format to enable automated semantic versioning and changelog generation.
  Required types include feat, fix, docs, style, refactor, test, chore, build, ci, perf, and revert with breaking change indicators.
---
<!-- REQUIRED: Filename MUST follow the format: YYYY-MM-DD-Title (replace all spaces with hyphens) -->
# Conventional Commits

A decision to adopt the Conventional Commits specification for standardizing commit messages across the project to improve automation, semantic versioning, and change communication.

## Context

The project currently lacks a standardized approach to commit messages, which creates several challenges:

1. **Inconsistent commit history**: Developers use different styles and formats for commit messages, making it difficult to understand the nature and impact of changes
2. **Manual versioning**: Without structured commit messages, determining semantic version bumps requires manual analysis of changes
3. **Changelog generation**: Creating release notes and changelogs is a manual process that is time-consuming and error-prone
4. **Automated tooling limitations**: The lack of structured commit messages prevents the use of automated tools for CI/CD, semantic versioning, and release management
5. **Communication gaps**: Unclear commit messages make it difficult for team members and stakeholders to understand the impact of changes

The project already uses GitVersion (as evidenced by `GitVersion.MsBuild` in `Directory.Packages.props`) and follows centralized package management, indicating a preference for automation and structured development practices.

## Decision

We will adopt the [Conventional Commits 1.0.0 specification](https://www.conventionalcommits.org/en/v1.0.0/) for all commit messages in this project.

**Commit Message Structure:**
```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

**Required Types:**
- `feat`: A new feature (correlates with MINOR in Semantic Versioning)
- `fix`: A bug fix (correlates with PATCH in Semantic Versioning)
- `BREAKING CHANGE`: Breaking API changes (correlates with MAJOR in Semantic Versioning)

**Recommended Additional Types:**
- `build`: Changes to build system or external dependencies
- `chore`: Maintenance tasks that don't modify src or test files
- `ci`: Changes to CI configuration files and scripts
- `docs`: Documentation changes
- `style`: Code style changes (formatting, missing semicolons, etc.)
- `refactor`: Code changes that neither fix bugs nor add features
- `perf`: Performance improvements
- `test`: Adding or modifying tests

**Implementation Guidelines:**
- Commits MUST be prefixed with a type followed by an optional scope, optional `!`, and required terminal colon and space
- Breaking changes MUST be indicated either by `!` after the type/scope or by including `BREAKING CHANGE:` in the footer
- Scope MAY be provided to give additional context (e.g., `feat(api):`, `fix(parser):`)
- Description MUST immediately follow the colon and space after the type/scope prefix
- Body and footers are OPTIONAL and provide additional context when needed

## Consequences

**Positive Consequences:**
1. **Automated versioning**: GitVersion and other tools can automatically determine semantic version bumps based on commit types
2. **Automated changelog generation**: Tools can automatically generate changelogs and release notes from structured commit messages
3. **Improved communication**: Clear, structured commit messages help team members and stakeholders understand the nature and impact of changes
4. **Better CI/CD integration**: Automated tools can trigger appropriate build and deployment processes based on commit types
5. **Consistent commit history**: Standardized format makes the project history more readable and searchable
6. **Enhanced collaboration**: New contributors can quickly understand the commit convention and follow established patterns

**Potential Challenges:**
1. **Learning curve**: Team members need to learn and remember the conventional commit format
2. **Enforcement overhead**: Initial effort required to establish tooling and processes to enforce the convention
3. **Commit message length**: Some developers may need to adjust to writing more descriptive commit messages
4. **Retroactive application**: Existing commit history will not follow the convention, creating inconsistency in historical data

**Risk Mitigation:**
- Implement commit message linting tools to validate format automatically
- Provide training and documentation for team members
- Use commit message templates in development environments
- Consider squash-and-merge workflow for pull requests to allow maintainers to create properly formatted commit messages

## Alternatives Considered

### 1. Custom Commit Convention

**Description**: Develop a project-specific commit message convention tailored to our needs.

**Pros**: Complete control over format and requirements, can be optimized for specific project needs

**Cons**: Requires documentation and training, no existing tooling support, reinventing the wheel, lacks community standards

**Rejection Reason**: Conventional Commits is already a well-established standard with extensive tooling support

### 2. Continue with Free-form Commit Messages

**Description**: Maintain the current approach of allowing developers to write commit messages without specific formatting requirements.

**Pros**: No learning curve, no enforcement overhead, developer freedom

**Cons**: Inconsistent history, no automation benefits, poor communication, manual versioning and changelog generation

**Rejection Reason**: Does not address the identified problems and prevents automation benefits

### 3. GitHub Flow with Descriptive PR Titles

**Description**: Focus on descriptive pull request titles and use squash merging to create clean commit messages.

**Pros**: Reduces individual commit message burden, maintains clean main branch history

**Cons**: Still lacks standardization, no automation benefits, requires manual formatting of squash commit messages

**Rejection Reason**: Does not provide the structured format needed for automation tools

### 4. Semantic Commit Messages (Alternative Format)

**Description**: Use alternative structured commit formats like Angular commit convention or other semantic formats.

**Pros**: Similar benefits to Conventional Commits, some tooling support

**Cons**: Less standardized than Conventional Commits, smaller ecosystem, potential compatibility issues

**Rejection Reason**: Conventional Commits is more widely adopted and has better tooling ecosystem

## Related Decisions

- [Centralized Package Version Management](./2025-07-10-centralized-package-version-management.md) - The adoption of centralized package management demonstrates the project's commitment to automation and standardization, which aligns with adopting Conventional Commits for automated versioning and release management
- [GitVersion for Automated Semantic Versioning](./2025-07-10-gitversion-automated-semantic-versioning.md) - GitVersion is configured to parse conventional commit messages and automatically determine semantic version increments, making conventional commits essential for automated versioning to work correctly
