namespace NetEvolve.HealthChecks.Tests.Unit.Cassandra;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CassandraDriver = global::Cassandra;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Cassandra;
using NSubstitute;

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
}
