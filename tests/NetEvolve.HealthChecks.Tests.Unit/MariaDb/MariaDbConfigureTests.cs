namespace NetEvolve.HealthChecks.Tests.Unit.MariaDb;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.MariaDb;

[TestGroup(nameof(MariaDb))]
public sealed class MariaDbConfigureTests
{
    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentException()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string? name = null;
        var options = new MariaDbOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void Configure_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        var name = string.Empty;
        var options = new MariaDbOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void Configure_WhenArgumentNameWhiteSpace_ThrowArgumentException()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string name = " ";
        var options = new MariaDbOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Configure_WhenArgumentNameIsSet_SetConnectionString()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:MariaDb:Test:ConnectionString",
                        "Server=localhost;Database=test;Uid=test;Pwd=test;"
                    },
                }
            )
            .Build();
        var configure = new MariaDbConfigure(configuration);
        const string name = "Test";
        var options = new MariaDbOptions();

        // Act
        configure.Configure(name, options);

        // Assert
        _ = await Assert.That(options.ConnectionString).IsEqualTo("Server=localhost;Database=test;Uid=test;Pwd=test;");
    }

    [Test]
    public async Task Configure_WhenArgumentNameIsSet_SetTimeout()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MariaDb:Test:Timeout", "1000" },
                }
            )
            .Build();
        var configure = new MariaDbConfigure(configuration);
        const string name = "Test";
        var options = new MariaDbOptions();

        // Act
        configure.Configure(name, options);

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(1000);
    }

    [Test]
    public async Task PostConfigure_WhenArgumentNameNull_DoNothing()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string? name = null;
        var options = new MariaDbOptions();

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(MariaDbHealthCheck.DefaultCommand);
    }

    [Test]
    public async Task PostConfigure_WhenArgumentNameEmpty_DoNothing()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        var name = string.Empty;
        var options = new MariaDbOptions();

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(MariaDbHealthCheck.DefaultCommand);
    }

    [Test]
    public async Task PostConfigure_WhenArgumentNameWhiteSpace_DoNothing()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string name = " ";
        var options = new MariaDbOptions();

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(MariaDbHealthCheck.DefaultCommand);
    }

    [Test]
    public async Task PostConfigure_WhenCommandIsNull_SetDefaultCommand()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new MariaDbOptions { Command = null! };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(MariaDbHealthCheck.DefaultCommand);
    }

    [Test]
    public async Task PostConfigure_WhenCommandIsEmpty_SetDefaultCommand()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new MariaDbOptions { Command = string.Empty };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(MariaDbHealthCheck.DefaultCommand);
    }

    [Test]
    public async Task PostConfigure_WhenCommandIsWhiteSpace_SetDefaultCommand()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new MariaDbOptions { Command = " " };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(MariaDbHealthCheck.DefaultCommand);
    }

    [Test]
    public async Task Validate_WhenArgumentNameNull_ReturnFailed()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string? name = null;
        var options = new MariaDbOptions();

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
    public async Task Validate_WhenArgumentNameEmpty_ReturnFailed()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        var name = string.Empty;
        var options = new MariaDbOptions();

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
    public async Task Validate_WhenArgumentNameWhiteSpace_ReturnFailed()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string name = " ";
        var options = new MariaDbOptions();

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
    public async Task Validate_WhenArgumentOptionsNull_ReturnFailed()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        const MariaDbOptions? options = null;

        // Act
        var result = configure.Validate(name, options!);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The option cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WhenArgumentOptionsConnectionStringNull_ReturnFailed()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new MariaDbOptions { ConnectionString = null! };

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
    public async Task Validate_WhenArgumentOptionsConnectionStringEmpty_ReturnFailed()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new MariaDbOptions { ConnectionString = string.Empty };

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
    public async Task Validate_WhenArgumentOptionsConnectionStringWhiteSpace_ReturnFailed()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new MariaDbOptions { ConnectionString = " " };

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
    public async Task Validate_WhenArgumentOptionsTimeoutMinusTwo_ReturnFailed()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new MariaDbOptions
        {
            ConnectionString = "Server=localhost;Database=test;Uid=test;Pwd=test;",
            Timeout = -2,
        };

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
    public async Task Validate_WhenArgumentsValid_ReturnSuccess()
    {
        // Arrange
        var configure = new MariaDbConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new MariaDbOptions { ConnectionString = "Server=localhost;Database=test;Uid=test;Pwd=test;" };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }
}
