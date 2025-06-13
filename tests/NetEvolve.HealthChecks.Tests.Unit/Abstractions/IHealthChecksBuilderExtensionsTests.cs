namespace NetEvolve.HealthChecks.Tests.Unit.Abstractions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Abstractions;

[TestGroup(nameof(HealthChecks))]
public class IHealthChecksBuilderExtensionsTests
{
    [Test]
    public void IsServiceTypeRegistered_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => builder.IsServiceTypeRegistered<IConfiguration>();

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Test]
    public async Task IsServiceTypeRegistered_WhenServiceTypeIsRegistered_ReturnsTrue()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();

        // Act
        var result = builder.IsServiceTypeRegistered<IConfiguration>();

        // Assert
        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsServiceTypeRegistered_WhenServiceTypeIsNotRegistered_ReturnsFalse()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        var result = builder.IsServiceTypeRegistered<IConfiguration>();

        // Assert
        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public void ThrowIfNameIsAlreadyUsed_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => builder.ThrowIfNameIsAlreadyUsed<TestHealthCheck>("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
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
