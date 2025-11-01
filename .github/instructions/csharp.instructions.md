---
applyTo: '**/*.cs'

description: 'This file contains instructions for C# development.
  It includes guidelines for using GitHub Copilot, managing dependencies, and following coding standards.
  Ensure to follow the practices outlined in this file to maintain code quality and consistency.'
---

# C# Development Instructions

## General

* The project MUST use the latest version C#, currently C# 13 features.
* MUST follow standard C# coding conventions (naming, indentation, spacing).
* MUST implement robust error handling with try-catch blocks and meaningful messages.
* MUST write unit tests for all critical functionalities.
* MUST always review AI-generated code for correctness, security, and performance.

## Code Style

Follow these C# coding conventions for consistency across the project.

* MUST use `var` when the type is obvious from the right side of the assignment.
* MUST use appropriate null-checking pattern based on package availability:
  - When `NetEvolve.Arguments` package is referenced in project: Use `Argument.ThrowIfNull(x)`
  - When `NetEvolve.Arguments` package is NOT referenced: Use `ArgumentNullException.ThrowIfNull(x)`
  - Check `Directory.Packages.props` for `NetEvolve.Arguments` availability
  - Example with NetEvolve.Arguments:
    ```csharp
    public void ProcessData(string data)
    {
        Argument.ThrowIfNull(data);
        // process data
    }
    ```
  - Example without NetEvolve.Arguments:
    ```csharp
    public void ProcessData(string data)
    {
        ArgumentNullException.ThrowIfNull(data);
        // process data
    }
    ```

## Formatting

Apply code-formatting style defined in `.editorconfig`.

* MUST prefer file-scoped namespace declarations and single-line using directives.
* MUST insert a newline before the opening curly brace of any code block (e.g., after `if`, `for`, `while`, `foreach`, `using`, `try`, etc.).
* MUST ensure that the final return statement of a method is on its own line.
* MUST use pattern matching and switch expressions wherever possible.
* MUST use `nameof` instead of string literals when referring to member names.
* MUST ensure that XML doc comments are created for any public APIs. When applicable, include `<inheritdoc />` for derived members.

## Arrays and Collections

* MUST prefer `Enumerable.Empty<T>()` or `Array.Empty<T>()` over `null` returns.

## Nullable Reference Types

Declare variables non-nullable, and check for `null` at entry points.

* MUST always use `is null` or `is not null` instead of `== null` or `!= null`.
* MUST trust the C# null annotations and don't add null checks when the type system says a value cannot be null.
* MUST apply `NotNullAttribute` and other nullable attributes for any public APIs.

## Testing

* MUST prefer to use [TUnit](https://github.com/thomhurst/TUnit) with [Microsoft.Testing.Platform](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro).
* If TUnit is not available, MUST use [XUnit](https://xunit.net/).
* MUST NOT emit `Act`, `Arrange` or `Assert` comments.
* MUST NOT use any mocking framework at the moment.
* MUST copy existing style in nearby files for test method names and capitalization.

## Build and test AI-generated code

1. Build from the root with `dotnet build <solutionfile>.slnx`.
2. If that produces errors, fix those errors and build again. Repeat until the build is successful.
3. Run tests with `dotnet test <solutionfile>.slnx`.
4. If tests fail, analyze the failures and fix the code. Do not ignore failing tests.
5. Verify that all new code has appropriate test coverage.
6. Run a final build to ensure everything compiles successfully after all changes.

