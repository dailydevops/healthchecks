namespace NetEvolve.HealthChecks.Tests.Unit.Redis;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Redis;
using NSubstitute;
using Xunit;

[TestGroup(nameof(Redis))]
public sealed class RedisDatabaseCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<RedisDatabaseOptions>>();
        var check = new RedisDatabaseHealthCheck(serviceProvider, optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<RedisDatabaseOptions>>();
        var check = new RedisDatabaseHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext { Registration = new HealthCheckRegistration("Test", check, null, null) };
        var cancellationToken = new CancellationToken(true);

        // Act
        var result = await check.CheckHealthAsync(context, cancellationToken);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Test: Cancellation requested.", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenOptionsAreNull_ShouldReturnUnhealthy()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<RedisDatabaseOptions>>();
        var check = new RedisDatabaseHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext { Registration = new HealthCheckRegistration("Test", check, null, null) };

        // Act
        var result = await check.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Test: Missing configuration.", result.Description);
    }
}
