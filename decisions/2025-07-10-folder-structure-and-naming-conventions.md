---
authors:
  - Martin Stühmer

applyTo:
  - "**/*.*"

created: 2025-07-10

lastModified: 2025-07-14

state: accepted

instructions: |
  Establish standard folder structure with src/ for production code and tests/ for test projects using ProjectName.TestType naming pattern.
  Test types include Unit, Integration, Performance, and Acceptance to ensure clear categorization and automated execution.
---

# Decision: Folder Structure and Naming Conventions

This decision establishes the standard folder structure for .NET projects and defines naming conventions for test projects to ensure consistency and clarity across the codebase.

## Context

As .NET projects grow in complexity, maintaining a clear and consistent folder structure becomes crucial for:

- Developer onboarding and navigation
- Build automation and CI/CD pipelines
- Separation of concerns between production and test code
- Test categorization and execution strategies
- IDE and tooling support

Without standardized conventions, teams may adopt inconsistent patterns that lead to:

- Confusion about where to place new code
- Difficulty in setting up automated test execution
- Unclear distinction between different types of tests
- Inconsistent project references and dependencies

## Decision

We will adopt the following folder structure and naming conventions:

### Root Folder Structure

```txt
/
├── src/           # All production source code
├── tests/         # All test projects
├── docs/          # Documentation (optional)
├── tools/         # Build tools and scripts (optional)
└── samples/       # Example applications (optional)
```

### Source Code Organization

- All production code MUST be placed under the `src/` folder
- Project folders under `src/` MUST follow the namespace hierarchy
- Each project MUST have its own folder directly under `src/`
- The `SpixSpreed.` prefix is automatically applied to all projects through build automation in `Directory.Build.targets`

### Test Project Naming Conventions

Test projects MUST follow this naming pattern:

- **Unit Tests**: `{ProjectName}.Tests.Unit`
- **Integration Tests**: `{ProjectName}.Tests.Integration`

Where `{ProjectName}` is the name of the production project being tested (the build system will automatically apply the `SpixSpreed.` prefix).

### Project Naming Convention

Project folder names follow a simple `{ComponentName}` pattern, where:

- `{ComponentName}` describes the functional area or component (e.g., Core, Data, Api, Web)
- The `SpixSpreed.` prefix is automatically applied by the build system through `Directory.Build.targets`
- This ensures consistent assembly names, namespaces, and package IDs without manual prefixing

### Examples

```txt
src/
├── Api/                     # Web API controllers, middleware, and HTTP-related logic
├── Core/                    # Domain models, business logic, interfaces, and shared utilities
└── Data/                    # Data access layer, repositories, Entity Framework contexts

tests/
├── Api.Tests.Integration/   # End-to-end API tests with real HTTP requests
├── Api.Tests.Unit/          # Unit tests for controllers, middleware, and API logic
├── Core.Tests.Integration/  # Integration tests for business workflows and cross-component scenarios
├── Core.Tests.Unit/         # Unit tests for domain models, business logic, and utilities
├── Data.Tests.Integration/  # Database integration tests with real or test databases
└── Data.Tests.Unit/         # Unit tests for repositories and data access logic
```

## Consequences

### Positive Consequences

- **Clear Separation**: Distinct separation between production and test code prevents accidental inclusion of test code in production builds
- **Consistent Navigation**: Developers can quickly locate source code and corresponding tests
- **Test Categorization**: Clear distinction between unit and integration tests enables selective test execution
- **Build Pipeline Optimization**: CI/CD pipelines can easily target specific test categories (e.g., run unit tests on every commit, integration tests on merge to main)
- **IDE Support**: Most IDEs and tools recognize this structure and provide better navigation and project templates
- **Scalability**: Structure scales well as projects grow in size and complexity

### Potential Drawbacks

- **Initial Setup**: Requires discipline to maintain the structure, especially in the early stages of a project
- **Migration Effort**: Existing projects may need restructuring to adopt these conventions
- **Tool Configuration**: Some tools may need configuration updates to work with the new structure

## Alternatives Considered

### Alternative 1: Flat Structure

```txt
/
├── ProjectA/
├── ProjectA.Tests/
├── ProjectB/
└── ProjectB.Tests/
```

**Rejected because**: Doesn't scale well, mixes production and test code at the same level, and doesn't distinguish between test types.

### Alternative 2: Tests Alongside Source

```txt
src/
├── ProjectA/
│   ├── ProjectA.csproj
│   └── Tests/
└── ProjectB/
    ├── ProjectB.csproj
    └── Tests/
```

**Rejected because**: MSBuild automatically includes all code files in subfolders, which would cause the test code to be included in the production project compilation. This creates build errors and conflicts, making this structure technically infeasible without complex exclusion configurations.

### Alternative 3: Different Test Suffixes

Using `.UnitTests` and `.IntegrationTests` instead of `.Tests.Unit` and `.Tests.Integration`.

**Rejected because**: The chosen format provides better grouping in IDE project explorers and is more explicit about the relationship between test types.

## Related Decisions

None at this time. This decision establishes foundational project structure conventions that are independent of other architectural decisions.
