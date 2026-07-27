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
using TUnit.Mocks;

[TestGroup(nameof(NATS))]
public sealed class NATSHealthCheckTests
{
    private const string TestName = nameof(NATS);

    [Test]
    public async Task CheckHealthAsync_WithKeyedService_UsesKeyedService()
    {
        // Arrange
        var options = new NatsOptions { KeyedService = "test-key", Timeout = 10000 };

        var optionsMonitor = IOptionsMonitor<NatsOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup connection mock that returns success
        var mockConnection = IConnection.Mock();
        _ = mockConnection.State.Returns(ConnState.CONNECTED);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddKeyedSingleton<IConnection>("test-key", mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new NatsHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{TestName}: Healthy");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WithoutKeyedService_UsesDefaultService()
    {
        // Arrange
        var options = new NatsOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = IOptionsMonitor<NatsOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup connection mock that returns success
        var mockConnection = IConnection.Mock();
        _ = mockConnection.State.Returns(ConnState.CONNECTED);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<IConnection>(mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new NatsHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{TestName}: Healthy");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenConnectionClosed_ReturnsUnhealthy()
    {
        // Arrange
        var options = new NatsOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = IOptionsMonitor<NatsOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup connection mock that returns closed state
        var mockConnection = IConnection.Mock();
        _ = mockConnection.State.Returns(ConnState.CLOSED);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<IConnection>(mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new NatsHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert
                .That(result.Description)
                .IsEqualTo($"{TestName}: NATS connection is not connected.", StringComparison.Ordinal);
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenTimeout_ReturnsDegraded()
    {
        // Arrange
        var options = new NatsOptions
        {
            KeyedService = null,
            Timeout = 0, // Very short timeout to force a timeout
        };

        var optionsMonitor = IOptionsMonitor<NatsOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup connection mock that delays long enough to cause timeout
        var mockConnection = IConnection.Mock();

        // Configure the mock so that accessing State takes longer than the timeout
        _ = mockConnection.State.Returns(() =>
        {
            Thread.Sleep(200); // Delay to force timeout
            return ConnState.CONNECTED;
        });

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<IConnection>(mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new NatsHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Degraded);
            _ = await Assert.That(result.Description).IsEqualTo($"{TestName}: Degraded", StringComparison.Ordinal);
        }
    }
}
