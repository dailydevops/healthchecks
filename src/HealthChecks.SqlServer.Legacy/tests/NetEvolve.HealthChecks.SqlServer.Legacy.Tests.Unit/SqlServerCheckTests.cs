namespace NetEvolve.HealthChecks.SqlServer.Legacy.Tests.Unit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[ExcludeFromCodeCoverage]
[UnitTest]
public sealed class SqlServerCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<SqlServerLegacyOptions>>();
        var check = new SqlServerLegacyCheck(optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!).ConfigureAwait(false);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act).ConfigureAwait(false);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<SqlServerLegacyOptions>>();
        var check = new SqlServerLegacyCheck(optionsMonitor);
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
        var optionsMonitor = Substitute.For<IOptionsMonitor<SqlServerLegacyOptions>>();
        var check = new SqlServerLegacyCheck(optionsMonitor);
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
