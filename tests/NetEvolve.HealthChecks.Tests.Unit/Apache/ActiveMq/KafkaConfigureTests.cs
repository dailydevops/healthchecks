namespace NetEvolve.HealthChecks.Tests.Unit.Apache.ActiveMq;

using System;
using Microsoft.Extensions.Configuration;
using NetEvolve.HealthChecks.Apache.ActiveMq;
using Xunit;

[TestGroup($"{nameof(Apache)}.{nameof(ActiveMq)}")]
public sealed class ActiveMqConfigureTests
{
    [Fact]
    public void Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new ActiveMqOptions();
        var configure = new ActiveMqConfigure(new ConfigurationBuilder().Build());
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
        var configure = new ActiveMqConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = default(ActiveMqOptions);

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
        var configure = new ActiveMqConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new ActiveMqOptions();

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The broker address cannot be null or whitespace.", result.FailureMessage);
    }

    [Fact]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new ActiveMqConfigure(new ConfigurationBuilder().Build());
        var name = default(string);
        var options = new ActiveMqOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void Configure_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new ActiveMqConfigure(new ConfigurationBuilder().Build());
        var options = new ActiveMqOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
