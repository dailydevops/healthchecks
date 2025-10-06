namespace NetEvolve.HealthChecks.Tests.Unit.Azure.CosmosDB;

using System;
using System.Collections.Generic;
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

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .BuildServiceProvider();

        var configure = new CosmosDbConfigure(configuration, serviceProvider);
        var options = new CosmosDbOptions();

        // Act
        configure.Configure("Test", options);
        var result = configure.Validate("Test", options);

        // Assert
        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(options.ConnectionString).IsEqualTo(
            "AccountEndpoint=https://test.documents.azure.com:443/;AccountKey=test;"
        );
        await Assert.That(options.Mode).IsEqualTo(CosmosDbClientCreationMode.ConnectionString);
    }

    [Test]
    public async Task Validate_WhenMissingConnectionString_ShouldFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new CosmosDbConfigure(configuration, serviceProvider);
        var options = new CosmosDbOptions { Mode = CosmosDbClientCreationMode.ConnectionString };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        await Assert.That(result.Succeeded).IsFalse();
        await Assert.That(result.Failures).Contains(
            "The connection string cannot be null or whitespace when using `ConnectionString` mode."
        );
    }

    [Test]
    public async Task Validate_WhenAccountKeyModeWithMissingEndpoint_ShouldFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new CosmosDbConfigure(configuration, serviceProvider);
        var options = new CosmosDbOptions { Mode = CosmosDbClientCreationMode.AccountKey, AccountKey = "test-key" };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        await Assert.That(result.Succeeded).IsFalse();
        await Assert.That(result.Failures).Contains(
            "The service endpoint cannot be null or whitespace when using `AccountKey` mode."
        );
    }

    [Test]
    public async Task Validate_WhenAccountKeyModeWithMissingAccountKey_ShouldFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new CosmosDbConfigure(configuration, serviceProvider);
        var options = new CosmosDbOptions
        {
            Mode = CosmosDbClientCreationMode.AccountKey,
            ServiceEndpoint = "https://test.documents.azure.com:443/",
        };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        await Assert.That(result.Succeeded).IsFalse();
        await Assert.That(result.Failures).Contains(
            "The account key cannot be null or whitespace when using `AccountKey` mode."
        );
    }

    [Test]
    public async Task Configure_WhenAccountKeyMode_ShouldSucceed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new[]
                {
                    new KeyValuePair<string, string?>(
                        "HealthChecks:CosmosDb:Test:ServiceEndpoint",
                        "https://test.documents.azure.com:443/"
                    ),
                    new KeyValuePair<string, string?>("HealthChecks:CosmosDb:Test:AccountKey", "test-key"),
                    new KeyValuePair<string, string?>("HealthChecks:CosmosDb:Test:Mode", "AccountKey"),
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
        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(options.ServiceEndpoint).IsEqualTo("https://test.documents.azure.com:443/");
        await Assert.That(options.AccountKey).IsEqualTo("test-key");
        await Assert.That(options.Mode).IsEqualTo(CosmosDbClientCreationMode.AccountKey);
    }

    [Test]
    public async Task Configure_WhenDefaultAzureCredentialsMode_ShouldSucceed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new[]
                {
                    new KeyValuePair<string, string?>(
                        "HealthChecks:CosmosDb:Test:ServiceEndpoint",
                        "https://test.documents.azure.com:443/"
                    ),
                    new KeyValuePair<string, string?>("HealthChecks:CosmosDb:Test:Mode", "DefaultAzureCredentials"),
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
        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(options.ServiceEndpoint).IsEqualTo("https://test.documents.azure.com:443/");
        await Assert.That(options.Mode).IsEqualTo(CosmosDbClientCreationMode.DefaultAzureCredentials);
    }

    [Test]
    public async Task Validate_WhenDefaultAzureCredentialsModeWithMissingEndpoint_ShouldFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new CosmosDbConfigure(configuration, serviceProvider);
        var options = new CosmosDbOptions { Mode = CosmosDbClientCreationMode.DefaultAzureCredentials };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        await Assert.That(result.Succeeded).IsFalse();
        await Assert.That(result.Failures).Contains(
            "The service endpoint cannot be null or whitespace when using `DefaultAzureCredentials` mode."
        );
    }
}
