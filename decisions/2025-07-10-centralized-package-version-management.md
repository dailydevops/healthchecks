---
authors:
  - Martin Stühmer

applyTo:
  - "Directory.Packages.props"
  - "**/*.csproj"
  - "**/*.fsproj"
  - "**/*.vbproj"

created: 2025-07-10

lastModified: 2025-07-14

state: accepted

instructions: |
  Enable centralized package version management by setting ManagePackageVersionsCentrally to true in Directory.Packages.props to eliminate version inconsistencies and reduce maintenance overhead.
  All projects reference packages without version numbers, with versions controlled centrally from a single source of truth.
---

# Decision: Centralized Package Version Management with ManagePackageVersionsCentrally

We have decided to enable centralized package version management across the entire solution by setting `ManagePackageVersionsCentrally` to `true` in the `Directory.Packages.props` file.

## Context

In multi-project .NET solutions, managing NuGet package versions can become challenging and error-prone. Common issues include:

- **Version Inconsistencies**: Different projects in the same solution referencing different versions of the same package, leading to potential compatibility issues and unexpected behavior.
- **Maintenance Overhead**: Updating package versions requires modifying multiple project files individually, increasing the risk of missing updates or introducing inconsistencies.
- **Dependency Conflicts**: Transitive dependencies can introduce version conflicts when projects use different versions of packages that depend on the same underlying libraries.
- **Security Vulnerabilities**: Difficulty in ensuring all projects use the latest secure versions of packages across the entire solution.
- **Build and Runtime Issues**: Version mismatches can cause build failures, runtime exceptions, or subtle bugs that are difficult to diagnose.

Our template-dotnet project serves as a foundation for multiple .NET projects and needs to establish consistent package management practices from the start.

## Decision

We have implemented centralized package version management by:

1. **Enabling `ManagePackageVersionsCentrally`**: Set to `true` in `Directory.Packages.props` to enable MSBuild's central package management feature.
2. **Enabling `CentralPackageTransitivePinningEnabled`**: Set to `true` to ensure transitive dependencies are also managed centrally and pinned to specific versions.
3. **Defining Global Package References**: All common packages (analyzers, build tools, etc.) are defined once in `Directory.Packages.props` with specific versions.
4. **Version-only Management**: Individual project files reference packages without version numbers, with versions controlled centrally.

This approach ensures that all projects in the solution use consistent package versions while maintaining a single source of truth for version management.

## Consequences

### Positive Consequences

- **Consistency**: All projects in the solution use the same versions of shared packages, eliminating version conflicts.
- **Simplified Maintenance**: Package updates only require changes in one location (`Directory.Packages.props`).
- **Better Security Posture**: Easier to ensure all projects use the latest secure versions of packages.
- **Reduced Build Issues**: Eliminates version-related build failures and runtime conflicts.
- **Improved Developer Experience**: Developers don't need to worry about package versions when adding references to existing packages.
- **Transitive Dependency Control**: With `CentralPackageTransitivePinningEnabled`, even indirect dependencies are controlled and predictable.

### Potential Negative Consequences

- **Learning Curve**: Developers unfamiliar with central package management may need time to adapt.
- **Flexibility Reduction**: Individual projects cannot override package versions without modifying the central configuration.
- **Migration Complexity**: Existing projects may require significant changes to adopt this pattern.

## Alternatives Considered

### 1. Traditional Per-Project Package Management

**Description**: Allow each project to manage its own package versions independently.

**Why Not Chosen**:

- Leads to version inconsistencies across the solution
- Increases maintenance overhead
- Higher risk of dependency conflicts
- Difficult to ensure security updates are applied consistently

### 2. Shared MSBuild Props Files Without Central Management

**Description**: Use shared `.props` files to define common package versions but still reference versions in individual projects.

**Why Not Chosen**:

- Still requires version references in each project file
- More complex to implement and maintain
- Doesn't leverage MSBuild's built-in central package management features
- Less enforceable than the native MSBuild approach

### 3. Package Version Variables

**Description**: Define package versions as MSBuild properties and reference them in individual projects.

**Why Not Chosen**:

- Verbose syntax in project files
- No built-in tooling support
- More error-prone than the centralized approach
- Doesn't address transitive dependency management

## Related Decisions

- [GitVersion for Automated Semantic Versioning](./2025-07-10-gitversion-automated-semantic-versioning.md) - Related because centralized package management supports automated versioning by ensuring consistent dependency versions across the solution
