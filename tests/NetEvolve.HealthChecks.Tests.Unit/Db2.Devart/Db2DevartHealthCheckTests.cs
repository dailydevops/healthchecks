namespace NetEvolve.HealthChecks.Tests.Unit.Db2.Devart;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Db2.Devart;
using NSubstitute;

[TestGroup($"{nameof(Db2)}.{nameof(Devart)}")]
public sealed class Db2DevartHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<Db2DevartOptions>>();
        var check = new Db2DevartHealthCheck(optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<Db2DevartOptions>>();
        var check = new Db2DevartHealthCheck(optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("Test", check, HealthStatus.Unhealthy, [])
        };

        using var tokenSource = new CancellationTokenSource();
        tokenSource.Cancel();

        // Act
        var result = await check.CheckHealthAsync(context, tokenSource.Token);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo("Db2DevartHealthCheck: Canceled");
        }
    }
}