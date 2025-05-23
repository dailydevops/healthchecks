namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;
using NSubstitute;
using Xunit;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public sealed class ServiceBusTopicHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<ServiceBusTopicOptions>>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var check = new ServiceBusTopicHealthCheck(optionsMonitor, serviceProvider);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<ServiceBusTopicOptions>>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var check = new ServiceBusTopicHealthCheck(optionsMonitor, serviceProvider);
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
        var optionsMonitor = Substitute.For<IOptionsMonitor<ServiceBusTopicOptions>>();
        _ = optionsMonitor.Get("Test").Returns((ServiceBusTopicOptions)null!);
        var serviceProvider = Substitute.For<IServiceProvider>();
        var check = new ServiceBusTopicHealthCheck(optionsMonitor, serviceProvider);
        var context = new HealthCheckContext { Registration = new HealthCheckRegistration("Test", check, null, null) };

        // Act
        var result = await check.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Test: Missing configuration.", result.Description);
    }
}
