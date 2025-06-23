namespace NetEvolve.HealthChecks.Tests.Unit.Redpanda;

using System;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Redpanda;

[TestGroup(nameof(Redpanda))]
public sealed class RedpandaConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new RedpandaOptions();
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        const string? name = default;

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(RedpandaOptions);

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The option cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WhenArgumentConnectionStringNull_ThrowArgumentException()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new RedpandaOptions();

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The topic cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenModeCreate_ThrowArgumentException()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new RedpandaOptions { Topic = "Test", Mode = ProducerHandleMode.Create };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The configuration cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WhenModeCreateAndBootstrapServerEmpty_ThrowArgumentException()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new RedpandaOptions
        {
            Topic = "Test",
            Mode = ProducerHandleMode.Create,
            Configuration = new ProducerConfig(),
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The property BootstrapServers cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_EverythingCorrect_Expected()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new RedpandaOptions
        {
            Topic = "Test",
            Mode = ProducerHandleMode.Create,
            Configuration = new ProducerConfig { BootstrapServers = "localhost:9092" },
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new RedpandaOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
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

    [Test]
    public async Task Validate_WhenArgumentTimeoutLessThanInfinite_ThrowArgumentException()
    {
        // Arrange
        var configure = new RedpandaConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new RedpandaOptions { Timeout = -2 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout."
                );
        }
    }
}
