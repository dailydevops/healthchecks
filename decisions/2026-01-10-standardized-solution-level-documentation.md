---
authors:
  - Martin Stühmer

applyTo:
  - "README.md"
  - "templates/readme-solution.md"

created: 2026-01-10

lastModified: 2026-01-10

state: accepted

instructions: |
  MUST use the standardized solution-level README template from templates/readme-solution.md for all repository-level documentation.
  MUST preserve existing content markers (e.g., <!-- packages:start -->) when modernizing existing README files.
  MUST include all required sections: Overview, Projects, Features, Getting Started, Development, Architecture, Contributing, Code of Conduct, Documentation, Versioning, Support, and License.
  MUST use relative links for all internal repository references (CONTRIBUTING.md, CODE_OF_CONDUCT.md, LICENSE, decisions/).
---

# Decision: Standardized Solution-Level Documentation

This decision establishes a standardized template for solution-level README files to ensure consistency, maintainability, and comprehensive documentation across all repositories.

## Context

Repository README files serve as the primary entry point for developers, contributors, and users. Without a standardized approach, documentation quality, structure, and completeness vary significantly across projects. This inconsistency creates several challenges:

- New contributors struggle to understand project structure and getting started procedures
- Essential information (contributing guidelines, architecture decisions, support channels) is inconsistently documented or missing
- Modernizing documentation requires significant effort due to lack of common structure
- Cross-referencing between documentation files (CONTRIBUTING.md, CODE_OF_CONDUCT.md, ADRs) is inconsistent
- Maintenance burden increases as each repository uses different documentation patterns

The need for a standardized, maintainable template became apparent as repositories grew and required regular documentation updates. A template-based approach enables consistent structure while preserving repository-specific content and custom markers.

## Decision

Adopt a standardized template for solution-level README files (`templates/readme-solution.md`) with specific sections, formatting conventions, and comprehensive inline guidance.

The template includes:

1. **Standard Structure**: Overview, Projects, Features, Getting Started, Development, Architecture, Contributing, Code of Conduct, Documentation, Versioning, Support, and License sections
2. **Relative Links**: All internal references use relative paths (e.g., `[CONTRIBUTING.md](CONTRIBUTING.md)`)
3. **Inline Guidance**: HTML comments provide detailed instructions for each section
4. **Badge Support**: Standardized badges for license, build status, and contributors
5. **Placeholder Patterns**: Consistent use of `YourRepo`, `ProjectName`, and similar placeholders for easy customization
6. **Flexible Structure**: Optional sections (Acknowledgments, Configuration) can be removed if not needed
7. **Marker Preservation**: Support for preserving existing content markers (e.g., `<!-- packages:start -->`, `<!-- packages:end -->`) during modernization
8. **Support Communication**: Directs users to GitHub Issues as the central point of contact (GitHub Discussions is not used)
9. **GitVersion Integration**: Documents the use of automated semantic versioning
10. **Conventional Commits Reference**: Links to commit message standards
11. **ADR Integration**: References Architecture Decision Records with relative links to `decisions/` folder

When modernizing existing README files:

- MUST preserve all existing content markers (comments like `<!-- packages:start -->`)
- MUST retain existing content where applicable while restructuring to match template sections
- MUST maintain any custom sections that provide repository-specific value
- MUST update links to use relative paths consistently

## Consequences

### Positive

- **Consistency**: All repositories follow the same documentation structure, improving navigation and comprehension
- **Completeness**: Template ensures all essential sections are present and documented
- **Maintainability**: Centralized template allows updating documentation standards across all repositories
- **Onboarding**: New contributors can quickly understand project structure, setup, and contribution process
- **Discoverability**: Standard sections make it easier to find specific information (architecture, testing, support)
- **Cross-referencing**: Relative links create a cohesive documentation ecosystem connecting README, CONTRIBUTING.md, CODE_OF_CONDUCT.md, and ADRs
- **Automation-friendly**: Consistent structure enables automated documentation checks and updates
- **Backward Compatibility**: Marker preservation ensures existing automation and tooling continues to work during modernization

### Negative

- **Initial Effort**: Existing repositories require one-time modernization effort to adopt the template
- **Customization Constraints**: Highly specialized repositories may need to adapt template sections
- **Maintenance Overhead**: Template updates may require propagating changes to all repositories using it
- **Learning Curve**: Contributors need to familiarize themselves with template structure and placeholder patterns

### Trade-offs

- Prioritizes consistency over complete flexibility in documentation structure
- Requires initial time investment for long-term maintainability gains
- May include sections not relevant to all repository types (can be removed as documented)

## Alternatives Considered

### No Template / Freeform Documentation

**Description**: Allow each repository to define its own README structure without standardization.

**Pros**:

- Maximum flexibility for repository-specific needs
- No migration effort required
- Each team can optimize for their specific use case

**Cons**:

- Inconsistent documentation quality and completeness across repositories
- Higher maintenance burden as each repository evolves independently
- Poor discoverability for users navigating multiple repositories
- Difficult to enforce documentation standards
- No economies of scale for documentation improvements

**Rejection Rationale**: The cost of inconsistency and maintenance burden outweighs the flexibility benefits. Standardization improves the experience for all stakeholders.

### Wiki-Based Documentation

**Description**: Use GitHub Wiki or external documentation platforms instead of README files.

**Pros**:

- More powerful formatting and organization capabilities
- Separate documentation from code repository
- Easier to maintain large documentation sets

**Cons**:

- Separate from codebase, harder to keep synchronized
- Not visible in repository root for quick reference
- Requires additional access management
- Less portable (tied to specific platform)
- README files remain necessary for initial overview

**Rejection Rationale**: README files are essential for repository discoverability and initial orientation. Wiki can complement but not replace solution-level README.

### Minimal README with External Links

**Description**: Keep README minimal and link to external documentation sites.

**Pros**:

- Reduced repository clutter
- More sophisticated documentation tooling available
- Better version control for documentation history

**Cons**:

- Requires maintaining separate documentation infrastructure
- Higher barrier to contribution (separate system to learn)
- Documentation less accessible for offline or restricted environments
- External dependencies for basic project information

**Rejection Rationale**: Essential information should be readily available in the repository. External documentation can supplement but not replace core README content.

## Related Decisions

- [Folder Structure and Naming Conventions](./2025-07-10-folder-structure-and-naming-conventions.md) - Defines where documentation and templates are organized
- [English as Project Language](./2025-07-11-english-as-project-language.md) - Establishes language requirements for all documentation
- [GitVersion Automated Semantic Versioning](./2025-07-10-gitversion-automated-semantic-versioning.md) - Referenced in the Versioning section of the template
- [Conventional Commits](./2025-07-10-conventional-commits.md) - Referenced in Contributing and Versioning sections
- [No GitHub Discussions for Communication](./2025-11-01-no-github-discussions-for-communication.md) - GitHub Issues serve as the central point of contact for all project discussions
