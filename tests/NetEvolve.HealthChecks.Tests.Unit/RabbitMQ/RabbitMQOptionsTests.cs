namespace NetEvolve.HealthChecks.Tests.Unit.RabbitMQ;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.RabbitMQ;
using Xunit;

[TestGroup(nameof(RabbitMQ))]
public sealed class RabbitMQOptionsTests
{
    [Fact]
    public void Constructor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new RabbitMQOptions();

        // Assert
        Assert.Null(options.KeyedService);
        Assert.Equal(100, options.Timeout);
    }

    [Fact]
    public void KeyedService_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new RabbitMQOptions();
        const string testValue = "test-keyed-service";

        // Act
        options.KeyedService = testValue;

        // Assert
        Assert.Equal(testValue, options.KeyedService);
    }

    [Fact]
    public void Timeout_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new RabbitMQOptions();
        const int testValue = 5000;

        // Act
        options.Timeout = testValue;

        // Assert
        Assert.Equal(testValue, options.Timeout);
    }
}

[TestGroup(nameof(RabbitMQ))]
public sealed class RabbitMQConfigureTests
{
    [Fact]
    public void Configure_WithNameAndOptions_BindsConfigurationCorrectly()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["HealthChecks:RabbitMQ:TestName:KeyedService"] = "test-key",
            ["HealthChecks:RabbitMQ:TestName:Timeout"] = "200",
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();

        var options = new RabbitMQOptions();
        var configure = new RabbitMQConfigure(configuration);

        // Act
        configure.Configure("TestName", options);

        // Assert
        Assert.Equal("test-key", options.KeyedService);
        Assert.Equal(200, options.Timeout);
    }

    [Fact]
    public void Configure_WithNullName_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var options = new RabbitMQOptions();
        var configure = new RabbitMQConfigure(configuration);
        const string? name = null;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => configure.Configure(name, options));
    }

    [Fact]
    public void Configure_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var options = new RabbitMQOptions();
        var configure = new RabbitMQConfigure(configuration);
        var name = string.Empty;

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => configure.Configure(name, options));
    }

    [Fact]
    public void Validate_WithValidOptions_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var options = new RabbitMQOptions { Timeout = 100 };
        var configure = new RabbitMQConfigure(configuration);

        // Act
        var result = configure.Validate("TestName", options);

        // Assert
        Assert.Equal(ValidateOptionsResult.Success, result);
    }

    [Fact]
    public void Validate_WithNullName_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var options = new RabbitMQOptions();
        var configure = new RabbitMQConfigure(configuration);

        // Act
        var result = configure.Validate(null, options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("name cannot be null", result.FailureMessage, StringComparison.Ordinal);
    }

    [Fact]
    public void Validate_WithEmptyName_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var options = new RabbitMQOptions();
        var configure = new RabbitMQConfigure(configuration);

        // Act
        var result = configure.Validate("", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("name cannot be null", result.FailureMessage, StringComparison.Ordinal);
    }

    [Fact]
    public void Validate_WithNullOptions_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new RabbitMQConfigure(configuration);

        // Act
        var result = configure.Validate("TestName", null!);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("option cannot be null", result.FailureMessage, StringComparison.Ordinal);
    }

    [Fact]
    public void Validate_WithInvalidTimeout_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var options = new RabbitMQOptions
        {
            Timeout = -2, // -1 is valid (Infinite), but -2 is not
        };
        var configure = new RabbitMQConfigure(configuration);

        // Act
        var result = configure.Validate("TestName", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("timeout cannot be less than infinite", result.FailureMessage, StringComparison.Ordinal);
    }
}
