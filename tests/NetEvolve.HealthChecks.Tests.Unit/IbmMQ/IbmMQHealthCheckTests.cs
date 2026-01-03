namespace NetEvolve.HealthChecks.Tests.Unit.IbmMQ;

using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using IBM.WMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.IbmMQ;
using NSubstitute;

[TestGroup(nameof(IbmMQ))]
public sealed class IbmMQHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WithConnectedQueueManager_ReturnsHealthy()
    {
        // Arrange
        var options = new IbmMQOptions { KeyedService = null, Timeout = 10000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<IbmMQOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var properties = new Hashtable { { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_BINDINGS } };
        using var mockQueueManager = new MQQueueManager("QM_TEST", properties);

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
    public async Task CheckHealthAsync_WithKeyedService_UsesKeyedService()
    {
        // Arrange
        var options = new IbmMQOptions { KeyedService = "test-key", Timeout = 10000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<IbmMQOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var properties = new Hashtable { { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_BINDINGS } };
        using var mockQueueManager = new MQQueueManager("QM_TEST", properties);

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
    public async Task CheckHealthAsync_WhenExceptionThrown_ReturnsUnhealthy()
    {
        // Arrange
        var options = new IbmMQOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<IbmMQOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Create a queue manager factory that throws an exception
        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<MQQueueManager>(sp =>
        {
            throw new InvalidOperationException("Connection failed");
        });
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
            _ = await Assert
                .That(result.Description)
                .IsEqualTo("test: Unexpected error.", StringComparison.Ordinal);
            _ = await Assert.That(result.Exception).IsNotNull();
        }
    }

    [Test]
    public async Task CheckHealthAsync_WithVeryShortTimeout_ReturnsDegraded()
    {
        // Arrange
        var options = new IbmMQOptions { KeyedService = null, Timeout = 0 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<IbmMQOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var properties = new Hashtable { { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_BINDINGS } };
        using var mockQueueManager = new MQQueueManager("QM_TEST", properties);

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
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Degraded);
            _ = await Assert.That(result.Description).IsEqualTo("test: Degraded", StringComparison.Ordinal);
        }
    }
}
