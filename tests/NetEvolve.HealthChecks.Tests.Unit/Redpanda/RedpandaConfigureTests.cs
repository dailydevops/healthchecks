namespace NetEvolve.HealthChecks.Tests.Unit.Redpanda;

using System;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Redpanda;
using Xunit;

[TestGroup(nameof(Redpanda))]
public sealed class RedpandaConfigureTests
{
    [Fact]
    public void Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new RedpandaOptions();
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        var name = default(string);

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The name cannot be null or whitespace.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = default(RedpandaOptions);

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The option cannot be null.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenArgumentConnectionStringNull_ThrowArgumentException()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new RedpandaOptions();

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The topic cannot be null or whitespace.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenModeCreate_ThrowArgumentException()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new RedpandaOptions { Topic = "Test", Mode = ProducerHandleMode.Create };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The configuration cannot be null.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenModeCreateAndBootstrapServerEmpty_ThrowArgumentException()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new RedpandaOptions
        {
            Topic = "Test",
            Mode = ProducerHandleMode.Create,
            Configuration = new ProducerConfig(),
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The property BootstrapServers cannot be null or whitespace.", result.FailureMessage);
    }

    [Fact]
    public void Validate_EverythingCorrect_Expected()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new RedpandaOptions
        {
            Topic = "Test",
            Mode = ProducerHandleMode.Create,
            Configuration = new ProducerConfig { BootstrapServers = "localhost:9092" },
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        var name = default(string);
        var options = new RedpandaOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void Configure_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        var options = new RedpandaOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
