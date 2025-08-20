<!-- List of authors who contributed to this decision. Include full names and roles if applicable. -->
authors:
- Martin Stühmer

<!--
The patterns this decision applies to. Each entry is a glob pattern that matches files affected by this decision.
-->
applyTo:
- "**/*.*"

<!-- The date this ADR was initially created in YYYY-MM-DD format. -->
created: 2025-07-11

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
  Establish English as the mandatory language for all code, documentation, comments, commit messages, and written content to ensure consistency and global accessibility.
  Applies to all identifiers, configuration files, database objects, and communication using clear, professional English standards.
---
<!-- REQUIRED: Filename MUST follow the format: YYYY-MM-DD-Title (replace all spaces with hyphens) -->
# English as Project Language

A decision to establish English as the primary and mandatory language for all code, documentation, and written content within the project to ensure consistency, professionalism, and global accessibility.

## Context

Software development projects often face challenges with language consistency, especially in international teams or when targeting global audiences. Mixed language usage in codebases can lead to several issues:

1. **Inconsistent Communication**: Using multiple languages creates confusion and reduces readability for team members who may not be fluent in all languages used
2. **Maintenance Complexity**: Mixed language comments, documentation, and variable names make code maintenance more difficult
3. **Global Collaboration**: Non-English content can exclude potential contributors and limit the project's reach
4. **Professional Standards**: Industry best practices favor English as the standard language for software development
5. **Tool Compatibility**: Many development tools, IDEs, and automated systems work better with English content
6. **Documentation Accessibility**: English documentation ensures broader accessibility for users and contributors worldwide

## Decision

All code, documentation, comments, and other written content in this project MUST be written in English. This includes:

- **Source Code**: All variable names, function names, class names, method names, and other identifiers
- **Code Comments**: All inline comments, documentation comments, and code annotations
- **Documentation**: README files, API documentation, user guides, and technical specifications
- **Commit Messages**: All commit messages must follow the established Conventional Commits format in English
- **Issue Tracking**: GitHub issues, pull request descriptions, and discussions
- **Configuration Files**: Comments and descriptions in configuration files
- **Database Objects**: Table names, column names, stored procedure names, and database documentation

The English used should be:
- **Professional**: Use clear, concise, and professional language
- **Consistent**: Follow established naming conventions and terminology
- **Accessible**: Avoid unnecessary jargon and use plain English where possible

## Consequences

### Benefits

1. **Improved Collaboration**: Team members from different linguistic backgrounds can effectively collaborate
2. **Enhanced Maintainability**: Consistent language usage makes code easier to read, understand, and maintain
3. **Global Accessibility**: English content ensures the project is accessible to the widest possible audience
4. **Professional Standards**: Aligns with industry best practices and professional development standards
5. **Tool Integration**: Better compatibility with development tools, linters, and automated systems
6. **Knowledge Transfer**: Easier onboarding of new team members regardless of their native language
7. **Open Source Readiness**: Prepares the project for potential open-source contributions

### Trade-offs

1. **Learning Curve**: Team members who are not native English speakers may need additional time to express complex technical concepts
2. **Translation Overhead**: Existing non-English content will need to be translated during migration
3. **Cultural Context**: Some domain-specific terms or concepts may lose nuance when translated to English

### Risks

1. **Initial Productivity Impact**: Short-term productivity decrease while team members adapt to English-only communication
2. **Quality Concerns**: Non-native English speakers may initially produce less polished documentation or comments

## Alternatives Considered

### Multi-language Approach
- **Description**: Allow multiple languages based on team member preferences or regional requirements
- **Rejected Because**: Creates inconsistency, reduces maintainability, and limits global collaboration

### Native Language with English Translation
- **Description**: Write in native language first, then translate to English
- **Rejected Because**: Increases maintenance overhead, creates potential for translation errors, and delays development

### Gradual Migration
- **Description**: Slowly transition to English over time without enforcing immediate compliance
- **Rejected Because**: Would result in inconsistent codebase for extended periods and may never achieve full compliance

## Related Decisions

- [Conventional Commits](./2025-07-10-conventional-commits.md) - This decision affects commit message language, which must now be in English following the conventional format
- [Folder Structure and Naming Conventions](./2025-07-10-folder-structure-and-naming-conventions.md) - All folder and file naming conventions must now follow English terminology
