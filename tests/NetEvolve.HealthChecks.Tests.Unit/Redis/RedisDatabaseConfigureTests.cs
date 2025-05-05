namespace NetEvolve.HealthChecks.Tests.Unit.Redis;

using System;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Redis;
using Xunit;

[TestGroup(nameof(Redis))]
public sealed class RedisDatabaseConfigureTests
{
    [Fact]
    public void Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new RedisDatabaseOptions();
        var configure = new RedisDatabaseConfigure(new ConfigurationBuilder().Build());
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
        var configure = new RedisDatabaseConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = default(RedisDatabaseOptions);

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
        var configure = new RedisDatabaseConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new RedisDatabaseOptions { ConnectionString = default, Mode = ConnectionHandleMode.Create };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The property ConnectionString cannot be null or whitespace.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenArgumentTimeoutLessThanInfinite_ThrowArgumentException()
    {
        // Arrange
        var configure = new RedisDatabaseConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new RedisDatabaseOptions { ConnectionString = "Test", Timeout = -2 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The property Timeout cannot be less than -1.", result.FailureMessage);
    }

    [Fact]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new RedisDatabaseConfigure(new ConfigurationBuilder().Build());
        var name = default(string);
        var options = new RedisDatabaseOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void Configure_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new RedisDatabaseConfigure(new ConfigurationBuilder().Build());
        var options = new RedisDatabaseOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
