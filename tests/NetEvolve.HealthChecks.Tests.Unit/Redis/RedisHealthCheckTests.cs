namespace NetEvolve.HealthChecks.Tests.Unit.Redis;

using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Redis;
using TUnit.Mocks;

[TestGroup(nameof(Redis))]
public sealed class RedisHealthCheckTests
{
    private const string TestName = nameof(Redis);

    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var serviceProvider = IServiceProvider.Mock();
        var optionsMonitor = IOptionsMonitor<RedisOptions>.Mock();
        using var check = new RedisHealthCheck(serviceProvider, optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var serviceProvider = IServiceProvider.Mock();
        var optionsMonitor = IOptionsMonitor<RedisOptions>.Mock();
        using var check = new RedisHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, check, null, null),
        };
        var cancellationToken = new CancellationToken(true);

        // Act
        var result = await check.CheckHealthAsync(context, cancellationToken);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{TestName}: Cancellation requested.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenOptionsAreNull_ShouldReturnUnhealthy()
    {
        // Arrange
        var serviceProvider = IServiceProvider.Mock();
        var optionsMonitor = IOptionsMonitor<RedisOptions>.Mock();
        using var check = new RedisHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, check, null, null),
        };

        // Act
        var result = await check.CheckHealthAsync(context);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{TestName}: Missing configuration.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenModeCreateAndServerUnreachable_ShouldNotCompleteSynchronously()
    {
        // Arrange
        // 192.0.2.1 is a non-routable TEST-NET-1 address (RFC 5737), so the connection attempt
        // reliably fails without ever reaching a real server.
        var options = new RedisOptions
        {
            ConnectionString = "192.0.2.1:6379,abortConnect=true,connectRetry=0,connectTimeout=500",
            Mode = ConnectionHandleMode.Create,
            Timeout = 500,
        };

        var optionsMonitor = IOptionsMonitor<RedisOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);
        var serviceProvider = IServiceProvider.Mock();
        using var check = new RedisHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, check, HealthStatus.Unhealthy, null),
        };

        // Act
        var resultTask = check.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        // The connection attempt must be asynchronous, so the returned task must not already be
        // completed the moment control returns to the caller. If it is, the calling thread was
        // blocked synchronously while establishing the connection.
        _ = await Assert.That(resultTask.IsCompleted).IsFalse();

        // Cleanup
        _ = await resultTask;
    }

    [Test]
    public async Task CheckHealthAsync_WhenModeCreateAndServerUnreachable_ShouldNotCacheFailedConnection()
    {
        // Arrange
        // A failed connection attempt must not be cached, otherwise every subsequent call would
        // immediately return the same failure, even after the server became reachable again.
        var options = new RedisOptions
        {
            ConnectionString = "192.0.2.1:6379,abortConnect=true,connectRetry=0,connectTimeout=500",
            Mode = ConnectionHandleMode.Create,
            Timeout = 500,
        };

        var optionsMonitor = IOptionsMonitor<RedisOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);
        var serviceProvider = IServiceProvider.Mock();
        using var check = new RedisHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, check, HealthStatus.Unhealthy, null),
        };

        // Act
        var firstResult = await check.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        _ = await Assert.That(firstResult.Status).IsEqualTo(HealthStatus.Unhealthy);

        // The faulted connection attempt is evicted asynchronously right after it faults, so
        // poll briefly for the internal cache to become empty again instead of asserting it
        // synchronously.
        var connectionsField = typeof(RedisHealthCheck).GetField(
            "_connections",
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        _ = await Assert.That(connectionsField).IsNotNull();

        var deadline = DateTime.UtcNow.AddSeconds(5);
        int count;
        do
        {
            var connections = (IDictionary?)connectionsField!.GetValue(check);
            count = connections?.Count ?? 0;
            if (count == 0)
            {
                break;
            }

            await Task.Delay(25, CancellationToken.None);
        } while (DateTime.UtcNow < deadline);

        _ = await Assert.That(count).IsEqualTo(0);
    }
}
