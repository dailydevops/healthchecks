namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Cosmos;

using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Cosmos;
using Xunit;

[TestGroup($"{nameof(Azure)}.{nameof(Cosmos)}")]
public sealed class CosmosClientAvailableConfigureTests
{
    [Fact]
    public void Configure_WithValidName_BindsConfiguration()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string?>(
                        "HealthChecks:AzureCosmos:test:ConnectionString",
                        "testConnectionString"
                    ),
                    new KeyValuePair<string, string?>("HealthChecks:AzureCosmos:test:Timeout", "200"),
                ]
            )
            .Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosClientAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosClientAvailableOptions();

        // Act
        sut.Configure("test", options);

        // Assert
        Assert.Equal("testConnectionString", options.ConnectionString);
        Assert.Equal(200, options.Timeout);
    }

    [Fact]
    public void Configure_WithNullName_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosClientAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosClientAvailableOptions();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => sut.Configure(null, options));
    }

    [Fact]
    public void Validate_WithNullName_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosClientAvailableConfigure(configuration, serviceProvider);

        // Act
        var result = sut.Validate(null, new CosmosClientAvailableOptions());

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("The name cannot be null or whitespace.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WithNullOptions_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosClientAvailableConfigure(configuration, serviceProvider);

        // Act
        var result = sut.Validate("test", null!);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("The option cannot be null.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WithInvalidTimeout_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosClientAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosClientAvailableOptions { Timeout = -2 };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("The timeout cannot be less than infinite (-1).", result.FailureMessage);
    }

    [Fact]
    public void Validate_ServiceProviderModeWithNoRegisteredClient_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosClientAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosClientAvailableOptions { Mode = CosmosClientCreationMode.ServiceProvider };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("No service of type", result.FailureMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_ServiceProviderModeWithRegisteredClient_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        _ = services.AddSingleton(_ => new CosmosClient("https://localhost:8081", "dummyKey"));
        var serviceProvider = services.BuildServiceProvider();
        var sut = new CosmosClientAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosClientAvailableOptions { Mode = CosmosClientCreationMode.ServiceProvider };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_ConnectionStringModeWithNullConnectionString_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosClientAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosClientAvailableOptions { Mode = CosmosClientCreationMode.ConnectionString };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("connection string cannot be null", result.FailureMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_ConnectionStringModeWithValidConnectionString_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosClientAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosClientAvailableOptions
        {
            Mode = CosmosClientCreationMode.ConnectionString,
            ConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=dummyKey;",
        };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_DefaultAzureCredentialsModeWithNullEndpointUri_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosClientAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosClientAvailableOptions { Mode = CosmosClientCreationMode.DefaultAzureCredentials };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("endpoint URI cannot be null", result.FailureMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_DefaultAzureCredentialsModeWithRelativeUri_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosClientAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosClientAvailableOptions
        {
            Mode = CosmosClientCreationMode.DefaultAzureCredentials,
            EndpointUri = new Uri("relative", UriKind.Relative),
        };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains(
            "endpoint URI must be an absolute URI",
            result.FailureMessage,
            StringComparison.OrdinalIgnoreCase
        );
    }

    [Fact]
    public void Validate_DefaultAzureCredentialsModeWithValidUri_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new CosmosClientAvailableConfigure(configuration, serviceProvider);
        var options = new CosmosClientAvailableOptions
        {
            Mode = CosmosClientCreationMode.DefaultAzureCredentials,
            EndpointUri = new Uri("https://localhost:8081"),
        };

        // Act
        var result = sut.Validate("test", options);

        // Assert
        Assert.True(result.Succeeded);
    }
}
