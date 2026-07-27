namespace NetEvolve.HealthChecks.Tests.Unit.Couchbase;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Couchbase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Couchbase;
using TUnit.Mocks;

[TestGroup(nameof(Couchbase))]
public sealed class CouchbaseHealthCheckTests
{
    private const string TestName = nameof(Couchbase);

    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var serviceProvider = IServiceProvider.Mock();
        var optionsMonitor = IOptionsMonitor<CouchbaseOptions>.Mock();
        var check = new CouchbaseHealthCheck(serviceProvider, optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var serviceProvider = IServiceProvider.Mock();
        var optionsMonitor = IOptionsMonitor<CouchbaseOptions>.Mock();

        var check = new CouchbaseHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, check, null, null),
        };
        var cancellationToken = new CancellationToken(true);

        // Act
        var result = await check.CheckHealthAsync(context, cancellationToken);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{TestName}: Cancellation requested.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenOptionsAreNull_ShouldReturnUnhealthy()
    {
        // Arrange
        var serviceProvider = IServiceProvider.Mock();
        var optionsMonitor = IOptionsMonitor<CouchbaseOptions>.Mock();

        var check = new CouchbaseHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, check, null, null),
        };

        // Act
        var result = await check.CheckHealthAsync(context);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{TestName}: Missing configuration.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WithKeyedService_ShouldUseKeyedService()
    {
        // Arrange
        var options = new CouchbaseOptions
        {
            KeyedService = "test-key",
            Timeout = 100,
            CommandAsync = async (_, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return true;
            },
        };

        var optionsMonitor = IOptionsMonitor<CouchbaseOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        var cluster = ICluster.Mock();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddKeyedSingleton<ICluster>("test-key", cluster);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new CouchbaseHealthCheck(serviceProvider, optionsMonitor);
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
    public async Task CheckHealthAsync_WithoutKeyedService_ShouldUseDefaultService()
    {
        // Arrange
        var options = new CouchbaseOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (_, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return true;
            },
        };

        var optionsMonitor = IOptionsMonitor<CouchbaseOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        var cluster = ICluster.Mock();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<ICluster>(cluster);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new CouchbaseHealthCheck(serviceProvider, optionsMonitor);
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
    public async Task CheckHealthAsync_WhenConnectionFails_ShouldReturnUnhealthy()
    {
        // Arrange
        var options = new CouchbaseOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (_, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                throw new InvalidOperationException("test");
            },
        };

        var optionsMonitor = IOptionsMonitor<CouchbaseOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        var cluster = ICluster.Mock();

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<ICluster>(cluster);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new CouchbaseHealthCheck(serviceProvider, optionsMonitor);
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
                .Contains($"{TestName}: Unexpected error.", StringComparison.Ordinal);
            _ = await Assert.That(result.Exception).IsNotNull();
        }
    }
}
