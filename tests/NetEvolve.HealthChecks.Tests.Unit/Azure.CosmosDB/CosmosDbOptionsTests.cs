namespace NetEvolve.HealthChecks.Tests.Unit.Azure.CosmosDB;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.CosmosDB;

[TestGroup($"{nameof(Azure)}.{nameof(CosmosDB)}")]
public class CosmosDbOptionsTests
{
    [Test]
    public void Constructor_WhenParametersDefault_ExpectedValues()
    {
        // Arrange / Act
        var options = new CosmosDbOptions();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(options.ConnectionString, Is.Null);
            Assert.That(options.ServiceEndpoint, Is.Null);
            Assert.That(options.AccountKey, Is.Null);
            Assert.That(options.Mode, Is.Null);
            Assert.That(options.DatabaseName, Is.Null);
            Assert.That(options.ContainerName, Is.Null);
            Assert.That(options.Timeout, Is.EqualTo(100));
            Assert.That(options.ConfigureClientOptions, Is.Null);
        });
    }

    [Test]
    public void Properties_WhenSet_ExpectedValues()
    {
        // Arrange
        var expectedConnectionString = "AccountEndpoint=https://test.documents.azure.com:443/;AccountKey=test;";
        var expectedServiceEndpoint = "https://test.documents.azure.com:443/";
        var expectedAccountKey = "test-key";
        var expectedMode = CosmosDbClientCreationMode.AccountKey;
        var expectedDatabaseName = "TestDatabase";
        var expectedContainerName = "TestContainer";
        var expectedTimeout = 500;

        // Act
        var options = new CosmosDbOptions
        {
            ConnectionString = expectedConnectionString,
            ServiceEndpoint = expectedServiceEndpoint,
            AccountKey = expectedAccountKey,
            Mode = expectedMode,
            DatabaseName = expectedDatabaseName,
            ContainerName = expectedContainerName,
            Timeout = expectedTimeout,
            ConfigureClientOptions = _ => { }
        };

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(options.ConnectionString, Is.EqualTo(expectedConnectionString));
            Assert.That(options.ServiceEndpoint, Is.EqualTo(expectedServiceEndpoint));
            Assert.That(options.AccountKey, Is.EqualTo(expectedAccountKey));
            Assert.That(options.Mode, Is.EqualTo(expectedMode));
            Assert.That(options.DatabaseName, Is.EqualTo(expectedDatabaseName));
            Assert.That(options.ContainerName, Is.EqualTo(expectedContainerName));
            Assert.That(options.Timeout, Is.EqualTo(expectedTimeout));
            Assert.That(options.ConfigureClientOptions, Is.Not.Null);
        });
    }
}