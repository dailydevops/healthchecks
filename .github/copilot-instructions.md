# Instructions for GitHub and VisualStudio Copilot

## General

* Make only high confidence suggestions when reviewing code changes.
* Always use the latest version C#, currently C# 13 features.
* Never change `global.json` unless explicitly asked to.
* Never change `Directory.Build.props` unless explicitly asked to.
* Never change `Directory.Build.targets` unless explicitly asked to.
* Never change `Directory.Packages.props` unless explicitly asked to.

## Code Style

* Use `var` when the type is obvious from the right side of the assignment.
* Use `Argument.ThrowIfNull(x)` instead of `if (x == null) throw new ArgumentNullException(nameof(x))`, when nuget package `NetEvolve.Arguments` is referenced.
* Use `ArgumentNullException.ThrowIfNull(x)` instead of `if (x == null) throw new ArgumentNullException(nameof(x))`, when nuget package `NetEvolve.Arguments` is not referenced.

## Formatting

* Apply code-formatting style defined in `.editorconfig`.
* Prefer file-scoped namespace declarations and single-line using directives.
* Insert a newline before the opening curly brace of any code block (e.g., after `if`, `for`, `while`, `foreach`, `using`, `try`, etc.).
* Ensure that the final return statement of a method is on its own line.
* Use pattern matching and switch expressions wherever possible.
* Use `nameof` instead of string literals when referring to member names.
* Ensure that XML doc comments are created for any public APIs. When applicable, include `<inheritdoc />` for derived members.

### Arrays and Collections

* Prefer `Enumerable.Empty<T>()` or `Array.Empty<T>()` over `null` returns.

## Nullable Reference Types

* Declare variables non-nullable, and check for `null` at entry points.
* Always use `is null` or `is not null` instead of `== null` or `!= null`.
* Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.
* Apply `NotNullAttribute` and other nullable attributes for any public APIs.

## Testing

* We prefer to use [TUnit](https://github.com/thomhurst/TUnit) with [Microsoft.Testing.Platform](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro).
* If TUnit is not available, we use [XUnit](https://xunit.net/).
* Do not emit "Act", "Arrange" or "Assert" comments.
* We do not use any mocking framework at the moment.
* Copy existing style in nearby files for test method names and capitalization.

### Running tests

1. Build from the root with `dotnet build <solutionfile>`.
2. If that produces errors, fix those errors and build again. Repeat until the build is successful.
3. To then run tests, use a command similar to this `dotnet test <solutionfile>` (using the path to whatever projects are applicable to the change).
