namespace NetEvolve.HealthChecks.Tests.Unit.Cassandra;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Cassandra;
using NSubstitute;
using CassandraDriver = global::Cassandra;

[TestGroup(nameof(Cassandra))]
public class CassandraHealthCheckTests
{
    [Test]
    public async Task DefaultCommandAsync_WhenClusterAvailable_ReturnsTrue()
    {
        // Arrange
        var cluster = Substitute.For<CassandraDriver.ICluster>();
        var session = Substitute.For<CassandraDriver.ISession>();
        var rowSet = Substitute.For<CassandraDriver.RowSet>();

        _ = cluster.ConnectAsync().Returns(Task.FromResult(session));
        _ = session.ExecuteAsync(Arg.Any<CassandraDriver.IStatement>()).Returns(Task.FromResult(rowSet));
        _ = rowSet.GetEnumerator().Returns(Enumerable.Repeat(Substitute.For<CassandraDriver.Row>(), 1).GetEnumerator());

        // Act
        var result = await CassandraHealthCheck.DefaultCommandAsync(cluster, CancellationToken.None);

        // Assert
        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task DefaultCommandAsync_WhenResultIsNull_ReturnsFalse()
    {
        // Arrange
        var cluster = Substitute.For<CassandraDriver.ICluster>();
        var session = Substitute.For<CassandraDriver.ISession>();

        _ = cluster.ConnectAsync().Returns(Task.FromResult(session));
        _ = session
            .ExecuteAsync(Arg.Any<CassandraDriver.IStatement>())
            .Returns(Task.FromResult<CassandraDriver.RowSet>(null!));

        // Act
        var result = await CassandraHealthCheck.DefaultCommandAsync(cluster, CancellationToken.None);

        // Assert
        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task DefaultCommandAsync_WhenResultIsEmpty_ReturnsFalse()
    {
        // Arrange
        var cluster = Substitute.For<CassandraDriver.ICluster>();
        var session = Substitute.For<CassandraDriver.ISession>();
        var rowSet = Substitute.For<CassandraDriver.RowSet>();

        _ = cluster.ConnectAsync().Returns(Task.FromResult(session));
        _ = session.ExecuteAsync(Arg.Any<CassandraDriver.IStatement>()).Returns(Task.FromResult(rowSet));
        _ = rowSet.GetEnumerator().Returns(Enumerable.Empty<CassandraDriver.Row>().GetEnumerator());

        // Act
        var result = await CassandraHealthCheck.DefaultCommandAsync(cluster, CancellationToken.None);

        // Assert
        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task CheckHealthAsync_WhenCommandReturnsFalse_ShouldReturnUnhealthyWithMessage()
    {
        // Arrange
        var cluster = Substitute.For<CassandraDriver.ICluster>();
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

        var optionsMonitor = Substitute.For<IOptionsMonitor<CassandraOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(cluster);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new CassandraHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert
                .That(result.Description)
                .IsEqualTo("test: The Cassandra command did not return a valid result.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenCommandReturnsTrue_ShouldReturnHealthy()
    {
        // Arrange
        var cluster = Substitute.For<CassandraDriver.ICluster>();
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

        var optionsMonitor = Substitute.For<IOptionsMonitor<CassandraOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(cluster);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new CassandraHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo("test: Healthy");
        }
    }
}
