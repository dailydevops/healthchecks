namespace NetEvolve.HealthChecks.Tests.Unit.NATS;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::NATS.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.NATS;
using NSubstitute;

[TestGroup(nameof(NATS))]
public sealed class NATSHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WithKeyedService_UsesKeyedService()
    {
        // Arrange
        var options = new NATSOptions { KeyedService = "test-key", Timeout = 100 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<NATSOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that returns success
        var mockConnection = Substitute.For<IConnection>();
        _ = mockConnection.State.Returns(ConnState.CONNECTED);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddKeyedSingleton("test-key", mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new NATSHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo("test: Healthy");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WithoutKeyedService_UsesDefaultService()
    {
        // Arrange
        var options = new NATSOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<NATSOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that returns success
        var mockConnection = Substitute.For<IConnection>();
        _ = mockConnection.State.Returns(ConnState.CONNECTED);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new NATSHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo("test: Healthy");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenConnectionClosed_ReturnsDegraded()
    {
        // Arrange
        var options = new NATSOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<NATSOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that returns closed state
        var mockConnection = Substitute.For<IConnection>();
        mockConnection.State.Returns(ConnState.CLOSED);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new NATSHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Degraded);
            _ = await Assert.That(result.Description).IsEqualTo("test: Degraded", StringComparison.Ordinal);
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenTimeout_ReturnsDegraded()
    {
        // Arrange
        var options = new NATSOptions
        {
            KeyedService = null,
            Timeout = 5, // Very short timeout to force a timeout
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<NATSOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that delays long enough to cause timeout
        var mockConnection = Substitute.For<IConnection>();

        // Configure the mock so that accessing State takes longer than the timeout
        mockConnection
            .State.Returns(_ =>
            {
                Thread.Sleep(200); // Delay to force timeout
                return ConnState.CONNECTED;
            });

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new NATSHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Degraded);
            _ = await Assert.That(result.Description).IsEqualTo("test: Degraded", StringComparison.Ordinal);
        }
    }
}
