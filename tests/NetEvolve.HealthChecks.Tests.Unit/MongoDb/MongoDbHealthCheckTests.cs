namespace NetEvolve.HealthChecks.Tests.Unit.MongoDb;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.MongoDb;
using NSubstitute;

[TestGroup(nameof(MongoDb))]
public sealed class MongoDbHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<MongoDbOptions>>();
        var check = new MongoDbHealthCheck(optionsMonitor, serviceProvider);

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
        var optionsMonitor = Substitute.For<IOptionsMonitor<MongoDbOptions>>();

        var check = new MongoDbHealthCheck(optionsMonitor, serviceProvider);
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
        var optionsMonitor = Substitute.For<IOptionsMonitor<MongoDbOptions>>();

        var check = new MongoDbHealthCheck(optionsMonitor, serviceProvider);
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
        var options = new MongoDbOptions
        {
            KeyedService = "test-key",
            Timeout = 100,
            CommandAsync = async (MongoClient _, CancellationToken cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return new BsonDocument("test", 1);
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<MongoDbOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup client mock that returns success
        using var client = new MongoClient();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddKeyedSingleton("test-key", client);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new MongoDbHealthCheck(optionsMonitor, serviceProvider);
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
        var options = new MongoDbOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (MongoClient _, CancellationToken cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return new BsonDocument("test", 1);
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<MongoDbOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that returns success
        using var client = new MongoClient();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(client);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new MongoDbHealthCheck(optionsMonitor, serviceProvider);
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
        var options = new MongoDbOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (MongoClient _, CancellationToken cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                throw new MongoException("test");
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<MongoDbOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that throws an exception
        using var client = new MongoClient();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(client);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new MongoDbHealthCheck(optionsMonitor, serviceProvider);
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
        var options = new MongoDbOptions
        {
            KeyedService = null,
            Timeout = 1, // Very short timeout to force a timeout
            CommandAsync = async (MongoClient _, CancellationToken cancellationToken) =>
            {
                await Task.Delay(100, cancellationToken); // Simulate long-running command
                return new BsonDocument("test", 1);
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<MongoDbOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that delays long enough to cause timeout
        using var client = new MongoClient();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(client);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new MongoDbHealthCheck(optionsMonitor, serviceProvider);
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
