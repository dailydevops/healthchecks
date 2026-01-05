namespace NetEvolve.HealthChecks.Tests.Unit.Mosquitto;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MQTTnet;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Mosquitto;
using NSubstitute;

[TestGroup(nameof(Mosquitto))]
public sealed class MosquittoHealthCheckTests
{
    private const string TestName = nameof(Mosquitto);

    [Test]
    public async Task CheckHealthAsync_WithKeyedService_UsesKeyedService()
    {
        // Arrange
        var options = new MosquittoOptions { KeyedService = "test-key", Timeout = 10000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<MosquittoOptions>>();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup client mock that returns success
        var mockClient = Substitute.For<IMqttClient>();
        _ = mockClient.IsConnected.Returns(true);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddKeyedSingleton("test-key", mockClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new MosquittoHealthCheck(serviceProvider, optionsMonitor);
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
        var options = new MosquittoOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<MosquittoOptions>>();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup client mock that returns success
        var mockClient = Substitute.For<IMqttClient>();
        _ = mockClient.IsConnected.Returns(true);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new MosquittoHealthCheck(serviceProvider, optionsMonitor);
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
    public async Task CheckHealthAsync_WhenClientNotConnected_ReturnsUnhealthy()
    {
        // Arrange
        var options = new MosquittoOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<MosquittoOptions>>();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup client mock that returns not connected
        var mockClient = Substitute.For<IMqttClient>();
        _ = mockClient.IsConnected.Returns(false);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new MosquittoHealthCheck(serviceProvider, optionsMonitor);
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
                .IsEqualTo($"{TestName}: Mosquitto health check failed.", StringComparison.Ordinal);
        }
    }
}
