namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Cosmos;

using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Cosmos;
using Xunit;

[TestGroup($"{nameof(Azure)}.{nameof(Cosmos)}")]
public sealed class ClientCreationTests
{
    [Fact]
    public void GetCosmosClient_WhenModeIsServiceProvider_ReturnsServiceFromServiceProvider()
    {
        // Arrange
        var serviceProvider = new ServiceCollection()
            .AddSingleton(_ => new CosmosClient("https://localhost:8081", "dummyKey"))
            .BuildServiceProvider();

        var options = new CosmosClientAvailableOptions { Mode = CosmosClientCreationMode.ServiceProvider };

        // Act
        var result = ClientCreation.GetCosmosClient("test", options, serviceProvider);

        // Assert
        Assert.NotNull(result);
        _ = Assert.IsType<CosmosClient>(result);
    }

    [Fact]
    public void GetCosmosClient_WhenModeIsConnectionString_ReturnsNewClient()
    {
        // Arrange
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var options = new CosmosClientAvailableOptions
        {
            Mode = CosmosClientCreationMode.ConnectionString,
            ConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=dummyKey;",
        };

        // Act
        var result = ClientCreation.GetCosmosClient("test", options, serviceProvider);

        // Assert
        Assert.NotNull(result);
        _ = Assert.IsType<CosmosClient>(result);
    }

    [Fact]
    public void GetCosmosClient_WhenModeIsDefaultAzureCredentials_ReturnsNewClient()
    {
        // Arrange
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var options = new CosmosClientAvailableOptions
        {
            Mode = CosmosClientCreationMode.DefaultAzureCredentials,
            EndpointUri = new Uri("https://localhost:8081"),
        };

        // Act
        var result = ClientCreation.GetCosmosClient("test", options, serviceProvider);

        // Assert
        Assert.NotNull(result);
        _ = Assert.IsType<CosmosClient>(result);
    }

    [Fact]
    public void GetCosmosClient_WhenModeIsDefault_ReturnsNewClientWithEndpointAndKey()
    {
        // Arrange
        var serviceProvider = new ServiceCollection().AddCosmosDbClient().BuildServiceProvider();
        var options = new CosmosClientAvailableOptions
        {
            EndpointUri = new Uri("https://localhost:8081"),
            PrimaryKey = "dummyKey",
        };

        // Act
        var result = ClientCreation.GetCosmosClient("test", options, serviceProvider);

        // Assert
        Assert.NotNull(result);
        _ = Assert.IsType<CosmosClient>(result);
    }

    [Fact]
    public void CreateCosmosClient_WhenEndpointUriIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var options = new CosmosClientAvailableOptions { PrimaryKey = "dummyKey" };

        // Act
        void Act() => ClientCreation.CreateCosmosClient(options, serviceProvider);

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(Act);
        Assert.Equal("EndpointUri cannot be null.", exception.Message);
    }

    [Fact]
    public void CreateCosmosClient_WithClientOptions_ConfiguresTheClientOptions()
    {
        // Arrange
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var optionsConfigured = false;
        var options = new CosmosClientAvailableOptions
        {
            EndpointUri = new Uri("https://localhost:8081"),
            PrimaryKey = "dummyKey",
            ConfigureClientOptions = opt => optionsConfigured = true,
        };

        // Act
        using var result = ClientCreation.CreateCosmosClient(options, serviceProvider);

        // Assert
        Assert.NotNull(result);
        _ = Assert.IsType<CosmosClient>(result);
        Assert.True(optionsConfigured);
    }

    [Fact]
    public void GetCosmosClient_CalledMultipleTimes_ReturnsSameInstance()
    {
        // Arrange
        var serviceProvider = new ServiceCollection().AddCosmosDbClient().BuildServiceProvider();
        var options = new CosmosClientAvailableOptions
        {
            EndpointUri = new Uri("https://localhost:8081"),
            PrimaryKey = "dummyKey",
        };

        // Act
        var result1 = ClientCreation.GetCosmosClient("test", options, serviceProvider);
        var result2 = ClientCreation.GetCosmosClient("test", options, serviceProvider);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Same(result1, result2);
    }

    [Fact]
    public void GetCosmosClient_WithDifferentNames_ReturnsDifferentInstances()
    {
        // Arrange
        var serviceProvider = new ServiceCollection().AddCosmosDbClient().BuildServiceProvider();
        var options = new CosmosClientAvailableOptions
        {
            EndpointUri = new Uri("https://localhost:8081"),
            PrimaryKey = "dummyKey",
        };

        // Act
        var result1 = ClientCreation.GetCosmosClient("test1", options, serviceProvider);
        var result2 = ClientCreation.GetCosmosClient("test2", options, serviceProvider);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotSame(result1, result2);
    }
}
