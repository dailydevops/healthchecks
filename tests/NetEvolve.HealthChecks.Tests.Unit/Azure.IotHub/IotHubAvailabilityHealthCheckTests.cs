namespace NetEvolve.HealthChecks.Tests.Unit.Azure.IotHub;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.IotHub;
using NSubstitute;

[TestGroup($"{nameof(Azure)}.{nameof(IotHub)}")]
public sealed class IotHubAvailabilityHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<IotHubAvailabilityOptions>>();
        var clientFactory = Substitute.For<IotHubClientFactory>();
        var check = new IotHubAvailabilityHealthCheck(optionsMonitor, clientFactory);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<IotHubAvailabilityOptions>>();
        var clientFactory = Substitute.For<IotHubClientFactory>();
        var check = new IotHubAvailabilityHealthCheck(optionsMonitor, clientFactory);
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
        var optionsMonitor = Substitute.For<IOptionsMonitor<IotHubAvailabilityOptions>>();
        _ = optionsMonitor.Get("Test").Returns((IotHubAvailabilityOptions?)null);

        var clientFactory = Substitute.For<IotHubClientFactory>();
        var check = new IotHubAvailabilityHealthCheck(optionsMonitor, clientFactory);
        var context = new HealthCheckContext { Registration = new HealthCheckRegistration("Test", check, null, null) };

        // Act
        var result = await check.CheckHealthAsync(context, default);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo("Test: Missing configuration.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenValidationFailed_ShouldReturnUnhealthy()
    {
        // Arrange
        var options = new IotHubAvailabilityOptions
        {
            Mode = null, // Invalid mode
        };
        var optionsMonitor = Substitute.For<IOptionsMonitor<IotHubAvailabilityOptions>>();
        _ = optionsMonitor.Get("Test").Returns(options);

        var clientFactory = Substitute.For<IotHubClientFactory>();
        var check = new IotHubAvailabilityHealthCheck(optionsMonitor, clientFactory);
        var context = new HealthCheckContext { Registration = new HealthCheckRegistration("Test", check, null, null) };

        // Act
        var result = await check.CheckHealthAsync(context, default);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo("Test: The client creation mode cannot be null.");
        }
    }
}
