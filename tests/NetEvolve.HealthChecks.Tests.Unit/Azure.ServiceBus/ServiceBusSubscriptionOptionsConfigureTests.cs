namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Subscription")]
public sealed class ServiceBusSubscriptionOptionsConfigureTests
{
    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new ServiceBusSubscriptionOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public void Configure_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        var name = string.Empty;
        var options = new ServiceBusSubscriptionOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void Configure_WhenArgumentNameWhitespace_ThrowArgumentException()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = " ";
        var options = new ServiceBusSubscriptionOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void Configure_WhenDefaultConfigure_ThrowArgumentException()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        var options = new ServiceBusSubscriptionOptions();

        // Act

        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Validate_WhenNameNull_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new ServiceBusSubscriptionOptions();

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
    public async Task Validate_WhenOptionsNull_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(ServiceBusSubscriptionOptions);

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
    public async Task Validate_WhenTimeoutLessThanInfinite_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusSubscriptionOptions { Timeout = -2 };

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
    public async Task Validate_WhenModeNull_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusSubscriptionOptions { Timeout = 100, Mode = null };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The client creation mode cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WhenModeDefaultAzureCredentialsAndNoNamespace_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusSubscriptionOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = null,
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
                    "The fully qualified namespace cannot be null or whitespace when using DefaultAzureCredentials."
                );
        }
    }

    [Test]
    public async Task Validate_WhenModeConnectionStringAndNoConnectionString_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusSubscriptionOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = null,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The connection string cannot be null or whitespace when using ConnectionString.");
        }
    }

    [Test]
    public async Task Validate_WhenTopicNameNull_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusSubscriptionOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = null,
            SubscriptionName = "TestSubscription",
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The topic name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenTopicNameEmpty_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusSubscriptionOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = string.Empty,
            SubscriptionName = "TestSubscription",
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The topic name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenSubscriptionNameNull_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusSubscriptionOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = "TestTopic",
            SubscriptionName = null,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The subscription name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenSubscriptionNameEmpty_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusSubscriptionOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = "TestTopic",
            SubscriptionName = string.Empty,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The subscription name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var configure = new ServiceBusSubscriptionOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusSubscriptionOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = "TestTopic",
            SubscriptionName = "TestSubscription",
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsNull();
        }
    }
}
