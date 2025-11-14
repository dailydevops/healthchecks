namespace NetEvolve.HealthChecks.Tests.Unit.Cassandra;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Cassandra;

[TestGroup(nameof(Cassandra))]
public class CassandraOptionsTests
{
    [Test]
    public async Task CassandraOptions_DefaultConstructor_ExpectDefaultValues()
    {
        // Arrange
        var options = new CassandraOptions();

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(100);
        _ = await Assert.That(options.KeyedService).IsNull();
        _ = await Assert.That(options.CommandAsync).IsNotNull();
    }
}
