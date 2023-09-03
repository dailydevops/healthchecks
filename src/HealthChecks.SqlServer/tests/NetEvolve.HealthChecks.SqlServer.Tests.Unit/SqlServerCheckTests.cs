namespace NetEvolve.HealthChecks.SqlServer.Tests.Unit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.XUnit;
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
        var services = new ServiceCollection();
        _ = services.AddSingleton<IHealthCheck, SqlServerCheck>().AddOptions();
        var provider = services.BuildServiceProvider();
        var check = provider.GetRequiredService<IHealthCheck>();

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!).ConfigureAwait(false);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act).ConfigureAwait(false);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldBeUnhealthy()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IHealthCheck, SqlServerCheck>().AddOptions();
        var provider = services.BuildServiceProvider();
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("Test", provider.GetRequiredService<IHealthCheck>(), null, null)
        };
        var check = provider.GetRequiredService<IHealthCheck>();
        var cancellationToken = new CancellationToken(true);

        // Act
        var result = await check.CheckHealthAsync(context, cancellationToken).ConfigureAwait(false);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Test: Cancellation requested", result.Description);
    }
}
