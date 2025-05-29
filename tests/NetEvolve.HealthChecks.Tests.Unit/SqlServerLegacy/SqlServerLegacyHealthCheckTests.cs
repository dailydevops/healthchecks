namespace NetEvolve.HealthChecks.Tests.Unit.SqlServerLegacy;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.SqlServer.Legacy;
using NSubstitute;
using Xunit;

[TestGroup(nameof(SqlServerLegacy))]
public sealed class SqlServerLegacyHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<SqlServerLegacyOptions>>();
        var check = new SqlServerLegacyHealthCheck(optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<SqlServerLegacyOptions>>();
        var check = new SqlServerLegacyHealthCheck(optionsMonitor);
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
        var optionsMonitor = Substitute.For<IOptionsMonitor<SqlServerLegacyOptions>>();
        var check = new SqlServerLegacyHealthCheck(optionsMonitor);
        var context = new HealthCheckContext { Registration = new HealthCheckRegistration("Test", check, null, null) };

        // Act
        var result = await check.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Test: Missing configuration.", result.Description);
    }
}
