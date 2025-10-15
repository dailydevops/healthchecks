namespace NetEvolve.HealthChecks.Tests.Unit.Azure.CosmosDB;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.CosmosDB;

[TestGroup($"{nameof(Azure)}.{nameof(CosmosDB)}")]
public class CosmosDbConfigureTests
{
    [Test]
    public async Task Configure_WhenConnectionStringMode_ShouldSucceed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new[]
                {
                    new KeyValuePair<string, string?>(
                        "HealthChecks:CosmosDb:Test:ConnectionString",
                        "AccountEndpoint=https://test.documents.azure.com:443/;AccountKey=test;"
                    ),
                    new KeyValuePair<string, string?>("HealthChecks:CosmosDb:Test:Mode", "ConnectionString"),
                }
            )
            .Build();

        var configure = new CosmosDbConfigure(configuration);
        var options = new CosmosDbOptions();

        // Act
        configure.Configure("Test", options);
        var result = configure.Validate("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert
                .That(options.ConnectionString)
                .IsEqualTo("AccountEndpoint=https://test.documents.azure.com:443/;AccountKey=test;");
            _ = await Assert.That(options.Mode).IsEqualTo(CosmosDbClientCreationMode.ConnectionString);
        }
    }

    [Test]
    public async Task Validate_WhenMissingConnectionString_ShouldFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new CosmosDbConfigure(configuration);
        var options = new CosmosDbOptions { Mode = CosmosDbClientCreationMode.ConnectionString };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert
                .That(result.Failures)
                .Contains("The connection string cannot be null or whitespace when using `ConnectionString` mode.");
        }
    }

    [Test]
    public async Task Validate_WhenAccountKeyModeWithMissingEndpoint_ShouldFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new CosmosDbConfigure(configuration);
        var options = new CosmosDbOptions { Mode = CosmosDbClientCreationMode.AccountKey, AccountKey = "test-key" };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert
                .That(result.Failures)
                .Contains("The service endpoint cannot be null or whitespace when using `AccountKey` mode.");
        }
    }
}
