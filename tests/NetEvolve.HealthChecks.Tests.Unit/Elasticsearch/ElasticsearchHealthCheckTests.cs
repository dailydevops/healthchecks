namespace NetEvolve.HealthChecks.Tests.Unit.Elasticsearch;

using System;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Elasticsearch;
using NSubstitute;

[TestGroup(nameof(Elasticsearch))]
public sealed class ElasticsearchHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<ElasticsearchOptions>>();
        var check = new ElasticsearchHealthCheck(optionsMonitor, serviceProvider);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        const string testName = "Test";
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<ElasticsearchOptions>>();

        var check = new ElasticsearchHealthCheck(optionsMonitor, serviceProvider);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(testName, check, null, null),
        };
        var cancellationToken = new CancellationToken(true);

        // Act
        var result = await check.CheckHealthAsync(context, cancellationToken);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert
                .That(result.Status)
                .IsEqualTo(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{testName}: Cancellation requested.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenOptionsAreNull_ShouldReturnUnhealthy()
    {
        // Arrange
        const string testName = "Test";
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<ElasticsearchOptions>>();

        var check = new ElasticsearchHealthCheck(optionsMonitor, serviceProvider);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(testName, check, null, null),
        };

        // Act
        var result = await check.CheckHealthAsync(context);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert
                .That(result.Status)
                .IsEqualTo(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{testName}: Missing configuration.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WithKeyedService_ShouldUseKeyedService()
    {
        // Arrange
        const string testName = "Test";
        const string serviceKey = "test-key";

        var options = new ElasticsearchOptions
        {
            KeyedService = serviceKey,
            Timeout = 100,
            CommandAsync = async (_, _) =>
            {
                await Task.CompletedTask;
                return true;
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<ElasticsearchOptions>>();
        _ = optionsMonitor.Get(testName).Returns(options);

        // Setup client mock that returns success
        var uri = Substitute.For<Uri>("http://localhost/test");
        var settings = Substitute.For<ElasticsearchClientSettings>(uri);
        var client = new ElasticsearchClient(settings);

        var serviceProvider = new ServiceCollection().AddKeyedSingleton(serviceKey, client).BuildServiceProvider();

        var healthCheck = new ElasticsearchHealthCheck(optionsMonitor, serviceProvider);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(
                testName,
                healthCheck,
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                null
            ),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert
                .That(result.Status)
                .IsEqualTo(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{testName}: Healthy");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WithoutKeyedService_ShouldUseDefaultService()
    {
        // Arrange
        const string testName = "Test";

        var options = new ElasticsearchOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (_, _) =>
            {
                await Task.CompletedTask;
                return true;
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<ElasticsearchOptions>>();
        _ = optionsMonitor.Get(testName).Returns(options);

        // Setup connection mock that returns success
        var uri = Substitute.For<Uri>("http://localhost/test");
        var settings = Substitute.For<ElasticsearchClientSettings>(uri);
        var client = new ElasticsearchClient(settings);

        var serviceProvider = new ServiceCollection().AddSingleton(client).BuildServiceProvider();

        var healthCheck = new ElasticsearchHealthCheck(optionsMonitor, serviceProvider);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(
                testName,
                healthCheck,
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                null
            ),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert
                .That(result.Status)
                .IsEqualTo(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{testName}: Healthy");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenConnectionFails_ShouldReturnUnhealthy()
    {
        // Arrange
        const string testName = "Test";

        var options = new ElasticsearchOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (_, _) =>
            {
                await Task.CompletedTask;
                throw new InvalidOperationException("Elasticsearch unhealthy test");
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<ElasticsearchOptions>>();
        _ = optionsMonitor.Get(testName).Returns(options);

        // Setup connection mock that throws an exception
        var uri = Substitute.For<Uri>("http://localhost/test");
        var settings = Substitute.For<ElasticsearchClientSettings>(uri);
        var client = new ElasticsearchClient(settings);

        var serviceProvider = new ServiceCollection().AddSingleton(client).BuildServiceProvider();

        var healthCheck = new ElasticsearchHealthCheck(optionsMonitor, serviceProvider);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(
                testName,
                healthCheck,
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                null
            ),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert
                .That(result.Status)
                .IsEqualTo(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy);
            _ = await Assert
                .That(result.Description)
                .Contains($"{testName}: Unexpected error.", StringComparison.Ordinal);
            _ = await Assert.That(result.Exception).IsNotNull();
        }
    }
}
