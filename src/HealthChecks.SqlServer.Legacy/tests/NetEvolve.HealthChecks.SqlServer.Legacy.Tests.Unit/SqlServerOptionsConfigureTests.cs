namespace NetEvolve.HealthChecks.SqlServer.Legacy.Tests.Unit;

using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.SqlServer.Legacy;
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
        var options = new SqlServerLegacyOptions();
        var configure = new SqlServerLegacyOptionsConfigure(new ConfigurationBuilder().Build());
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
        var configure = new SqlServerLegacyOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = default(SqlServerLegacyOptions);

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
        var configure = new SqlServerLegacyOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new SqlServerLegacyOptions();

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
        var configure = new SqlServerLegacyOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new SqlServerLegacyOptions { ConnectionString = "Test", Timeout = -2 };

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
        var configure = new SqlServerLegacyOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new SqlServerLegacyOptions { ConnectionString = "Test" };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(SqlServerLegacyCheck.DefaultCommand, options.Command);
    }

    [Fact]
    public void PostConfigure_WhenArgumentCommandEmpty_SetDefaultCommand()
    {
        // Arrange
        var configure = new SqlServerLegacyOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new SqlServerLegacyOptions
        {
            ConnectionString = "Test",
            Command = string.Empty
        };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        Assert.Equal(SqlServerLegacyCheck.DefaultCommand, options.Command);
    }

    [Fact]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new SqlServerLegacyOptionsConfigure(new ConfigurationBuilder().Build());
        var name = default(string);
        var options = new SqlServerLegacyOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void Configure_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new SqlServerLegacyOptionsConfigure(new ConfigurationBuilder().Build());
        var options = new SqlServerLegacyOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
