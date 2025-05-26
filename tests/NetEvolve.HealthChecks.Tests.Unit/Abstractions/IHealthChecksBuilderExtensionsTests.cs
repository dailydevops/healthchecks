namespace NetEvolve.HealthChecks.Tests.Unit.Abstractions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Abstractions;
using Xunit;

[TestGroup(nameof(HealthChecks))]
public class IHealthChecksBuilderExtensionsTests
{
    [Fact]
    public void IsServiceTypeRegistered_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => builder.IsServiceTypeRegistered<IConfiguration>();

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Fact]
    public void IsServiceTypeRegistered_WhenServiceTypeIsRegistered_ReturnsTrue()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();

        // Act
        var result = builder.IsServiceTypeRegistered<IConfiguration>();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsServiceTypeRegistered_WhenServiceTypeIsNotRegistered_ReturnsFalse()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        var result = builder.IsServiceTypeRegistered<IConfiguration>();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNameAlreadyUsed_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => builder.IsNameAlreadyUsed<TestHealthCheck>("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Fact]
    public void IsNameAlreadyUsed_WhenNameIsUsedForSameServiceType_ReturnsTrue()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<TestHealthCheck>();
        var builder = services.AddHealthChecks();
        const string? name = "Test";

        // Add a health check with the given name and type
        _ = builder.AddCheck<TestHealthCheck>(name, HealthStatus.Healthy);

        // Act
        var result = builder.IsNameAlreadyUsed<TestHealthCheck>(name);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNameAlreadyUsed_WhenNameIsUsedForDifferentServiceType_ReturnsFalse()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<TestHealthCheck>();
        _ = services.AddSingleton<AnotherTestHealthCheck>();
        var builder = services.AddHealthChecks();
        const string? name = "Test";

        // Add a health check with the given name but different type
        _ = builder.AddCheck<AnotherTestHealthCheck>(name, HealthStatus.Healthy);

        // Act
        var result = builder.IsNameAlreadyUsed<TestHealthCheck>(name);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNameAlreadyUsed_WhenNameIsNotUsed_ReturnsFalse()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<TestHealthCheck>();
        var builder = services.AddHealthChecks();

        // No health check added with the name "Test"

        // Act
        var result = builder.IsNameAlreadyUsed<TestHealthCheck>("Test");

        // Assert
        Assert.False(result);
    }

    // Test health check classes
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used in tests")]
    private sealed class TestHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
        ) => Task.FromResult(HealthCheckResult.Healthy());
    }

    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used in tests")]
    private sealed class AnotherTestHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
        ) => Task.FromResult(HealthCheckResult.Healthy());
    }
}
