namespace NetEvolve.HealthChecks.MySql.Tests.Unit;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.MySql;
using NSubstitute;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[ExcludeFromCodeCoverage]
[UnitTest]
public sealed class MySqlCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<MySqlOptions>>();
        var check = new MySqlCheck(optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!).ConfigureAwait(false);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act).ConfigureAwait(false);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<MySqlOptions>>();
        var check = new MySqlCheck(optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("Test", check, null, null)
        };
        var cancellationToken = new CancellationToken(true);

        // Act
        var result = await check.CheckHealthAsync(context, cancellationToken).ConfigureAwait(false);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Test: Cancellation requested.", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenOptionsAreNull_ShouldReturnUnhealthy()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<MySqlOptions>>();
        var check = new MySqlCheck(optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("Test", check, null, null)
        };

        // Act
        var result = await check.CheckHealthAsync(context).ConfigureAwait(false);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Test: Missing configuration.", result.Description);
    }
}
