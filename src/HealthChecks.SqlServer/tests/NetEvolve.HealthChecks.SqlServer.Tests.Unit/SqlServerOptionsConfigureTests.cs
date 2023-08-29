namespace NetEvolve.HealthChecks.SqlServer.Tests.Unit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

[ExcludeFromCodeCoverage]
[UnitTest]
public sealed class SqlServerOptionsConfigureTests
{
    [Fact]
    public void Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new SqlServerOptions();
        var configure = new SqlServerOptionsConfigure(new ConfigurationBuilder().Build());
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
        var configure = new SqlServerOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = default(SqlServerOptions);

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
        var configure = new SqlServerOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new SqlServerOptions();

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
        var configure = new SqlServerOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new SqlServerOptions { ConnectionString = "Test", Timeout = -2 };

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
        var configure = new SqlServerOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new SqlServerOptions { ConnectionString = "Test" };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(SqlServerCheck.DefaultCommand, options.Command);
    }

    [Fact]
    public void Validate_WhenArgumentCommandEmpty_SetDefaultCommand()
    {
        // Arrange
        var configure = new SqlServerOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new SqlServerOptions { ConnectionString = "Test", Command = string.Empty };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(SqlServerCheck.DefaultCommand, options.Command);
    }

    [Fact]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new SqlServerOptionsConfigure(new ConfigurationBuilder().Build());
        var name = default(string);
        var options = new SqlServerOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void Configure_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new SqlServerOptionsConfigure(new ConfigurationBuilder().Build());
        var options = new SqlServerOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
