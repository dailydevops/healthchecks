namespace NetEvolve.HealthChecks.Tests.Unit.Dapr;

using System;
using Microsoft.Extensions.Configuration;
using NetEvolve.HealthChecks.Dapr;
using Xunit;

public sealed class DaprConfigureTests
{
    [Fact]
    public void Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new DaprOptions();
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
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
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = default(DaprOptions);

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The option cannot be null.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenArgumentTimeoutLessThanInfinite_ThrowArgumentException()
    {
        // Arrange
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new DaprOptions { Timeout = -2 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The timeout cannot be less than infinite (-1).", result.FailureMessage);
    }

    [Fact]
    public void Validate_EverythingFine_Expected()
    {
        // Arrange
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new DaprOptions { Timeout = 100 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
        var name = default(string);
        var options = new DaprOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void Configure_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
        var options = new DaprOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
