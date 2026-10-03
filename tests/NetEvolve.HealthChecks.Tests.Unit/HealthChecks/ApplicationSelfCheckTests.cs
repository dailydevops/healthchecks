namespace NetEvolve.HealthChecks.Tests.Unit.HealthChecks;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;

[TestGroup(nameof(HealthChecks))]
public sealed class ApplicationSelfCheckTests
{
    private const string TestName = nameof(HealthChecks);

    [Test]
    public async Task CheckHealthAsync_WhenArgumentContextNull_ThrowException(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Arrange
        var sut = new ApplicationHealthyCheck();

        // Act
        async Task Act() => await sut.CheckHealthAsync(null!, cancellationToken);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenArgumentCancellationToken_ReturnsHealthy(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Arrange
        var sut = new ApplicationHealthyCheck();
        var cancelledToken = new CancellationToken();
        var context = new HealthCheckContext { Registration = new(TestName, sut, HealthStatus.Unhealthy, null) };

        // Act
        var result = await sut.CheckHealthAsync(context, cancelledToken);

        // Assert
        _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
    }

    [Test]
    public async Task CheckHealthAsync_WhenArgumentCancellationTokenIsCancelled_ReturnsUnhealthy(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Arrange
        var sut = new ApplicationHealthyCheck();
        var cancelledToken = new CancellationToken(true);
        var context = new HealthCheckContext { Registration = new(TestName, sut, HealthStatus.Unhealthy, null) };

        // Act
        var result = await sut.CheckHealthAsync(context, cancelledToken);

        // Assert
        _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
    }
}
