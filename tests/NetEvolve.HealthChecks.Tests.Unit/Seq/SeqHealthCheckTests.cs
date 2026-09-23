namespace NetEvolve.HealthChecks.Tests.Unit.Seq;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Seq.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Seq;
using TUnit.Mocks;

[TestGroup(nameof(Seq))]
public sealed class SeqHealthCheckTests
{
    private const string TestName = nameof(Seq);

    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var serviceProvider = IServiceProvider.Mock();
        var optionsMonitor = IOptionsMonitor<NetEvolve.HealthChecks.Seq.SeqOptions>.Mock();
        var check = new SeqHealthCheck(serviceProvider, optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Arrange

        var serviceProvider = IServiceProvider.Mock();
        var optionsMonitor = IOptionsMonitor<NetEvolve.HealthChecks.Seq.SeqOptions>.Mock();

        var check = new SeqHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, check, null, null),
        };
        var cancelledToken = new CancellationToken(true);

        // Act
        var result = await check.CheckHealthAsync(context, cancelledToken);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{TestName}: Cancellation requested.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenOptionsAreNull_ShouldReturnUnhealthy(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Arrange

        var serviceProvider = IServiceProvider.Mock();
        var optionsMonitor = IOptionsMonitor<NetEvolve.HealthChecks.Seq.SeqOptions>.Mock();

        var check = new SeqHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, check, null, null),
        };

        // Act
        var result = await check.CheckHealthAsync(context, cancellationToken);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{TestName}: Missing configuration.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WithKeyedService_ShouldUseKeyedService(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Arrange
        const string serviceKey = "test-key";

        var options = new NetEvolve.HealthChecks.Seq.SeqOptions
        {
            KeyedService = serviceKey,
            Timeout = 100,
            CommandAsync = async (_, _) =>
            {
                await Task.CompletedTask;
                return true;
            },
        };

        var optionsMonitor = IOptionsMonitor<NetEvolve.HealthChecks.Seq.SeqOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup connection mock that returns success
        using var connection = new SeqConnection("http://localhost/test", "test");

        var serviceProvider = new ServiceCollection()
            .AddKeyedSingleton(serviceKey, connection)
            .AddSingleton<SeqClientProvider>()
            .BuildServiceProvider();

        var healthCheck = new SeqHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, cancellationToken);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{TestName}: Healthy");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WithoutKeyedService_ShouldUseDefaultService(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Arrange
        var options = new NetEvolve.HealthChecks.Seq.SeqOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (_, _) =>
            {
                await Task.CompletedTask;
                return true;
            },
        };

        var optionsMonitor = IOptionsMonitor<NetEvolve.HealthChecks.Seq.SeqOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup connection mock that returns success
        using var connection = new SeqConnection("http://localhost/test", "test");

        var serviceProvider = new ServiceCollection()
            .AddSingleton(connection)
            .AddSingleton<SeqClientProvider>()
            .BuildServiceProvider();

        var healthCheck = new SeqHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, cancellationToken);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{TestName}: Healthy");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenConnectionFails_ShouldReturnUnhealthy(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Arrange

        var options = new NetEvolve.HealthChecks.Seq.SeqOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (_, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                throw new InvalidOperationException("Seq unhealthy test");
            },
        };

        var optionsMonitor = IOptionsMonitor<NetEvolve.HealthChecks.Seq.SeqOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        // Setup connection mock that throws an exception
        using var connection = new SeqConnection("http://localhost/test", "test");

        var serviceProvider = new ServiceCollection()
            .AddSingleton(connection)
            .AddSingleton<SeqClientProvider>()
            .BuildServiceProvider();

        var healthCheck = new SeqHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, cancellationToken);

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
