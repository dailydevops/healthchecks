namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;
using Xunit;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public sealed class ServiceBusTopicOptionsConfigureTests
{
    [Fact]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new ServiceBusTopicOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void Configure_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        var name = string.Empty;
        var options = new ServiceBusTopicOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Fact]
    public void Configure_WhenArgumentNameWhitespace_ThrowArgumentException()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = " ";
        var options = new ServiceBusTopicOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Fact]
    public void Configure_WhenDefaultConfigure_ThrowArgumentException()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        var options = new ServiceBusTopicOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Fact]
    public void Validate_WhenNameNull_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new ServiceBusTopicOptions();

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The name cannot be null or whitespace.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenOptionsNull_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(ServiceBusTopicOptions);

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The option cannot be null.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenTimeoutLessThanInfinite_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusTopicOptions { Timeout = -2 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The timeout cannot be less than infinite (-1).", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenModeNull_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusTopicOptions { Timeout = 100, Mode = null };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The client creation mode cannot be null.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenModeDefaultAzureCredentialsAndNoNamespace_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusTopicOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = null,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal(
            "The fully qualified namespace cannot be null or whitespace when using DefaultAzureCredentials.",
            result.FailureMessage
        );
    }

    [Fact]
    public void Validate_WhenModeConnectionStringAndNoConnectionString_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusTopicOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = null,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal(
            "The connection string cannot be null or whitespace when using ConnectionString.",
            result.FailureMessage
        );
    }

    [Fact]
    public void Validate_WhenTopicNameNull_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusTopicOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = null,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The topic name cannot be null or whitespace.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenTopicNameEmpty_ReturnsFail()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusTopicOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = string.Empty,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The topic name cannot be null or whitespace.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var configure = new ServiceBusTopicOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new ServiceBusTopicOptions
        {
            Timeout = 100,
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = "TestTopic",
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Null(result.FailureMessage);
    }
}
