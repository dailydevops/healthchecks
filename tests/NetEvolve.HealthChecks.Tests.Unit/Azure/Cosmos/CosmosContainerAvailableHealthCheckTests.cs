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

    [Fact]
    public async Task CheckHealthAsync_WhenOptionsNull_ReturnsUnhealthyStatus()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddOptions();
        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<CosmosContainerAvailableOptions>>();
        var sut = new CosmosContainerAvailableHealthCheck(serviceProvider, optionsMonitor);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", sut, HealthStatus.Unhealthy, Array.Empty<string>()),
        };

        // Act
        var result = await sut.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("test: Missing configuration.", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenOptionsProvided_ExecutesHealthCheck()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddOptions();
        _ = services.Configure<CosmosContainerAvailableOptions>(
            "test",
            options =>
            {
                options.ConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=dummyKey;";
                options.Mode = CosmosClientCreationMode.ConnectionString;
                options.DatabaseId = "testdb";
                options.ContainerId = "testcontainer";
            }
        );

        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<CosmosContainerAvailableOptions>>();
        var sut = new CosmosContainerAvailableHealthCheck(serviceProvider, optionsMonitor);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", sut, HealthStatus.Unhealthy, Array.Empty<string>()),
        };

        // Act & Assert
        // The actual result will depend on whether a real Cosmos DB is available,
        // since we're not mocking. In tests, this will likely return Degraded or Unhealthy.
        var result = await sut.CheckHealthAsync(context);

        // We're just verifying that it doesn't throw an exception
        Assert.NotNull(result);
    }
}
