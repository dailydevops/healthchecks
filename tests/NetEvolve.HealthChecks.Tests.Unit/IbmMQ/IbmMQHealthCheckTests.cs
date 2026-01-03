namespace NetEvolve.HealthChecks.Tests.Unit.IbmMQ;

using System;
using System.Threading;
using System.Threading.Tasks;
using IBM.WMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.IbmMQ;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

[TestGroup(nameof(IbmMQ))]
public sealed class IbmMQHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WithKeyedService_UsesKeyedService()
    {
        // Arrange
        var options = new IbmMQOptions { KeyedService = "test-key", Timeout = 10000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<IbmMQOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var mockQueueManager = Substitute.For<MQQueueManager>();
        _ = mockQueueManager.IsConnected.Returns(true);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddKeyedSingleton("test-key", mockQueueManager);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new IbmMQHealthCheck(serviceProvider, optionsMonitor);
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
        var options = new IbmMQOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<IbmMQOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var mockQueueManager = Substitute.For<MQQueueManager>();
        _ = mockQueueManager.IsConnected.Returns(true);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockQueueManager);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new IbmMQHealthCheck(serviceProvider, optionsMonitor);
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
    public async Task CheckHealthAsync_WhenNotConnected_ReturnsUnhealthy()
    {
        // Arrange
        var options = new IbmMQOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<IbmMQOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var mockQueueManager = Substitute.For<MQQueueManager>();
        _ = mockQueueManager.IsConnected.Returns(false);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockQueueManager);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new IbmMQHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo("test: IBM MQ Queue Manager is not connected.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenExceptionThrown_ReturnsUnhealthy()
    {
        // Arrange
        var options = new IbmMQOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<IbmMQOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var mockQueueManager = Substitute.For<MQQueueManager>();
        _ = mockQueueManager.IsConnected.Throws(new InvalidOperationException("Connection failed"));

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockQueueManager);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new IbmMQHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo("test: Unexpected error.", StringComparison.Ordinal);
            _ = await Assert.That(result.Exception).IsNotNull();
        }
    }
}
