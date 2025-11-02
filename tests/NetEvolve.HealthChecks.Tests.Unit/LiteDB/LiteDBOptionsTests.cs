namespace NetEvolve.HealthChecks.Tests.Unit.LiteDB;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.LiteDB;

[TestGroup(nameof(LiteDB))]
public sealed class LiteDBOptionsTests
{
    [Test]
    public async Task LiteDBOptions_PropertiesInitializedCorrectly()
    {
        // Arrange & Act
        var options = new LiteDBOptions();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.ConnectionString).IsNull();
            _ = await Assert.That(options.CollectionName).IsNull();
            _ = await Assert.That(options.Timeout).IsEqualTo(100);
        }
    }

    [Test]
    public async Task LiteDBOptions_GetHashCodeImplementedCorrectly()
    {
        // Arrange
        var options1 = new LiteDBOptions
        {
            ConnectionString = "filename=test.db",
            CollectionName = "TestCollection",
            Timeout = 1000,
        };

        var options2 = new LiteDBOptions
        {
            ConnectionString = "filename=test.db",
            CollectionName = "TestCollection",
            Timeout = 1000,
        };

        var options3 = new LiteDBOptions
        {
            ConnectionString = "filename=different.db",
            CollectionName = "TestCollection",
            Timeout = 1000,
        };

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options1).IsEqualTo(options2);
            _ = await Assert.That(options1).IsNotEqualTo(options3);
        }
    }
}
