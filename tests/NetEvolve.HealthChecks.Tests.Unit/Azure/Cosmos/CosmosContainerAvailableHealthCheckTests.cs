namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Cosmos;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Cosmos;
using Xunit;

[TestGroup($"{nameof(Azure)}.{nameof(Cosmos)}")]
public sealed class CosmosContainerAvailableHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenContextIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddOptions();
        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<CosmosContainerAvailableOptions>>();
        var sut = new CosmosContainerAvailableHealthCheck(serviceProvider, optionsMonitor);

        // Act
        async Task Act() => await sut.CheckHealthAsync(null!);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenCancellationRequested_ReturnsFailureStatus()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddOptions();
        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<CosmosContainerAvailableOptions>>();
        var sut = new CosmosContainerAvailableHealthCheck(serviceProvider, optionsMonitor);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", sut, HealthStatus.Degraded, Array.Empty<string>()),
        };

        // Act
        var result = await sut.CheckHealthAsync(context, cts.Token);

        // Assert
        Assert.Equal(HealthStatus.Degraded, result.Status);
        Assert.Equal("test: Cancellation requested.", result.Description);
    }
}
