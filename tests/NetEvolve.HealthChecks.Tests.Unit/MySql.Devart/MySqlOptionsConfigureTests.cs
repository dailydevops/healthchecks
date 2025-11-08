namespace NetEvolve.HealthChecks.Tests.Unit.MySql.Devart;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.MySql.Devart;

[TestGroup($"{nameof(MySql)}.{nameof(Devart)}")]
public sealed class MySqlOptionsConfigureTests
{
    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MySqlDevartConfigure(configuration);
        var options = new MySqlDevartOptions();

        // Act
        void Act() => configure.Configure(null!, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public async Task PostConfigure_WhenNameIsNull_DoNothing()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MySqlDevartConfigure(configuration);
        var options = new MySqlDevartOptions { Command = "SELECT 1;" };

        // Act
        configure.PostConfigure(null, options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo("SELECT 1;");
    }

    [Test]
    public async Task PostConfigure_WhenCommandIsNull_SetToDefault()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MySqlDevartConfigure(configuration);
        var options = new MySqlDevartOptions { Command = null! };

        // Act
        configure.PostConfigure("Test", options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(MySqlDevartHealthCheck.DefaultCommand);
    }

    [Test]
    public async Task PostConfigure_WhenCommandIsEmpty_SetToDefault()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MySqlDevartConfigure(configuration);
        var options = new MySqlDevartOptions { Command = string.Empty };

        // Act
        configure.PostConfigure("Test", options);

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(MySqlDevartHealthCheck.DefaultCommand);
    }

    [Test]
    public async Task Validate_WhenNameIsNull_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MySqlDevartConfigure(configuration);
        var options = new MySqlDevartOptions();

        // Act
        var result = configure.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenOptionsIsNull_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MySqlDevartConfigure(configuration);

        // Act
        var result = configure.Validate("Test", null!);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The option cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WhenConnectionStringIsNull_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MySqlDevartConfigure(configuration);
        var options = new MySqlDevartOptions { ConnectionString = null! };

        // Act
        var result = configure.Validate("Test", options);

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
    public async Task Validate_WhenConnectionStringIsEmpty_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MySqlDevartConfigure(configuration);
        var options = new MySqlDevartOptions { ConnectionString = string.Empty };

        // Act
        var result = configure.Validate("Test", options);

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
    public async Task Validate_WhenTimeoutIsLessThanMinusOne_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MySqlDevartConfigure(configuration);
        var options = new MySqlDevartOptions { ConnectionString = "Server=localhost;", Timeout = -2 };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The timeout cannot be less than infinite (-1).");
        }
    }

    [Test]
    public async Task Validate_WhenOptionsAreValid_ReturnSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MySqlDevartConfigure(configuration);
        var options = new MySqlDevartOptions { ConnectionString = "Server=localhost;" };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }
}
