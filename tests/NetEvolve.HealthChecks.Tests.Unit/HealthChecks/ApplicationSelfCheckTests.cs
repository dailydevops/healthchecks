namespace NetEvolve.HealthChecks.Tests.Unit.HealthChecks;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.XUnit;
using Xunit;

[TestGroup(nameof(HealthChecks))]
public sealed class ApplicationSelfCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenArgumentContextNull_ThrowException()
    {
        // Arrange
        var sut = new ApplicationHealthyCheck();

        // Act
        async Task Act() => await sut.CheckHealthAsync(null!);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenArgumentCancellationToken_ReturnsHealthy()
    {
        // Arrange
        var sut = new ApplicationHealthyCheck();
        var cancellationToken = new CancellationToken();
        var context = new HealthCheckContext { Registration = new("Test", sut, HealthStatus.Unhealthy, null) };

        // Act
        var result = await sut.CheckHealthAsync(context, cancellationToken);

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenArgumentCancellationTokenIsCancelled_ReturnsUnhealthy()
    {
        // Arrange
        var sut = new ApplicationHealthyCheck();
        var cancellationToken = new CancellationToken(true);
        var context = new HealthCheckContext { Registration = new("Test", sut, HealthStatus.Unhealthy, null) };

        // Act
        var result = await sut.CheckHealthAsync(context, cancellationToken);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }
}
