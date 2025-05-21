namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Cosmos;

using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Cosmos;
using Xunit;

[TestGroup($"{nameof(Azure)}.{nameof(Cosmos)}")]
public sealed class DependencyInjectionExtensionsTests
{
    [Fact]
    public void AddCosmosClientAvailability_WhenBuilderIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(
            "builder",
            () => DependencyInjectionExtensions.AddCosmosClientAvailability(null!, "test")
        );
    }

    [Fact]
    public void AddCosmosClientAvailability_WhenNameIsNull_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>("name", () => builder.AddCosmosClientAvailability(null!));
    }

    [Fact]
    public void AddCosmosClientAvailability_WhenNameIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>("name", () => builder.AddCosmosClientAvailability(""));
    }

    [Fact]
    public void AddCosmosClientAvailability_WhenTagsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(
            "tags",
            () => builder.AddCosmosClientAvailability("test", null!, null!)
        );
    }

    [Fact]
    public void AddCosmosClientAvailability_WhenNameAlreadyUsed_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection().AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        var builder = services.AddHealthChecks();
        _ = builder.AddCosmosClientAvailability("test");

        // Act & Assert
        _ = Assert.Throws<ArgumentException>("name", () => builder.AddCosmosClientAvailability("test"));
    }

    [Fact]
    public void AddCosmosClientAvailability_WhenValid_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        _ = builder.AddCosmosClientAvailability("test");

        // Assert
        var serviceDescriptor = services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(CosmosClientAvailableHealthCheck)
        );
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
    }

    [Fact]
    public void AddCosmosClientAvailability_WithTags_IncludesTags()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        _ = builder.AddCosmosClientAvailability("test", null, "custom-tag");

        // Assert
        var serviceDescriptor = services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(CosmosClientAvailableHealthCheck)
        );
        Assert.NotNull(serviceDescriptor);
    }

    [Fact]
    public void AddCosmosDatabaseAvailability_WhenBuilderIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(
            "builder",
            () => DependencyInjectionExtensions.AddCosmosDatabaseAvailability(null!, "test")
        );
    }

    [Fact]
    public void AddCosmosDatabaseAvailability_WhenNameIsNull_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>("name", () => builder.AddCosmosDatabaseAvailability(null!));
    }

    [Fact]
    public void AddCosmosDatabaseAvailability_WhenValid_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        _ = builder.AddCosmosDatabaseAvailability("test");

        // Assert
        var serviceDescriptor = services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(CosmosDatabaseAvailableHealthCheck)
        );
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
    }

    [Fact]
    public void AddCosmosContainerAvailability_WhenBuilderIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(
            "builder",
            () => DependencyInjectionExtensions.AddCosmosContainerAvailability(null!, "test")
        );
    }

    [Fact]
    public void AddCosmosContainerAvailability_WhenNameIsNull_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>("name", () => builder.AddCosmosContainerAvailability(null!));
    }

    [Fact]
    public void AddCosmosContainerAvailability_WhenValid_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        _ = builder.AddCosmosContainerAvailability("test");

        // Assert
        var serviceDescriptor = services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(CosmosContainerAvailableHealthCheck)
        );
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
    }
}
