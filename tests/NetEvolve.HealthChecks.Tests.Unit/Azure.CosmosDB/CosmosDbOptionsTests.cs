namespace NetEvolve.HealthChecks.Tests.Unit.Azure.CosmosDB;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.CosmosDB;

[TestGroup($"{nameof(Azure)}.{nameof(CosmosDB)}")]
public class CosmosDbOptionsTests
{
    [Test]
    public async Task Constructor_WhenParametersDefault_ExpectedValues()
    {
        // Arrange / Act
        var options = new CosmosDbOptions();

        // Assert
        await Assert.That(options.ConnectionString).IsNull();
        await Assert.That(options.ServiceEndpoint).IsNull();
        await Assert.That(options.AccountKey).IsNull();
        await Assert.That(options.Mode).IsNull();
        await Assert.That(options.DatabaseName).IsNull();
        await Assert.That(options.ContainerName).IsNull();
        await Assert.That(options.Timeout).IsEqualTo(100);
        await Assert.That(options.ConfigureClientOptions).IsNull();
    }

    [Test]
    public async Task Properties_WhenSet_ExpectedValues()
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
            ConfigureClientOptions = _ => { },
        };

        // Assert
        await Assert.That(options.ConnectionString).IsEqualTo(expectedConnectionString);
        await Assert.That(options.ServiceEndpoint).IsEqualTo(expectedServiceEndpoint);
        await Assert.That(options.AccountKey).IsEqualTo(expectedAccountKey);
        await Assert.That(options.Mode).IsEqualTo(expectedMode);
        await Assert.That(options.DatabaseName).IsEqualTo(expectedDatabaseName);
        await Assert.That(options.ContainerName).IsEqualTo(expectedContainerName);
        await Assert.That(options.Timeout).IsEqualTo(expectedTimeout);
        await Assert.That(options.ConfigureClientOptions).IsNotNull();
    }

    [Test]
    public async Task ConnectionString_WhenSetToNull_ExpectedValue()
    {
        // Arrange
        var options = new CosmosDbOptions { ConnectionString = "test" };

        // Act
        options.ConnectionString = null;

        // Assert
        await Assert.That(options.ConnectionString).IsNull();
    }

    [Test]
    public async Task Mode_WhenSetToDefaultCredentials_ExpectedValue()
    {
        // Arrange / Act
        var options = new CosmosDbOptions { Mode = CosmosDbClientCreationMode.DefaultAzureCredentials };

        // Assert
        await Assert.That(options.Mode).IsEqualTo(CosmosDbClientCreationMode.DefaultAzureCredentials);
    }

    [Test]
    public async Task Mode_WhenSetToServicePrincipal_ExpectedValue()
    {
        // Arrange / Act
        var options = new CosmosDbOptions { Mode = CosmosDbClientCreationMode.ServicePrincipal };

        // Assert
        await Assert.That(options.Mode).IsEqualTo(CosmosDbClientCreationMode.ServicePrincipal);
    }

    [Test]
    public async Task Timeout_WhenSetToZero_ExpectedValue()
    {
        // Arrange / Act
        var options = new CosmosDbOptions { Timeout = 0 };

        // Assert
        await Assert.That(options.Timeout).IsEqualTo(0);
    }

    [Test]
    public async Task Timeout_WhenSetToNegative_ExpectedValue()
    {
        // Arrange / Act
        var options = new CosmosDbOptions { Timeout = -1 };

        // Assert
        await Assert.That(options.Timeout).IsEqualTo(-1);
    }
}
