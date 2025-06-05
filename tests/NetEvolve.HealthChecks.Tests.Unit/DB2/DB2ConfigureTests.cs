namespace NetEvolve.HealthChecks.Tests.Unit.DB2;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.DB2;

[TestGroup(nameof(DB2))]
public sealed class DB2ConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new DB2Options();
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
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
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(DB2Options);

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
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new DB2Options();

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The connection string cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutLessThanInfinite_ThrowArgumentException()
    {
        // Arrange
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new DB2Options { ConnectionString = "Test", Timeout = -2 };

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
    public async Task Validate_WhenArgumentCommandNull_SetDefaultCommand()
    {
        // Arrange
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new DB2Options { ConnectionString = "Test" };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(options.Command).IsEqualTo(DB2HealthCheck.DefaultCommand);
        }
    }

    [Test]
    public async Task PostConfigure_WhenArgumentCommandEmpty_SetDefaultCommand()
    {
        // Arrange
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new DB2Options { ConnectionString = "Test", Command = string.Empty };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(DB2HealthCheck.DefaultCommand);
    }

    [Test]
    public async Task PostConfigure_WhenNameIsNull_DoNothing()
    {
        // Arrange
        var configure = new DB2Configure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new DB2Options { ConnectionString = "Test", Command = string.Empty };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.Command).IsEqualTo(string.Empty);
            _ = await Assert.That(options.ConnectionString).IsEqualTo("Test");
        }
    }

    [Test]
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

    [Test]
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
