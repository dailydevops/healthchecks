namespace NetEvolve.HealthChecks.Tests.Unit.RavenDb;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.RavenDb;
using NSubstitute;
using Raven.Client.Documents;
using Raven.Client.Exceptions;

[TestGroup(nameof(RavenDb))]
public sealed class RavenDbHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<RavenDbOptions>>();
        var check = new RavenDbHealthCheck(optionsMonitor, serviceProvider);

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
        var optionsMonitor = Substitute.For<IOptionsMonitor<RavenDbOptions>>();

        var check = new RavenDbHealthCheck(optionsMonitor, serviceProvider);
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
        var optionsMonitor = Substitute.For<IOptionsMonitor<RavenDbOptions>>();

        var check = new RavenDbHealthCheck(optionsMonitor, serviceProvider);
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
        var options = new RavenDbOptions
        {
            KeyedService = "test-key",
            Timeout = 100,
            CommandAsync = async (IDocumentStore _1, CancellationToken _2) =>
            {
                await Task.CompletedTask;
                return true;
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<RavenDbOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup client mock that returns success
        var store = Substitute.For<IDocumentStore>();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddKeyedSingleton("test-key", store);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new RavenDbHealthCheck(optionsMonitor, serviceProvider);
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
        var options = new RavenDbOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (IDocumentStore _1, CancellationToken _2) =>
            {
                await Task.CompletedTask;
                return true;
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<RavenDbOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that returns success
        var store = Substitute.For<IDocumentStore>();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(store);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new RavenDbHealthCheck(optionsMonitor, serviceProvider);
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
        var options = new RavenDbOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (IDocumentStore _1, CancellationToken _2) =>
            {
                await Task.CompletedTask;
                throw new RavenException("Test exception");
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<RavenDbOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that throws an exception
        var store = Substitute.For<IDocumentStore>();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(store);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new RavenDbHealthCheck(optionsMonitor, serviceProvider);
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

    [Test]
    public async Task CheckHealthAsync_WhenTimeout_ShouldReturnDegraded()
    {
        // Arrange
        var options = new RavenDbOptions
        {
            KeyedService = null,
            Timeout = 1, // Very short timeout to force a timeout
            CommandAsync = async (IDocumentStore _1, CancellationToken cancellationToken) =>
            {
                await Task.Delay(100, cancellationToken); // Simulate long-running command
                return true;
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<RavenDbOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that delays long enough to cause timeout
        var store = Substitute.For<IDocumentStore>();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(store);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new RavenDbHealthCheck(optionsMonitor, serviceProvider);
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
            _ = await Assert.That(result.Description).Contains("test: Degraded", StringComparison.Ordinal);
        }
    }
}
