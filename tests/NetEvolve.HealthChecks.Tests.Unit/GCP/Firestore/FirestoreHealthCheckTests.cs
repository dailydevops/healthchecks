namespace NetEvolve.HealthChecks.Tests.Unit.GCP.Firestore;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.Firestore;
using NSubstitute;

[TestGroup(nameof(Firestore))]
public sealed class FirestoreHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<FirestoreOptions>>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var check = new FirestoreHealthCheck(optionsMonitor, serviceProvider);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<FirestoreOptions>>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var check = new FirestoreHealthCheck(optionsMonitor, serviceProvider);
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
        var optionsMonitor = Substitute.For<IOptionsMonitor<FirestoreOptions>>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var check = new FirestoreHealthCheck(optionsMonitor, serviceProvider);
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
