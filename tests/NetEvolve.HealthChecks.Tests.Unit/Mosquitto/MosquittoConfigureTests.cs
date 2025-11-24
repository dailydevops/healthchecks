namespace NetEvolve.HealthChecks.Tests.Unit.Mosquitto;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Mosquitto;

[TestGroup(nameof(Mosquitto))]
public sealed class MosquittoConfigureTests
{
    [Test]
    public async Task Configure_WithNameAndOptions_BindsConfigurationCorrectly()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["HealthChecks:Mosquitto:TestName:KeyedService"] = "test-key",
            ["HealthChecks:Mosquitto:TestName:Timeout"] = "200",
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();

        var options = new MosquittoOptions();
        var configure = new MosquittoConfigure(configuration);

        // Act
        configure.Configure("TestName", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.KeyedService).IsEqualTo("test-key");
            _ = await Assert.That(options.Timeout).IsEqualTo(200);
        }
    }

    [Test]
    public void Configure_WithNullName_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var options = new MosquittoOptions();
        var configure = new MosquittoConfigure(configuration);
        const string? name = null;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => configure.Configure(name, options));
    }

    [Test]
    public void Configure_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var options = new MosquittoOptions();
        var configure = new MosquittoConfigure(configuration);
        var name = string.Empty;

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => configure.Configure(name, options));
    }

    [Test]
    public async Task Validate_WithValidOptions_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var options = new MosquittoOptions { Timeout = 100 };
        var configure = new MosquittoConfigure(configuration);

        // Act
        var result = configure.Validate("TestName", options);

        // Assert
        _ = await Assert.That(result).IsEqualTo(ValidateOptionsResult.Success);
    }

    [Test]
    public async Task Validate_WithNullName_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var options = new MosquittoOptions();
        var configure = new MosquittoConfigure(configuration);

        // Act
        var result = configure.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WithEmptyName_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var options = new MosquittoOptions();
        var configure = new MosquittoConfigure(configuration);

        // Act
        var result = configure.Validate("", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WithNullOptions_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MosquittoConfigure(configuration);

        // Act
        var result = configure.Validate("TestName", null!);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The option cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WithInvalidTimeout_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var options = new MosquittoOptions
        {
            Timeout = -2, // -1 is valid (Infinite), but -2 is not
        };
        var configure = new MosquittoConfigure(configuration);

        // Act
        var result = configure.Validate("TestName", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout."
                );
        }
    }
}
