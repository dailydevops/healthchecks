namespace NetEvolve.HealthChecks.Tests.Unit.Redpanda;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Redpanda;
using NSubstitute;
using Xunit;

public sealed class RedpandaCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<RedpandaOptions>>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var check = new RedpandaCheck(serviceProvider, optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<RedpandaOptions>>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var check = new RedpandaCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("Test", check, null, null),
        };
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
        var optionsMonitor = Substitute.For<IOptionsMonitor<RedpandaOptions>>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var check = new RedpandaCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("Test", check, null, null),
        };

        // Act
        var result = await check.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Test: Missing configuration.", result.Description);
    }
}
