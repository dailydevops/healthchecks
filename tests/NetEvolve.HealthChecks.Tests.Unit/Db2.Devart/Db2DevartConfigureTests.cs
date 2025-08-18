namespace NetEvolve.HealthChecks.Tests.Unit.Db2.Devart;

using System;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Db2.Devart;

[TestGroup($"{nameof(Db2)}.{nameof(Devart)}")]
public sealed class Db2DevartConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new Db2DevartOptions();
        var configure = new Db2DevartConfigure(new ConfigurationBuilder().Build());
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
    public async Task Validate_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new Db2DevartConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(Db2DevartOptions);

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
    public async Task Validate_WhenArgumentConnectionStringNull_ThrowArgumentException()
    {
        // Arrange
        var configure = new Db2DevartConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new Db2DevartOptions { ConnectionString = default! };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The connection string cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutLessThanInfinite_ThrowArgumentException()
    {
        // Arrange
        var configure = new Db2DevartConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new Db2DevartOptions { ConnectionString = "Test", Timeout = -2 };

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
    public async Task Validate_WhenArgumentCommandNull_SetDefaultCommand()
    {
        // Arrange
        var configure = new Db2DevartConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new Db2DevartOptions { ConnectionString = "Test" };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(options.Command).IsEqualTo(Db2DevartHealthCheck.DefaultCommand);
        }
    }

    [Test]
    public async Task PostConfigure_WhenArgumentCommandEmpty_SetDefaultCommand()
    {
        // Arrange
        var configure = new Db2DevartConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new Db2DevartOptions { ConnectionString = "Test", Command = string.Empty };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(Db2DevartHealthCheck.DefaultCommand);
    }

    [Test]
    public async Task PostConfigure_WhenNameIsNull_DoNothing()
    {
        // Arrange
        var configure = new Db2DevartConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new Db2DevartOptions { ConnectionString = "Test", Command = string.Empty };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Command).IsEmpty();
    }

    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new Db2DevartConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new Db2DevartOptions();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => configure.Configure(name, options));
    }

    [Test]
    public void Configure_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new Db2DevartConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(Db2DevartOptions)!;

        // Act & Assert
        _ = Assert.Throws<NullReferenceException>(() => configure.Configure(name, options));
    }
}