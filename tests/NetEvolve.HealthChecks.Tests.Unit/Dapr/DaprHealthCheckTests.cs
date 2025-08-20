namespace NetEvolve.HealthChecks.Tests.Unit.Dapr;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Dapr.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Dapr;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

[TestGroup(nameof(Dapr))]
public sealed class DaprHealthCheckTests
{
    [Test]
    public void Constructor_WhenServiceProviderNull_ThrowsArgumentNullException()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<DaprOptions>>();
        var serviceProvider = default(IServiceProvider);

        // Act
        void Act() => _ = new DaprHealthCheck(serviceProvider!, optionsMonitor);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("serviceProvider", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenDaprHealthy_ReturnsHealthy()
    {
        // Arrange
        var services = new ServiceCollection();

        var mockClient = Substitute.For<DaprClient>();
        _ = mockClient.CheckHealthAsync(Arg.Any<CancellationToken>()).Returns(true);

        _ = services.AddSingleton<DaprClient>(mockClient);
        _ = services.Configure("DaprSidecar", (DaprOptions o) => o.Timeout = 10000);

        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var healthCheck = new DaprHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("DaprSidecar", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo("DaprSidecar: Healthy");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenDaprUnhealthy_ReturnsDegraded()
    {
        // Arrange
        var services = new ServiceCollection();

        var mockClient = Substitute.For<DaprClient>();
        _ = mockClient.CheckHealthAsync(Arg.Any<CancellationToken>()).Returns(false);

        _ = services.AddSingleton<DaprClient>(mockClient);
        _ = services.Configure("DaprSidecar", (DaprOptions o) => o.Timeout = 0);

        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var healthCheck = new DaprHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("DaprSidecar", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Degraded);
            _ = await Assert.That(result.Description).IsEqualTo("DaprSidecar: Degraded");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenDaprTimeout_ReturnsDegraded()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new DaprOptions { Timeout = 10 }; // Very short timeout

        var mockClient = Substitute.For<DaprClient>();
        _ = mockClient
            .CheckHealthAsync(Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                await Task.Delay(1000); // Delay longer than the timeout
                return true;
            });

        _ = services.AddSingleton<DaprClient>(mockClient);
        _ = services.Configure("DaprSidecar", (DaprOptions o) => o.Timeout = options.Timeout);

        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var healthCheck = new DaprHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("DaprSidecar", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Degraded);
            _ = await Assert.That(result.Description).IsEqualTo("DaprSidecar: Degraded");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenDaprError_ReturnsUnhealthy()
    {
        // Arrange
        var services = new ServiceCollection();

        var mockClient = Substitute.For<DaprClient>();
        _ = mockClient
            .CheckHealthAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        _ = services.AddSingleton<DaprClient>(mockClient);
        _ = services.Configure("DaprSidecar", (DaprOptions o) => o.Timeout = 10000);

        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var healthCheck = new DaprHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("DaprSidecar", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert
                .That(result.Description!.Contains("DaprSidecar: Unexpected error.", StringComparison.Ordinal))
                .IsTrue();
            _ = await Assert.That(result.Exception).IsNotNull().And.IsTypeOf<InvalidOperationException>();
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenCancelled_ReturnsUnhealthy()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new DaprOptions { Timeout = 100 };

        var mockClient = Substitute.For<DaprClient>();
        _ = mockClient.CheckHealthAsync(Arg.Any<CancellationToken>()).Returns(true);

        _ = services.AddSingleton<DaprClient>(mockClient);
        _ = services.Configure("DaprSidecar", (DaprOptions o) => o.Timeout = options.Timeout);

        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var healthCheck = new DaprHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("DaprSidecar", healthCheck, HealthStatus.Unhealthy, null),
        };

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var result = await healthCheck.CheckHealthAsync(context, cts.Token);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo("DaprSidecar: Cancellation requested.");
        }
    }
}
