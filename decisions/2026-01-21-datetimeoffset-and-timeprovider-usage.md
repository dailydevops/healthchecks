---
authors:
  - Martin Stühmer

applyTo:
  - "**/*.cs"

created: 2026-01-21

lastModified: 2026-01-21

state: accepted

instructions: |
  MUST use DateTimeOffset instead of DateTime for all date and time values unless external constraints require DateTime.
  MUST use TimeProvider for obtaining current time to ensure testability.
  MUST inject TimeProvider via constructor dependency injection.
  MUST NOT use DateTime.Now, DateTime.UtcNow, DateTimeOffset.Now, or DateTimeOffset.UtcNow directly in production code.
  MAY use DateOnly or TimeOnly only for performance-critical scenarios or UI display requirements.
---

# Decision: DateTimeOffset and TimeProvider Usage

This decision establishes the mandatory use of `DateTimeOffset` over `DateTime` and requires `TimeProvider` for all time-related operations to ensure consistent timezone handling and testability.

## Context

Date and time handling in software development presents several challenges:

1. **Timezone Ambiguity**: `DateTime` does not inherently store timezone information. The `Kind` property (`Local`, `Utc`, `Unspecified`) is often misinterpreted or lost during serialization, leading to incorrect time calculations across different timezones.

2. **Serialization Issues**: When `DateTime` values are serialized and deserialized (JSON, databases, APIs), timezone context is frequently lost, causing subtle bugs that are difficult to diagnose.

3. **Testability Concerns**: Direct calls to `DateTime.Now` or `DateTime.UtcNow` create hidden dependencies on the system clock, making unit tests non-deterministic and time-dependent scenarios impossible to test reliably.

4. **Global Applications**: Modern applications often serve users across multiple timezones, requiring precise handling of temporal data with explicit offset information.

5. **.NET Evolution**: .NET 8 introduced `TimeProvider` as a first-class abstraction for time operations, providing a standardized approach for testable time-dependent code.

## Decision

The project MUST adhere to the following requirements for date and time handling:

### DateTimeOffset over DateTime

* MUST use `DateTimeOffset` instead of `DateTime` for all date and time values.
* MUST only use `DateTime` when external constraints require it (for example, third-party APIs, legacy database schemas, or framework limitations).
* MUST document the reason when `DateTime` usage is unavoidable.
* MUST convert `DateTime` values to `DateTimeOffset` at system boundaries as early as possible.

### DateOnly and TimeOnly Usage

* MAY use `DateOnly` or `TimeOnly` only when one of the following conditions applies:
  - Performance-critical scenarios where the reduced memory footprint is measurable and relevant.
  - UI display requirements where only date or time components are needed for presentation.
* MUST NOT use `DateOnly` or `TimeOnly` as a general replacement for `DateTimeOffset`.
* MUST convert `DateOnly` or `TimeOnly` to `DateTimeOffset` when persisting to databases or transmitting via APIs.
* MUST document the reason when using `DateOnly` or `TimeOnly`.

### TimeProvider for Current Time

* MUST use `TimeProvider` to obtain the current time in all production code.
* MUST inject `TimeProvider` via constructor dependency injection.
* MUST NOT call `DateTime.Now`, `DateTime.UtcNow`, `DateTimeOffset.Now`, or `DateTimeOffset.UtcNow` directly.
* MUST use `TimeProvider.System` as the default implementation in production.
* MUST use `Microsoft.Extensions.Time.Testing.FakeTimeProvider` or custom implementations for testing.

### Implementation Pattern

```csharp
public class OrderService
{
    private readonly TimeProvider _timeProvider;

    public OrderService(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
    }

    public Order CreateOrder(OrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Order
        {
            Id = Guid.NewGuid(),
            CreatedAt = _timeProvider.GetUtcNow(),
            // ... other properties
        };
    }
}
```

### Test Implementation Pattern

```csharp
public class OrderServiceTests
{
    [Test]
    public async Task CreateOrder_ShouldSetCreatedAtToCurrentTime()
    {
        // Arrange
        var fakeTime = new DateTimeOffset(2026, 1, 21, 10, 30, 0, TimeSpan.Zero);
        var fakeTimeProvider = new FakeTimeProvider(fakeTime);
        var service = new OrderService(fakeTimeProvider);
        var request = new OrderRequest { /* ... */ };

        // Act
        var order = service.CreateOrder(request);

        // Assert
        await Assert.That(order.CreatedAt).IsEqualTo(fakeTime);
    }
}
```

## Consequences

### Positive Consequences

* **Timezone Safety**: `DateTimeOffset` explicitly stores the offset from UTC, eliminating timezone ambiguity.
* **Serialization Reliability**: Timezone information is preserved during serialization and deserialization.
* **Deterministic Tests**: `TimeProvider` injection enables fully controllable and repeatable time-based tests.
* **Time Travel Testing**: Tests can simulate past and future dates without system clock manipulation.
* **Framework Alignment**: Aligns with .NET 8+ best practices and the `TimeProvider` abstraction.
* **Cross-Timezone Correctness**: Calculations involving multiple timezones produce correct results.

### Negative Consequences

* **Migration Effort**: Existing code using `DateTime` requires refactoring.
* **Dependency Injection Overhead**: Every class requiring current time needs `TimeProvider` injection.
* **Learning Curve**: Developers unfamiliar with `DateTimeOffset` or `TimeProvider` require onboarding.
* **Storage Considerations**: Some databases handle `DateTimeOffset` differently than `DateTime`.
* **Third-Party Limitations**: Some external libraries or APIs may still require `DateTime`.

## Alternatives Considered

### Continue Using DateTime with UTC Convention

**Description**: Enforce a convention that all `DateTime` values are UTC and rely on `DateTimeKind.Utc`.

**Rejection Rationale**:
* Convention-based approaches are error-prone and not compiler-enforced.
* `DateTimeKind` is easily lost during serialization.
* Does not address testability concerns.
* Requires manual discipline across the entire codebase.

### Custom Time Abstraction

**Description**: Create a project-specific `ITimeService` or `IClock` interface for time operations.

**Rejection Rationale**:
* Reinvents functionality already provided by .NET's `TimeProvider`.
* Creates maintenance burden for custom abstraction.
* Reduces interoperability with libraries expecting `TimeProvider`.
* `TimeProvider` is the standardized .NET solution since .NET 8.

### Static Time Helper with Ambient Context

**Description**: Use a static `TimeHelper.Now` property that can be swapped during testing via ambient context.

**Rejection Rationale**:
* Ambient context patterns hide dependencies and reduce code clarity.
* Static access makes parallel test execution problematic.
* Violates explicit dependency principle.
* Not thread-safe without careful implementation.

## Related Decisions

* [.NET 10 and C# 13 Adoption](./2025-07-11-dotnet-10-csharp-13-adoption.md) - This decision builds upon the .NET version adoption which provides `TimeProvider` as a standard framework feature.
