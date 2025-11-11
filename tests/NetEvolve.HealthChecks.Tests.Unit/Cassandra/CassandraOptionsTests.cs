namespace NetEvolve.HealthChecks.Tests.Unit.Cassandra;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Cassandra;

[TestGroup(nameof(Cassandra))]
public class CassandraOptionsTests
{
    [Test]
    public void CassandraOptions_DefaultConstructor_ExpectDefaultValues()
    {
        // Arrange
        var options = new CassandraOptions();

        // Assert
        Assert.That(options.Timeout, Is.EqualTo(100));
        Assert.That(options.KeyedService, Is.Null);
        Assert.That(options.CommandAsync, Is.Not.Null);
    }
}
