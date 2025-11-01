<!-- List of authors who contributed to this decision. Include full names and roles if applicable. -->
authors:
- Martin Stühmer

<!--
The patterns this decision applies to. Each entry is a glob pattern that matches files affected by this decision.
-->
applyTo:
- "**/*.csproj"
- "**/*.fsproj"
- "**/*.vbproj"
- "**/*.sln"
- "**/*.slnx"
- "**/tests/**/*.cs"
- "AGENTS.md"
- ".github/copilot-instructions.md"

<!-- The date this ADR was initially created in YYYY-MM-DD format. -->
created: 2025-11-01

<!--
The most recent date this ADR was updated in YYYY-MM-DD format.
IMPORTANT: Update this field whenever the decision is modified.
-->
lastModified: 2025-11-01

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
  AI agents are authorized to execute dotnet build, restore, and test commands for all projects in the solution to verify code correctness and maintain quality standards.
  All operations MUST be executed at the solution level using the .sln or .slnx file found in the repository root directory.
  Before committing changes, AI agents MUST run restore, build, and test operations to ensure no regressions are introduced.
---
# Decision: AI Agent Authorization for Build, Restore, and Test Operations

AI coding assistants such as GitHub Copilot are authorized to execute `dotnet build`, `dotnet restore`, and `dotnet test` commands for all projects within the solution to verify code correctness, ensure compilation success, and validate functionality through automated tests.

## Context

AI coding assistants are increasingly capable of making complex code changes across multiple files and projects. However, without the ability to verify their changes, several issues arise:

- **Undetected Compilation Errors**: Code changes may introduce syntax errors, missing references, or type mismatches that prevent successful compilation.
- **Broken Dependencies**: Package references may be incomplete or incorrect, causing restore failures.
- **Regression Bugs**: Changes may break existing functionality that is covered by automated tests.
- **Integration Issues**: Multi-project changes may compile individually but fail when integrated.
- **Quality Degradation**: Without verification, the quality of AI-generated code cannot be assured before commit.
- **Developer Burden**: Human developers must manually verify all AI-generated changes, reducing productivity gains.

To maximize the effectiveness of AI coding assistants while maintaining code quality, agents need the capability to validate their own work through standard .NET development operations.

## Decision

We authorize AI coding assistants to execute the following .NET CLI commands:

1. **`dotnet restore`**: Restore NuGet packages and dependencies for projects and solutions.
2. **`dotnet build`**: Compile projects and solutions to verify code correctness and identify compilation errors.
3. **`dotnet test`**: Execute unit tests, integration tests, and other automated tests to validate functionality.

### Implementation Guidelines

AI agents MUST:

- Detect the solution file (`.sln` or `.slnx`) in the repository root directory and use it for all build, restore, and test operations.
- Execute all commands at the solution level to ensure consistency across all projects and detect integration issues.
- Execute `dotnet restore` before building if package references have been added or modified.
- Execute `dotnet build` after making code changes to verify compilation success.
- Execute `dotnet test` after code changes that may affect functionality to ensure no regressions.
- Report compilation errors, warnings, and test failures clearly to the user.
- Iterate on fixes when errors or test failures are detected.
- Use appropriate command-line options (e.g., `--configuration Release`, `--no-restore`) as context requires.

AI agents SHOULD:

- Use `--verbosity` options appropriately to provide useful diagnostic information.
- Consider build performance implications and avoid unnecessary rebuilds.
- Verify that the solution file exists before executing commands and report an error if missing.

AI agents MAY:

- Execute `dotnet clean` before building to ensure a clean build environment.
- Use additional CLI options (e.g., `--framework`, `--runtime`) when targeting specific configurations.
- Run specific test filters (e.g., `--filter Category=Unit`) when appropriate.

## Consequences

### Positive Consequences

- **Improved Code Quality**: AI-generated code is validated before being presented to developers.
- **Faster Development Cycles**: Errors are caught and fixed immediately by the AI agent rather than discovered later by developers.
- **Reduced Human Oversight**: Developers can trust that AI-generated changes compile and pass tests.
- **Better Error Context**: AI agents can see and respond to actual compiler and test errors rather than relying on static analysis alone.
- **Continuous Verification**: Changes are validated in real-time as part of the AI's workflow.
- **Test-Driven Development Support**: AI agents can run tests iteratively while implementing features.

### Potential Negative Consequences

- **Resource Usage**: Build and test operations consume CPU, memory, and disk I/O resources.
- **Time Overhead**: Each verification cycle adds time to AI operations.
- **Potential for Build Loops**: Poorly designed fixes could lead to repeated failed build attempts.
- **Complexity**: AI agents need to interpret and respond appropriately to build and test output.

### Mitigation Strategies

- Use incremental builds where possible to minimize resource usage.
- Set reasonable timeout limits for build and test operations.
- Implement clear error handling and reporting mechanisms.
- Monitor for and prevent infinite fix-attempt loops.
- Allow users to disable automatic verification if needed.

## Alternatives Considered

### 1. Manual Verification Only

**Description**: Require human developers to manually run build and test commands after AI-generated changes.

**Why Not Chosen**:
- Reduces the value proposition of AI assistance
- Increases developer workload and slows down development
- Errors are discovered later in the workflow
- Does not leverage AI's ability to iterate on fixes

### 2. Static Analysis Only

**Description**: Use static code analysis and linting without actual compilation or test execution.

**Why Not Chosen**:
- Cannot detect runtime issues or test failures
- May miss compilation errors not caught by static analysis
- Less comprehensive verification than actual build/test
- Doesn't validate package dependencies and restore operations

### 3. Sandbox/Mock Build Environment

**Description**: Create a separate, isolated environment for AI build verification that doesn't use actual project files.

**Why Not Chosen**:
- Adds significant complexity to implementation
- May not accurately reflect actual build behavior
- Difficult to maintain parity with real build environment
- Adds maintenance overhead for separate build configuration

### 4. Read-Only Analysis Without Execution

**Description**: Allow AI agents to analyze code but not execute any commands.

**Why Not Chosen**:
- Severely limits AI effectiveness
- Cannot verify changes work correctly
- Misses the primary benefit of automated verification
- Contradicts the goal of autonomous AI assistance

## Related Decisions

- [Centralized Package Version Management](./2025-07-10-centralized-package-version-management.md) - AI agents must respect centralized package versions when executing restore operations
- [.NET 10 and C# 13 Adoption](./2025-07-11-dotnet-10-csharp-13-adoption.md) - Build operations must target the correct .NET version and C# language version
