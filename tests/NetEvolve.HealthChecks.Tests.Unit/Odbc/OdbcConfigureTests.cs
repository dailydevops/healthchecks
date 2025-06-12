namespace NetEvolve.HealthChecks.Tests.Unit.Odbc;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Odbc;

[TestGroup(nameof(Odbc))]
public sealed class OdbcConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new OdbcOptions();
        var configure = new OdbcConfigure(new ConfigurationBuilder().Build());
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
        var configure = new OdbcConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(OdbcOptions);

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
        var configure = new OdbcConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new OdbcOptions();

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
        var configure = new OdbcConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new OdbcOptions { ConnectionString = "Test", Timeout = -2 };

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
        var configure = new OdbcConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new OdbcOptions { ConnectionString = "Test" };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(options.Command).IsEqualTo(OdbcHealthCheck.DefaultCommand);
        }
    }

    [Test]
    public async Task Validate_WhenArgumentsValid_ReturnSuccess()
    {
        // Arrange
        var configure = new OdbcConfigure(new ConfigurationBuilder().Build());
        const string name = "Test";
        var options = new OdbcOptions
        {
            ConnectionString = "Test",
            Command = "SELECT 1;",
            Timeout = 100,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsNull();
        }
    }

    [Test]
    public async Task PostConfigure_WhenArgumentCommandEmpty_SetDefaultCommand()
    {
        // Arrange
        var configure = new OdbcConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new OdbcOptions { ConnectionString = "Test", Command = string.Empty };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(OdbcHealthCheck.DefaultCommand);
    }

    [Test]
    public async Task PostConfigure_WhenCommandNotEmpty_LeaveUnchanged()
    {
        // Arrange
        var configure = new OdbcConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        const string customCommand = "SELECT custom_column FROM custom_table";
        var options = new OdbcOptions { ConnectionString = "Test", Command = customCommand };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(customCommand);
    }

    [Test]
    public async Task PostConfigure_WhenNameIsNull_DoNothing()
    {
        // Arrange
        var configure = new OdbcConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new OdbcOptions { ConnectionString = "Test", Command = string.Empty };

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
        var configure = new OdbcConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new OdbcOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public async Task Configure_WithValidName_BindsConfiguration()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            { "HealthChecks:Odbc:TestSection:ConnectionString", "TestConnection" },
            { "HealthChecks:Odbc:TestSection:Timeout", "200" },
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();

        var configure = new OdbcConfigure(configuration);
        const string name = "TestSection";
        var options = new OdbcOptions();

        // Act
        configure.Configure(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.ConnectionString).IsEqualTo("TestConnection");
            _ = await Assert.That(options.Timeout).IsEqualTo(200);
        }
    }

    [Test]
    public void Configure_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new OdbcConfigure(new ConfigurationBuilder().Build());
        var options = new OdbcOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
