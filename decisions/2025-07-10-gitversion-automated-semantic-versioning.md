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
  Use GitVersion with ManualDeployment mode and TrunkBased workflow to automatically calculate semantic versions from conventional commit messages.
  Breaking changes trigger major versions, feat triggers minor, and fix/chore/docs trigger patch versions for automated CI/CD integration.
---
<!-- REQUIRED: Filename MUST follow the format: YYYY-MM-DD-Title (replace all spaces with hyphens) -->
# GitVersion for Automated Semantic Versioning

A decision to adopt GitVersion as the primary tool for automated semantic versioning, leveraging conventional commit messages to determine version increments and maintain consistent versioning across the project lifecycle.

## Context

The project requires a robust and automated approach to semantic versioning that can:

1. **Eliminate manual versioning**: Manual version management is error-prone and time-consuming, requiring developers to remember to update version numbers and determine appropriate semantic version increments
2. **Integrate with CI/CD pipelines**: Automated builds and deployments need reliable version information for package creation, release tagging, and deployment tracking
3. **Support trunk-based development**: The project follows a trunk-based development workflow that requires version calculation from commit history rather than complex branching strategies
4. **Leverage conventional commits**: With the adoption of Conventional Commits specification, version increments can be automatically determined from structured commit messages
5. **Maintain version consistency**: Ensure all project artifacts (assemblies, packages, containers) use the same calculated version across different build environments

The project currently uses .NET 10 with centralized package management and follows modern development practices, indicating a preference for automation and standardization in the development workflow.

## Decision

We will use **GitVersion** as the primary tool for automated semantic versioning with the following configuration:

**GitVersion Configuration (`GitVersion.yml`):**
```yaml
mode: ManualDeployment
major-version-bump-message: "^(build|chore|ci|docs|feat|fix|perf|refactor|revert|style|test)(\\([\\w\\s-,/\\\\]*\\))?(!:|:.*\\n\\n((.+\\n)+\\n)?BREAKING CHANGE:\\s.+)"
minor-version-bump-message: "^(feat)(\\([\\w\\s-,/\\\\]*\\))?:"
patch-version-bump-message: "^(build|chore|ci|docs|fix|perf|refactor|revert|style|test)(\\([\\w\\s-,/\\\\]*\\))?:"
workflow: TrunkBased/preview1
```

**Implementation Details:**
- **Mode**: `ManualDeployment` - Provides full control over when versions are incremented and tagged
- **Workflow**: `TrunkBased/preview1` - Optimized for trunk-based development with feature previews
- **MSBuild Integration**: `GitVersion.MsBuild` package automatically injects version information into all projects during build
- **Conventional Commits Integration**: Version bump rules are configured to parse conventional commit messages and determine appropriate semantic version increments
- **Target Framework Workaround**: Temporary configuration to use `net9.0` for GitVersion processing until full .NET 10 support is available

**Version Increment Rules:**
- **Major Version**: Triggered by breaking changes (commits with `!` or `BREAKING CHANGE:` footer)
- **Minor Version**: Triggered by `feat:` commits (new features)
- **Patch Version**: Triggered by `fix:`, `build:`, `chore:`, `ci:`, `docs:`, `perf:`, `refactor:`, `revert:`, `style:`, `test:` commits

**MSBuild Integration:**
- GitVersion.MsBuild automatically provides version properties to all projects
- Version information is embedded in assemblies, packages, and other artifacts
- No manual version management required in project files

## Consequences

**Positive Consequences:**
1. **Automated version calculation**: Version numbers are automatically calculated from git history and conventional commit messages, eliminating manual version management
2. **Consistent versioning**: All project artifacts automatically use the same calculated version, ensuring consistency across builds and deployments
3. **CI/CD integration**: Build pipelines can reliably access version information for package creation, tagging, and deployment processes
4. **Semantic versioning compliance**: Automatic adherence to semantic versioning principles based on the nature of changes (breaking, feature, fix)
5. **Developer productivity**: Developers focus on code changes rather than version management, reducing cognitive load and potential errors
6. **Release automation**: Enables automated release processes with proper version tagging and changelog generation
7. **Audit trail**: Version increments are directly traceable to specific commits and their semantic meaning

**Potential Challenges:**
1. **Learning curve**: Developers need to understand how conventional commits impact version calculation
2. **Commit message discipline**: Incorrect commit message formats can lead to inappropriate version increments
3. **Build complexity**: Additional dependency in the build process that requires understanding for troubleshooting
4. **Preview versions**: Pre-release and preview version handling requires careful configuration and understanding
5. **Rollback scenarios**: Complex version rollback situations may require manual intervention

**Risk Mitigation:**
- Implement commit message validation to ensure proper conventional commit format
- Provide clear documentation and training on conventional commits and their version impact
- Configure appropriate branch and tag patterns for different deployment scenarios
- Establish clear guidelines for handling manual version corrections when necessary
- Monitor version calculations in CI/CD pipelines to catch unexpected increments

## Alternatives Considered

### 1. Manual Version Management

**Description**: Manually manage version numbers in project files and update them with each release.

**Pros**: Full control over versioning, simple to understand, no additional dependencies

**Cons**: Error-prone, time-consuming, inconsistent across team members, no automation benefits, requires manual coordination

**Rejection Reason**: Does not align with project goals for automation and standardization, prone to human error

### 2. Date-based Versioning

**Description**: Use date and time stamps or sequential build numbers for versioning.

**Pros**: Simple to implement, automatically incremental, no semantic meaning required

**Cons**: No semantic meaning, difficult to understand impact of changes, does not follow semantic versioning principles, poor developer experience

**Rejection Reason**: Does not provide semantic information about the nature of changes, incompatible with semantic versioning best practices

### 3. Custom Versioning Scripts

**Description**: Develop custom PowerShell or batch scripts to calculate versions based on git tags and commits.

**Pros**: Complete control over versioning logic, can be tailored to specific project needs

**Cons**: Additional maintenance burden, requires expertise to implement correctly, potential for bugs, reinventing existing solutions

**Rejection Reason**: GitVersion is a mature, well-tested solution that provides the same functionality without custom development overhead

### 4. NerdBank.GitVersioning

**Description**: Alternative automated versioning tool with similar capabilities to GitVersion.

**Pros**: Similar automation benefits, different configuration approach, Microsoft ecosystem alignment

**Cons**: Different syntax and configuration model, smaller community, less conventional commit integration

**Rejection Reason**: GitVersion has better integration with conventional commits and broader ecosystem support

### 5. Azure DevOps Build Variables

**Description**: Use Azure DevOps or other CI/CD platform built-in versioning capabilities.

**Pros**: Platform integration, simple configuration, build-specific versioning

**Cons**: Platform-dependent, limited semantic versioning support, no local development version consistency, vendor lock-in

**Rejection Reason**: Lacks semantic versioning automation and creates dependency on specific CI/CD platform

## Related Decisions

- [Conventional Commits](./2025-07-10-conventional-commits.md) - GitVersion configuration is specifically designed to integrate with the adopted Conventional Commits specification, using commit message patterns to determine appropriate version increments
- [Centralized Package Version Management](./2025-07-10-centralized-package-version-management.md) - GitVersion works seamlessly with centralized package management, providing automated version information that eliminates the need for manual version specifications in Directory.Packages.props
