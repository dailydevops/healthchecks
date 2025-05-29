namespace NetEvolve.HealthChecks.Tests.Unit.DB2;

using System;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.DB2;
using Xunit;

[TestGroup(nameof(DB2))]
public sealed class DB2ConfigureTests
{
    [Fact]
    public void Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new DB2Options();
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = default;

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
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(DB2Options);

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
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new DB2Options();

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The connection string cannot be null or whitespace.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenArgumentTimeoutLessThanInfinite_ThrowArgumentException()
    {
        // Arrange
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new DB2Options { ConnectionString = "Test", Timeout = -2 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The timeout cannot be less than infinite (-1).", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenArgumentCommandNull_SetDefaultCommand()
    {
        // Arrange
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new DB2Options { ConnectionString = "Test" };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(DB2Check.DefaultCommand, options.Command);
    }

    [Fact]
    public void PostConfigure_WhenArgumentCommandEmpty_SetDefaultCommand()
    {
        // Arrange
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new DB2Options { ConnectionString = "Test", Command = string.Empty };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        Assert.Equal(DB2Check.DefaultCommand, options.Command);
    }

    [Fact]
    public void PostConfigure_WhenNameIsNull_DoNothing()
    {
        // Arrange
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new DB2Options { ConnectionString = "Test", Command = string.Empty };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        Assert.Equal("Test", options.ConnectionString);
        Assert.Equal(string.Empty, options.Command);
    }

    [Fact]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new DB2Options();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void Configure_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        var options = new DB2Options();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
