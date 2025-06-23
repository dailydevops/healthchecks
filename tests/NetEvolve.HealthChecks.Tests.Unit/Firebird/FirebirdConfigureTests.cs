namespace NetEvolve.HealthChecks.Tests.Unit.Firebird;

using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Firebird;

[TestGroup(nameof(Firebird))]
public sealed class FirebirdConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new FirebirdOptions();
        var configure = new FirebirdConfigure(new ConfigurationBuilder().Build());
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
        var configure = new FirebirdConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(FirebirdOptions);

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
        var configure = new FirebirdConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new FirebirdOptions();

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
        var configure = new FirebirdConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new FirebirdOptions { ConnectionString = "Test", Timeout = -2 };

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
        var configure = new FirebirdConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new FirebirdOptions { ConnectionString = "Test" };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(options.Command).IsEqualTo(FirebirdHealthCheck.DefaultCommand);
        }
    }

    [Test]
    public async Task PostConfigure_WhenArgumentCommandEmpty_SetDefaultCommand()
    {
        // Arrange
        var configure = new FirebirdConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new FirebirdOptions { ConnectionString = "Test", Command = string.Empty };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(FirebirdHealthCheck.DefaultCommand);
    }

    [Test]
    public async Task PostConfigure_WhenNameIsNull_DoNothing()
    {
        // Arrange
        var configure = new FirebirdConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new FirebirdOptions { ConnectionString = "Test", Command = string.Empty };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.ConnectionString).IsEqualTo("Test");
            _ = await Assert.That(options.Command).IsEqualTo(string.Empty);
        }
    }

    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new FirebirdConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new FirebirdOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public void Configure_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new FirebirdConfigure(new ConfigurationBuilder().Build());
        var options = new FirebirdOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
