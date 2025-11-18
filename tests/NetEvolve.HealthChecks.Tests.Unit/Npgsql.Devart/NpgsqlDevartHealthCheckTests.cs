namespace NetEvolve.HealthChecks.Tests.Unit.Npgsql.Devart;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Npgsql.Devart;
using NSubstitute;

[TestGroup($"{nameof(Npgsql)}.{nameof(Devart)}")]
public sealed class NpgsqlDevartHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<NpgsqlDevartOptions>>();
        var check = new NpgsqlDevartHealthCheck(optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<NpgsqlDevartOptions>>();
        var check = new NpgsqlDevartHealthCheck(optionsMonitor);
        var context = new HealthCheckContext { Registration = new HealthCheckRegistration("Test", check, null, null) };
        var cancellationToken = new CancellationToken(true);

        // Act
        var result = await check.CheckHealthAsync(context, cancellationToken);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo("Test: Cancellation requested.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenOptionsAreNull_ShouldReturnUnhealthy()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<NpgsqlDevartOptions>>();
        var check = new NpgsqlDevartHealthCheck(optionsMonitor);
        var context = new HealthCheckContext { Registration = new HealthCheckRegistration("Test", check, null, null) };

        // Act
        var result = await check.CheckHealthAsync(context);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo("Test: Missing configuration.");
        }
    }
}
