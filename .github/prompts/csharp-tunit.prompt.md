---
agent: 'agent'
tools: ['vscode', 'execute', 'read', 'edit', 'search', 'web', 'deepwiki/*', 'fetch/*', 'microsoft-docs/*', 'nuget-server/*', 'agent', 'todo']
description: 'Best practices for TUnit unit testing, including data-driven tests'
---

# TUnit Best Practices

Help write effective unit tests with TUnit, covering both standard and data-driven testing approaches.

## Project Setup

* MUST use a separate test project with naming convention `[ProjectName].Tests`.
* MUST reference TUnit package and TUnit.Assertions for fluent assertions.
* MUST create test classes that match the classes being tested (for example, `CalculatorTests` for `Calculator`).
* MUST use .NET SDK test commands: `dotnet test` for running tests.
* MUST ensure TUnit requires .NET 8.0 or higher.

## Test Structure

* MUST NOT require test class attributes (like xUnit/NUnit).
* MUST use `[Test]` attribute for test methods (not `[Fact]` like xUnit).
* MUST follow the Arrange-Act-Assert (AAA) pattern.
* MUST name tests using the pattern `MethodName_Scenario_ExpectedBehavior`.
* MUST use lifecycle hooks: `[Before(Test)]` for setup and `[After(Test)]` for teardown.
* MUST use `[Before(Class)]` and `[After(Class)]` for shared context between tests in a class.
* MUST use `[Before(Assembly)]` and `[After(Assembly)]` for shared context across test classes.
* SHOULD use advanced lifecycle hooks like `[Before(TestSession)]` and `[After(TestSession)]` where appropriate.

## Standard Tests

* MUST keep tests focused on a single behavior.
* MUST avoid testing multiple behaviors in one test method.
* MUST use TUnit's fluent assertion syntax with `await Assert.That()`.
* MUST include only the assertions needed to verify the test case.
* MUST make tests independent and idempotent (can run in any order).
* SHOULD avoid test interdependencies (use `[DependsOn]` attribute if needed).

## Data-Driven Tests

* MUST use `[Arguments]` attribute for inline test data (equivalent to xUnit's `[InlineData]`).
* MUST use `[MethodData]` for method-based test data (equivalent to xUnit's `[MemberData]`).
* SHOULD use `[ClassData]` for class-based test data.
* MAY create custom data sources by implementing `ITestDataSource`.
* MUST use meaningful parameter names in data-driven tests.
* MUST allow multiple `[Arguments]` attributes to be applied to the same test method.

## Assertions

* MUST use `await Assert.That(value).IsEqualTo(expected)` for value equality.
* MUST use `await Assert.That(value).IsSameReferenceAs(expected)` for reference equality.
* MUST use `await Assert.That(value).IsTrue()` or `await Assert.That(value).IsFalse()` for boolean conditions.
* MUST use `await Assert.That(collection).Contains(item)` or `await Assert.That(collection).DoesNotContain(item)` for collections.
* SHOULD use `await Assert.That(value).Matches(pattern)` for regex pattern matching.
* MUST use `await Assert.That(action).Throws<TException>()` or `await Assert.That(asyncAction).ThrowsAsync<TException>()` to test exceptions.
* SHOULD chain assertions with `.And` operator: `await Assert.That(value).IsNotNull().And.IsEqualTo(expected)`.
* SHOULD use `.Or` operator for alternative conditions: `await Assert.That(value).IsEqualTo(1).Or.IsEqualTo(2)`.
* SHOULD use `.Within(tolerance)` for DateTime and numeric comparisons with tolerance.
* MUST ensure all assertions are asynchronous and must be awaited.

## Advanced Features

* SHOULD use `[Repeat(n)]` to repeat tests multiple times.
* SHOULD use `[Retry(n)]` for automatic retry on failure.
* SHOULD use `[ParallelLimit<T>]` to control parallel execution limits.
* SHOULD use `[Skip("reason")]` to skip tests conditionally.
* SHOULD use `[DependsOn(nameof(OtherTest))]` to create test dependencies.
* SHOULD use `[Timeout(milliseconds)]` to set test timeouts.
* MAY create custom attributes by extending TUnit's base attributes.

## Test Organization

* SHOULD group tests by feature or component.
* SHOULD use `[Category("CategoryName")]` for test categorization.
* SHOULD use `[DisplayName("Custom Test Name")]` for custom test names.
* MAY use `TestContext` for test diagnostics and information.
* SHOULD use conditional attributes like custom `[WindowsOnly]` for platform-specific tests.

## Performance and Parallel Execution

* MUST understand that TUnit runs tests in parallel by default (unlike xUnit which requires explicit configuration).
* SHOULD use `[NotInParallel]` to disable parallel execution for specific tests.
* SHOULD use `[ParallelLimit<T>]` with custom limit classes to control concurrency.
* MUST understand that tests within the same class run sequentially by default.
* SHOULD use `[Repeat(n)]` with `[ParallelLimit<T>]` for load testing scenarios.

## Migration from xUnit

* MUST replace `[Fact]` with `[Test]`.
* MUST replace `[Theory]` with `[Test]` and use `[Arguments]` for data.
* MUST replace `[InlineData]` with `[Arguments]`.
* MUST replace `[MemberData]` with `[MethodData]`.
* MUST replace `Assert.Equal` with `await Assert.That(actual).IsEqualTo(expected)`.
* MUST replace `Assert.True` with `await Assert.That(condition).IsTrue()`.
* MUST replace `Assert.Throws<T>` with `await Assert.That(action).Throws<T>()`.
* MUST replace constructor/IDisposable with `[Before(Test)]`/`[After(Test)]`.
* MUST replace `IClassFixture<T>` with `[Before(Class)]`/`[After(Class)]`.

### Why TUnit over xUnit?

TUnit offers a modern, fast, and flexible testing experience with advanced features not present in xUnit, such as asynchronous assertions, more refined lifecycle hooks, and improved data-driven testing capabilities. TUnit's fluent assertions provide clearer and more expressive test validation, making it especially suitable for complex .NET projects.