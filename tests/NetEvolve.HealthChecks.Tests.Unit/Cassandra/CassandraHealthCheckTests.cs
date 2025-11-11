namespace NetEvolve.HealthChecks.Tests.Unit.Cassandra;

using System;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
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
        var cluster = Substitute.For<ICluster>();
        var session = Substitute.For<ISession>();
        var rowSet = Substitute.For<RowSet>();

        _ = cluster.ConnectAsync().Returns(Task.FromResult(session));
        _ = session.ExecuteAsync(Arg.Any<IStatement>()).Returns(Task.FromResult(rowSet));
        _ = rowSet.GetEnumerator().Returns(new System.Collections.Generic.List<Row> { new Row() }.GetEnumerator());

        // Act
        var result = await CassandraHealthCheck.DefaultCommandAsync(cluster, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
    }
}
