namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Cosmos;

using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Cosmos;
using Xunit;

[TestGroup($"{nameof(Azure)}.{nameof(Cosmos)}")]
public sealed class CosmosDatabaseAvailableConfigureTests
{
    [Fact]
    public void Configure_WithValidName_BindsConfiguration()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string?>(
                        "HealthChecks:AzureCosmosDatabase:test:ConnectionString",
                        "testConnectionString"
                    ),
                    new KeyValuePair<string, string?>("HealthChecks:AzureCosmosDatabase:test:DatabaseId", "testdb"),
                    new KeyValuePair<string, string?>("HealthChecks:AzureCosmosDatabase:test:Timeout", "200"),
                ]
            )
            .Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosDatabaseAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosDatabaseAvailableOptions();

        // Act
        sut.Configure("test", options);

        // Assert
        Assert.Equal("testConnectionString", options.ConnectionString);
        Assert.Equal("testdb", options.DatabaseId);
        Assert.Equal(200, options.Timeout);
    }

    [Fact]
    public void Configure_WithNullName_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosDatabaseAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosDatabaseAvailableOptions();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => sut.Configure(null, options));
    }

    [Fact]
    public void Validate_WithNullDatabaseId_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        _ = services.AddSingleton(_ => new CosmosClient("https://localhost:8081", "dummyKey"));
        var serviceProvider = services.BuildServiceProvider();
        var sut = new CosmosDatabaseAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosDatabaseAvailableOptions
        {
            Mode = CosmosClientCreationMode.ServiceProvider,
            DatabaseId = null,
        };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("database ID cannot be null", result.FailureMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WithEmptyDatabaseId_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        _ = services.AddSingleton(_ => new CosmosClient("https://localhost:8081", "dummyKey"));
        var serviceProvider = services.BuildServiceProvider();
        var sut = new CosmosDatabaseAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosDatabaseAvailableOptions
        {
            Mode = CosmosClientCreationMode.ServiceProvider,
            DatabaseId = string.Empty,
        };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("database ID cannot be null", result.FailureMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WithValidDatabaseId_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        _ = services.AddSingleton(_ => new CosmosClient("https://localhost:8081", "dummyKey"));
        var serviceProvider = services.BuildServiceProvider();
        var sut = new CosmosDatabaseAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosDatabaseAvailableOptions
        {
            Mode = CosmosClientCreationMode.ServiceProvider,
            DatabaseId = "testDb",
        };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_ConnectionStringModeWithValidOptions_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosDatabaseAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosDatabaseAvailableOptions
        {
            Mode = CosmosClientCreationMode.ConnectionString,
            ConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=dummyKey;",
            DatabaseId = "testDb",
        };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_DefaultAzureCredentialsModeWithValidOptions_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosDatabaseAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosDatabaseAvailableOptions
        {
            Mode = CosmosClientCreationMode.DefaultAzureCredentials,
            EndpointUri = new Uri("https://localhost:8081"),
            DatabaseId = "testDb",
        };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.True(result.Succeeded);
    }
}
