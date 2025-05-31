namespace NetEvolve.HealthChecks.Tests.Unit.HealthChecks;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using NetEvolve.Extensions.TUnit;

[TestGroup(nameof(HealthChecks))]
public sealed class ApplicationReadinessCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WhenArgumentContextNull_ThrowException()
    {
        // Arrange
        using var lifetime = new TestHostApplicationLifeTime();
        var sut = new ApplicationReadyCheck(lifetime);

        // Act
        async Task Act() => await sut.CheckHealthAsync(null!);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenArgumentCancellationToken_ReturnsHealthy()
    {
        // Arrange
        using var lifetime = new TestHostApplicationLifeTime();
        var sut = new ApplicationReadyCheck(lifetime);
        var cancellationToken = new CancellationToken();
        var context = new HealthCheckContext { Registration = new("Test", sut, HealthStatus.Unhealthy, null) };

        // Act
        lifetime.StartApplication();
        var result = await sut.CheckHealthAsync(context, cancellationToken);

        // Assert
        _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
    }

    [Test]
    public async Task CheckHealthAsync_WhenArgumentCancellationTokenIsCancelled_ReturnsUnhealthy()
    {
        // Arrange
        using var lifetime = new TestHostApplicationLifeTime();
        var sut = new ApplicationReadyCheck(lifetime);
        var cancellationToken = new CancellationToken(true);
        var context = new HealthCheckContext { Registration = new("Test", sut, HealthStatus.Unhealthy, null) };

        // Act
        var result = await sut.CheckHealthAsync(context, cancellationToken);

        // Assert
        _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
    }

    [Test]
    public async Task CheckHealthAsync_WhenApplicationStopped_ReturnsUnhealthy()
    {
        // Arrange
        using var lifetime = new TestHostApplicationLifeTime();
        var sut = new ApplicationReadyCheck(lifetime);
        var cancellationToken = new CancellationToken();
        var context = new HealthCheckContext { Registration = new("Test", sut, HealthStatus.Unhealthy, null) };

        // Act
        lifetime.StartApplication();

        // Assert
        var result = await sut.CheckHealthAsync(context, cancellationToken);
        _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);

        lifetime.StopApplication();
        result = await sut.CheckHealthAsync(context, cancellationToken);
        _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
    }

    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "False positive")]
    private sealed class TestHostApplicationLifeTime : IHostApplicationLifetime, IDisposable
    {
        private readonly CancellationTokenSource _sourceStarted = new();
        private readonly CancellationTokenSource _sourceStopping = new();
        private readonly CancellationTokenSource _sourceStopped = new();

        public CancellationToken ApplicationStarted => _sourceStarted.Token;

        public CancellationToken ApplicationStopped => _sourceStopped.Token;

        public CancellationToken ApplicationStopping => _sourceStopping.Token;

        public void StartApplication() => ExecuteHandlers(_sourceStarted);

        public void StopApplication()
        {
            ExecuteHandlers(_sourceStopping);

            ExecuteHandlers(_sourceStopped);
        }

        private static void ExecuteHandlers(CancellationTokenSource source)
        {
            if (source.IsCancellationRequested)
            {
                return;
            }

            source.Cancel(throwOnFirstException: false);
        }

        public void Dispose()
        {
            _sourceStarted.Dispose();
            _sourceStopping.Dispose();
            _sourceStopped.Dispose();
        }
    }
}
