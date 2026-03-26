namespace NetEvolve.HealthChecks.Tests.Unit.Apache.RocketMQ;

using System;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Apache.RocketMQ;

[TestGroup($"{nameof(Apache)}.{nameof(RocketMQ)}")]
public sealed class RocketMQConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ReturnsFail()
    {
        // Arrange
        var options = new RocketMQOptions();
        var configure = new RocketMQConfigure(new ConfigurationBuilder().Build());
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
    public async Task Validate_WhenArgumentOptionsNull_ReturnsFail()
    {
        // Arrange
        var configure = new RocketMQConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = default(RocketMQOptions);

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
    public async Task Validate_WhenEndpointNull_ReturnsFail()
    {
        // Arrange
        var configure = new RocketMQConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new RocketMQOptions { Topic = "test-topic" };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The endpoint cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenTopicNull_ReturnsFail()
    {
        // Arrange
        var configure = new RocketMQConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new RocketMQOptions { Endpoint = "127.0.0.1:8081" };

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
    public async Task Validate_WhenTimeoutLessThanInfinite_ReturnsFail()
    {
        // Arrange
        var configure = new RocketMQConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new RocketMQOptions
        {
            Endpoint = "127.0.0.1:8081",
            Topic = "test-topic",
            Timeout = -2,
        };

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

    [Test]
    public async Task Validate_WhenAllValid_ReturnsSuccess()
    {
        // Arrange
        var configure = new RocketMQConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new RocketMQOptions
        {
            Endpoint = "127.0.0.1:8081",
            Topic = "test-topic",
            Timeout = 100,
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
        var configure = new RocketMQConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new RocketMQOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public void Configure_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configure = new RocketMQConfigure(new ConfigurationBuilder().Build());
        var options = new RocketMQOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Configure_WithNameAndOptions_BindsConfigurationCorrectly()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["HealthChecks:RocketMQ:TestName:Endpoint"] = "127.0.0.1:8081",
            ["HealthChecks:RocketMQ:TestName:Topic"] = "health-check-topic",
            ["HealthChecks:RocketMQ:TestName:AccessKey"] = "my-key",
            ["HealthChecks:RocketMQ:TestName:AccessSecret"] = "my-secret",
            ["HealthChecks:RocketMQ:TestName:EnableSsl"] = "false",
            ["HealthChecks:RocketMQ:TestName:Timeout"] = "200",
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();

        var options = new RocketMQOptions();
        var configure = new RocketMQConfigure(configuration);

        // Act
        configure.Configure("TestName", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.Endpoint).IsEqualTo("127.0.0.1:8081");
            _ = await Assert.That(options.Topic).IsEqualTo("health-check-topic");
            _ = await Assert.That(options.AccessKey).IsEqualTo("my-key");
            _ = await Assert.That(options.AccessSecret).IsEqualTo("my-secret");
            _ = await Assert.That(options.EnableSsl).IsFalse();
            _ = await Assert.That(options.Timeout).IsEqualTo(200);
        }
    }
}
