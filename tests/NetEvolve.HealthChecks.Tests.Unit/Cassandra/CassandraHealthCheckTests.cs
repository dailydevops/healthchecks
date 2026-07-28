namespace NetEvolve.HealthChecks.Tests.Unit.Cassandra;

using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Cassandra;
using TUnit.Mocks;
using CassandraDriver = global::Cassandra;

[TestGroup(nameof(Cassandra))]
public class CassandraHealthCheckTests
{
    private const string TestName = nameof(Cassandra);

    // CassandraDriver.RowSet is a concrete class whose GetEnumerator() reads from a protected
    // RowQueue property; TUnit.Mocks' partial-mock subclass triggers a virtual-dispatch NRE
    // because the driver's own constructor assigns RowQueue before the mock's tracking state is
    // initialized. Constructing a real RowSet and populating RowQueue via reflection sidesteps
    // the generator bug entirely while still exercising real driver enumeration behavior.
    private static CassandraDriver.RowSet CreateRowSet(params CassandraDriver.Row[] rows)
    {
        var rowSet = new CassandraDriver.RowSet();
        var rowQueueProperty = typeof(CassandraDriver.RowSet).GetProperty(
            "RowQueue",
            BindingFlags.Instance | BindingFlags.NonPublic
        )!;
        rowQueueProperty.SetValue(rowSet, new ConcurrentQueue<CassandraDriver.Row>(rows));
        return rowSet;
    }

    [Test]
    public async Task DefaultCommandAsync_WhenClusterAvailable_ReturnsTrue()
    {
        // Arrange
        var cluster = CassandraDriver.ICluster.Mock();
        var session = CassandraDriver.ISession.Mock();
        using var rowSet = CreateRowSet(new CassandraDriver.Row());

        _ = cluster.ConnectAsync().Returns(session);
        _ = session.ExecuteAsync(Any<CassandraDriver.IStatement>()).Returns(rowSet);

        // Act
        var result = await CassandraHealthCheck.DefaultCommandAsync(cluster, CancellationToken.None);

        // Assert
        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task DefaultCommandAsync_WhenResultIsNull_ReturnsFalse()
    {
        // Arrange
        var cluster = CassandraDriver.ICluster.Mock();
        var session = CassandraDriver.ISession.Mock();

        _ = cluster.ConnectAsync().Returns(session);
        _ = session.ExecuteAsync(Any<CassandraDriver.IStatement>()).Returns((CassandraDriver.RowSet)null!);

        // Act
        var result = await CassandraHealthCheck.DefaultCommandAsync(cluster, CancellationToken.None);

        // Assert
        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task DefaultCommandAsync_WhenResultIsEmpty_ReturnsFalse()
    {
        // Arrange
        var cluster = CassandraDriver.ICluster.Mock();
        var session = CassandraDriver.ISession.Mock();
        using var rowSet = CreateRowSet();

        _ = cluster.ConnectAsync().Returns(session);
        _ = session.ExecuteAsync(Any<CassandraDriver.IStatement>()).Returns(rowSet);

        // Act
        var result = await CassandraHealthCheck.DefaultCommandAsync(cluster, CancellationToken.None);

        // Assert
        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task CheckHealthAsync_WhenCommandReturnsFalse_ShouldReturnUnhealthyWithMessage()
    {
        // Arrange
        var cluster = CassandraDriver.ICluster.Mock();
        var options = new CassandraOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (_, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return false;
            },
        };

        var optionsMonitor = IOptionsMonitor<CassandraOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<CassandraDriver.ICluster>(cluster);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new CassandraHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert
                .That(result.Description)
                .IsEqualTo($"{TestName}: The Cassandra command did not return a valid result.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenCommandReturnsTrue_ShouldReturnHealthy()
    {
        // Arrange
        var cluster = CassandraDriver.ICluster.Mock();
        var options = new CassandraOptions
        {
            KeyedService = null,
            Timeout = 1000,
            CommandAsync = async (_, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return true;
            },
        };

        var optionsMonitor = IOptionsMonitor<CassandraOptions>.Mock();
        _ = optionsMonitor.Get(TestName).Returns(options);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<CassandraDriver.ICluster>(cluster);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new CassandraHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{TestName}: Healthy");
        }
    }
}
