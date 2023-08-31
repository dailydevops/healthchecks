namespace NetEvolve.HealthChecks.Tests.Unit;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.XUnit;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[ExcludeFromCodeCoverage]
[UnitTest]
public sealed class ApplicationSelfCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenArgumentContextNull_ThrowException()
    {
        // Arrange
        var sut = new ApplicationHealthyCheck();

        // Act
        async Task Act() => await sut.CheckHealthAsync(null!).ConfigureAwait(false);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act).ConfigureAwait(false);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenArgumentCancellationToken_ReturnsHealthy()
    {
        // Arrange
        var sut = new ApplicationHealthyCheck();
        var cancellationToken = new CancellationToken();
        var context = new HealthCheckContext
        {
            Registration = new("Test", sut, HealthStatus.Unhealthy, null)
        };

        // Act
        var result = await sut.CheckHealthAsync(context, cancellationToken).ConfigureAwait(false);

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenArgumentCancellationTokenIsCancelled_ReturnsUnhealthy()
    {
        // Arrange
        var sut = new ApplicationHealthyCheck();
        var cancellationToken = new CancellationToken(true);
        var context = new HealthCheckContext
        {
            Registration = new("Test", sut, HealthStatus.Unhealthy, null)
        };

        // Act
        var result = await sut.CheckHealthAsync(context, cancellationToken).ConfigureAwait(false);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }
}
