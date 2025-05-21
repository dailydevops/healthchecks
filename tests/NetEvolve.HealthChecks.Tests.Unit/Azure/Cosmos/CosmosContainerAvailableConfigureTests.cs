namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Cosmos;

using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Cosmos;
using Xunit;

[TestGroup($"{nameof(Azure)}.{nameof(Cosmos)}")]
public sealed class CosmosContainerAvailableConfigureTests
{
    [Fact]
    public void Configure_WithValidName_BindsConfiguration()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string?>(
                        "HealthChecks:AzureCosmosContainer:test:ConnectionString",
                        "testConnectionString"
                    ),
                    new KeyValuePair<string, string?>("HealthChecks:AzureCosmosContainer:test:DatabaseId", "testdb"),
                    new KeyValuePair<string, string?>(
                        "HealthChecks:AzureCosmosContainer:test:ContainerId",
                        "testcontainer"
                    ),
                    new KeyValuePair<string, string?>("HealthChecks:AzureCosmosContainer:test:Timeout", "200"),
                ]
            )
            .Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosContainerAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosContainerAvailableOptions();

        // Act
        sut.Configure("test", options);

        // Assert
        Assert.Equal("testConnectionString", options.ConnectionString);
        Assert.Equal("testdb", options.DatabaseId);
        Assert.Equal("testcontainer", options.ContainerId);
        Assert.Equal(200, options.Timeout);
    }

    [Fact]
    public void Configure_WithNullName_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosContainerAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosContainerAvailableOptions();

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
        var sut = new CosmosContainerAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosContainerAvailableOptions
        {
            Mode = CosmosClientCreationMode.ServiceProvider,
            DatabaseId = null,
            ContainerId = "testcontainer",
        };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("database ID cannot be null", result.FailureMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WithNullContainerId_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        _ = services.AddSingleton(_ => new CosmosClient("https://localhost:8081", "dummyKey"));
        var serviceProvider = services.BuildServiceProvider();
        var sut = new CosmosContainerAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosContainerAvailableOptions
        {
            Mode = CosmosClientCreationMode.ServiceProvider,
            DatabaseId = "testdb",
            ContainerId = null,
        };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("container ID cannot be null", result.FailureMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WithEmptyContainerId_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        _ = services.AddSingleton(_ => new CosmosClient("https://localhost:8081", "dummyKey"));
        var serviceProvider = services.BuildServiceProvider();
        var sut = new CosmosContainerAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosContainerAvailableOptions
        {
            Mode = CosmosClientCreationMode.ServiceProvider,
            DatabaseId = "testdb",
            ContainerId = string.Empty,
        };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("container ID cannot be null", result.FailureMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WithValidContainerId_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        _ = services.AddSingleton(_ => new CosmosClient("https://localhost:8081", "dummyKey"));
        var serviceProvider = services.BuildServiceProvider();
        var sut = new CosmosContainerAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosContainerAvailableOptions
        {
            Mode = CosmosClientCreationMode.ServiceProvider,
            DatabaseId = "testdb",
            ContainerId = "testcontainer",
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
        var sut = new CosmosContainerAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosContainerAvailableOptions
        {
            Mode = CosmosClientCreationMode.ConnectionString,
            ConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=dummyKey;",
            DatabaseId = "testdb",
            ContainerId = "testcontainer",
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
        var sut = new CosmosContainerAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosContainerAvailableOptions
        {
            Mode = CosmosClientCreationMode.DefaultAzureCredentials,
            EndpointUri = new Uri("https://localhost:8081"),
            DatabaseId = "testdb",
            ContainerId = "testcontainer",
        };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.True(result.Succeeded);
    }
}
