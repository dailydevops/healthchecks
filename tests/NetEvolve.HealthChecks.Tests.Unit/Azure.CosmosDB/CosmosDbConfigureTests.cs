namespace NetEvolve.HealthChecks.Tests.Unit.Azure.CosmosDB;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.CosmosDB;

[TestGroup($"{nameof(Azure)}.{nameof(CosmosDB)}")]
public class CosmosDbConfigureTests
{
    [Test]
    public void Configure_WhenConnectionStringMode_ShouldSucceed()
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

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .BuildServiceProvider();

        var configure = new CosmosDbConfigure(configuration, serviceProvider);
        var options = new CosmosDbOptions();

        // Act
        configure.Configure("Test", options);
        var result = configure.Validate("Test", options);

        // Assert
        Assert.That(result.Succeeded, Is.True);
        Assert.That(
            options.ConnectionString,
            Is.EqualTo("AccountEndpoint=https://test.documents.azure.com:443/;AccountKey=test;")
        );
        Assert.That(options.Mode, Is.EqualTo(CosmosDbClientCreationMode.ConnectionString));
    }

    [Test]
    public void Validate_WhenMissingConnectionString_ShouldFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new CosmosDbConfigure(configuration, serviceProvider);
        var options = new CosmosDbOptions { Mode = CosmosDbClientCreationMode.ConnectionString };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(
            result.Failures,
            Contains.Item("The connection string cannot be null or whitespace when using `ConnectionString` mode.")
        );
    }

    [Test]
    public void Validate_WhenAccountKeyModeWithMissingEndpoint_ShouldFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new CosmosDbConfigure(configuration, serviceProvider);
        var options = new CosmosDbOptions { Mode = CosmosDbClientCreationMode.AccountKey, AccountKey = "test-key" };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(
            result.Failures,
            Contains.Item("The service endpoint cannot be null or whitespace when using `AccountKey` mode.")
        );
    }
}
