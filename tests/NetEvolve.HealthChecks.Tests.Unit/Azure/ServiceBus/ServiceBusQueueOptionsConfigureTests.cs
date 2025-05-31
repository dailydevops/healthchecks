namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Queue")]
public sealed class ServiceBusQueueOptionsConfigureTests
{
    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new ServiceBusQueueOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public void Configure_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        var name = string.Empty;
        var options = new ServiceBusQueueOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void Configure_WhenArgumentNameWhitespace_ThrowArgumentException()
    {
        // Arrange
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = " ";
        var options = new ServiceBusQueueOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void Configure_WhenDefaultConfigure_ThrowArgumentException()
    {
        // Arrange
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        var options = new ServiceBusQueueOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Validate_WhenNameNull_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new ServiceBusQueueOptions();

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
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(ServiceBusQueueOptions);

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
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusQueueOptions { Timeout = -2 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The timeout cannot be less than infinite (-1).");
        }
    }

    [Test]
    public async Task Validate_WhenModeNull_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusQueueOptions { Timeout = 100, Mode = null };

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
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusQueueOptions
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
                    "The fully qualified namespace cannot be null or whitespace when using DefaultAzureCredentials.",
                    StringComparison.Ordinal
                );
        }
    }

    [Test]
    public async Task Validate_WhenModeConnectionStringAndNoConnectionString_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusQueueOptions
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
                .IsEqualTo(
                    "The connection string cannot be null or whitespace when using ConnectionString.",
                    StringComparison.Ordinal
                );
        }
    }

    [Test]
    public async Task Validate_WhenQueueNameNull_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusQueueOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ServiceProvider,
            QueueName = null,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The queue name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenQueueNameEmpty_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusQueueOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ServiceProvider,
            QueueName = string.Empty,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The queue name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var configure = new ServiceBusQueueOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusQueueOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ServiceProvider,
            QueueName = "TestQueue",
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
