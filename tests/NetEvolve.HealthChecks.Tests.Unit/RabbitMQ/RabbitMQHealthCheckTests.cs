namespace NetEvolve.HealthChecks.Tests.Unit.RabbitMQ;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using global::RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.RabbitMQ;
using NSubstitute;
using TUnit.Mocks;

[TestGroup(nameof(RabbitMQ))]
public sealed class RabbitMQHealthCheckTests
{
    private const string TestName = nameof(RabbitMQ);

    [Test]
    [SuppressMessage(
        "Reliability",
        "CA2025:Do not pass 'IDisposable' instances into unawaited tasks",
        Justification = "As designed."
    )]
    public async Task CheckHealthAsync_WithKeyedService_UsesKeyedService()
    {
        // Arrange
        var options = new RabbitMQOptions { KeyedService = "test-key", Timeout = 10000 };

        var optionsMonitor = IOptionsMonitor<RabbitMQOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup connection mock that returns success
        var mockChannel = IChannel.Mock();
        _ = mockChannel.IsOpen.Returns(true);
        var mockConnection = IConnection.Mock();
        _ = mockConnection.CreateChannelAsync(Any(), Any()).Returns(mockChannel);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddKeyedSingleton<IConnection>("test-key", mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new RabbitMQHealthCheck(serviceProvider, optionsMonitor);
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
    [SuppressMessage(
        "Reliability",
        "CA2025:Do not pass 'IDisposable' instances into unawaited tasks",
        Justification = "As designed."
    )]
    public async Task CheckHealthAsync_WithoutKeyedService_UsesDefaultService()
    {
        // Arrange
        var options = new RabbitMQOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = IOptionsMonitor<RabbitMQOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup connection mock that returns success
        var mockChannel = IChannel.Mock();
        _ = mockChannel.IsOpen.Returns(true);
        var mockConnection = IConnection.Mock();
        _ = mockConnection.CreateChannelAsync(Any(), Any()).Returns(mockChannel);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<IConnection>(mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new RabbitMQHealthCheck(serviceProvider, optionsMonitor);
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
    public async Task CheckHealthAsync_WhenConnectionFails_ReturnsUnhealthy()
    {
        // Arrange
        var options = new RabbitMQOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = IOptionsMonitor<RabbitMQOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup connection mock that throws an exception
        var mockConnection = IConnection.Mock();
        _ = mockConnection.CreateChannelAsync(Any(), Any()).Throws(new InvalidOperationException("Connection failed"));

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<IConnection>(mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new RabbitMQHealthCheck(serviceProvider, optionsMonitor);
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
                .IsEqualTo($"{TestName}: Unexpected error.", StringComparison.Ordinal);
            _ = await Assert.That(result.Exception).IsNotNull();
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenTimeout_ReturnsDegraded()
    {
        // Arrange
        var options = new RabbitMQOptions
        {
            KeyedService = null,
            Timeout = 1, // Very short timeout to force a timeout
        };

        // TUnit.Mocks' .Returns() only supports a synchronous Func<T> (auto-wrapped into a
        // completed Task<T>) - there's no way to hand back a task that stays pending and
        // completes asynchronously later, so a genuine timeout-race can't be simulated with it.
        // NSubstitute is used here instead, purely for this one test.
        var optionsMonitor = NSubstitute.Substitute.For<IOptionsMonitor<RabbitMQOptions>>();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup connection mock that delays long enough to cause timeout
        var mockChannel = NSubstitute.Substitute.For<IChannel>();
        _ = mockChannel.IsOpen.Returns(true);
        var mockConnection = NSubstitute.Substitute.For<IConnection>();
        _ = mockConnection
            .CreateChannelAsync(NSubstitute.Arg.Any<CreateChannelOptions>(), NSubstitute.Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                await Task.Delay(50); // Delay to force timeout
                return mockChannel;
            });

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new RabbitMQHealthCheck(serviceProvider, optionsMonitor);
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
