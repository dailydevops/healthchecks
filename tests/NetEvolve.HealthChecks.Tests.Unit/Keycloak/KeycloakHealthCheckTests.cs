namespace NetEvolve.HealthChecks.Tests.Unit.Keycloak;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Keycloak.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Keycloak;
using NSubstitute;

[TestGroup(nameof(Keycloak))]
public sealed class KeycloakHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<NetEvolve.HealthChecks.Keycloak.KeycloakOptions>>();
        var check = new KeycloakHealthCheck(serviceProvider, optionsMonitor);

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
        var optionsMonitor = Substitute.For<IOptionsMonitor<NetEvolve.HealthChecks.Keycloak.KeycloakOptions>>();

        var check = new KeycloakHealthCheck(serviceProvider, optionsMonitor);
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
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{testName}: Cancellation requested.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenOptionsAreNull_ShouldReturnUnhealthy()
    {
        // Arrange
        const string testName = "Test";

        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<NetEvolve.HealthChecks.Keycloak.KeycloakOptions>>();

        var check = new KeycloakHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(testName, check, null, null),
        };

        // Act
        var result = await check.CheckHealthAsync(context);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{testName}: Missing configuration.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WithKeyedService_ShouldUseKeyedService()
    {
        // Arrange
        const string testName = "Test";
        const string serviceKey = "test-key";

        var options = new NetEvolve.HealthChecks.Keycloak.KeycloakOptions
        {
            KeyedService = serviceKey,
            Timeout = 100,
            CommandAsync = async (_, _) =>
            {
                await Task.CompletedTask;
                return true;
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<NetEvolve.HealthChecks.Keycloak.KeycloakOptions>>();
        _ = optionsMonitor.Get(testName).Returns(options);

        // Setup client mock that returns success
        var client = new KeycloakClient("http://localhost/test", "test");

        var serviceProvider = new ServiceCollection()
            .AddKeyedSingleton(serviceKey, client)
            .AddSingleton<KeycloakClientProvider>()
            .BuildServiceProvider();

        var healthCheck = new KeycloakHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(testName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{testName}: Healthy");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WithoutKeyedService_ShouldUseDefaultService()
    {
        // Arrange
        const string testName = "Test";

        var options = new NetEvolve.HealthChecks.Keycloak.KeycloakOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (_, _) =>
            {
                await Task.CompletedTask;
                return true;
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<NetEvolve.HealthChecks.Keycloak.KeycloakOptions>>();
        _ = optionsMonitor.Get(testName).Returns(options);

        // Setup connection mock that returns success
        var client = new KeycloakClient("http://localhost/test", "test");

        var serviceProvider = new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton<KeycloakClientProvider>()
            .BuildServiceProvider();

        var healthCheck = new KeycloakHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(testName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{testName}: Healthy");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenConnectionFails_ShouldReturnUnhealthy()
    {
        // Arrange
        const string testName = "Test";

        var options = new NetEvolve.HealthChecks.Keycloak.KeycloakOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (_, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                throw new InvalidOperationException("Keycloak unhealthy test");
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<NetEvolve.HealthChecks.Keycloak.KeycloakOptions>>();
        _ = optionsMonitor.Get(testName).Returns(options);

        // Setup connection mock that throws an exception
        var client = new KeycloakClient("http://localhost/test", "test");

        var serviceProvider = new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton<KeycloakClientProvider>()
            .BuildServiceProvider();

        var healthCheck = new KeycloakHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(testName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert
                .That(result.Description)
                .Contains($"{testName}: Unexpected error.", StringComparison.Ordinal);
            _ = await Assert.That(result.Exception).IsNotNull();
        }
    }
}
