namespace NetEvolve.HealthChecks.Tests.Unit.CouchDb;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.CouchDb;

[TestGroup(nameof(CouchDb))]
public class CouchDbOptionsTests
{
    [Test]
    public async Task CouchDbOptions_DefaultValues_ShouldBeExpected()
    {
        // Arrange & Act
        var options = new CouchDbOptions();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.ConnectionString).IsNull();
            _ = await Assert.That(options.Timeout).IsEqualTo(100);
        }
    }
}
