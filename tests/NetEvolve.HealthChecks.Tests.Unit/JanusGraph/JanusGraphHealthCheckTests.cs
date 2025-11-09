namespace NetEvolve.HealthChecks.Tests.Unit.JanusGraph;

using System;
using System.Threading;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.JanusGraph;
using NSubstitute;

[TestGroup(nameof(JanusGraph))]
public sealed class JanusGraphHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<JanusGraphOptions>>();
        var check = new JanusGraphHealthCheck(serviceProvider, optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<JanusGraphOptions>>();

        var check = new JanusGraphHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext { Registration = new HealthCheckRegistration("Test", check, null, null) };
        var cancellationToken = new CancellationToken(true);

        // Act
        var result = await check.CheckHealthAsync(context, cancellationToken);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo("Test: Cancellation requested.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenOptionsAreNull_ShouldReturnUnhealthy()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<JanusGraphOptions>>();

        var check = new JanusGraphHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext { Registration = new HealthCheckRegistration("Test", check, null, null) };

        // Act
        var result = await check.CheckHealthAsync(context);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo("Test: Missing configuration.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WithKeyedService_ShouldUseKeyedService()
    {
        // Arrange
        var options = new JanusGraphOptions
        {
            KeyedService = "test-key",
            Timeout = 100,
            CommandAsync = async (_, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return new object();
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<JanusGraphOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var client = Substitute.For<IGremlinClient>();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddKeyedSingleton("test-key", client);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new JanusGraphHealthCheck(serviceProvider, optionsMonitor);
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
    public async Task CheckHealthAsync_WithoutKeyedService_ShouldUseDefaultService()
    {
        // Arrange
        var options = new JanusGraphOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (_, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return new object();
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<JanusGraphOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var client = Substitute.For<IGremlinClient>();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(client);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new JanusGraphHealthCheck(serviceProvider, optionsMonitor);
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
    public async Task CheckHealthAsync_WhenConnectionFails_ShouldReturnUnhealthy()
    {
        // Arrange
        var options = new JanusGraphOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (_, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                throw new InvalidOperationException("Connection failed");
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<JanusGraphOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var client = Substitute.For<IGremlinClient>();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(client);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new JanusGraphHealthCheck(serviceProvider, optionsMonitor);
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
            _ = await Assert.That(result.Description).Contains("test: Unexpected error.", StringComparison.Ordinal);
            _ = await Assert.That(result.Exception).IsNotNull();
        }
    }
}
