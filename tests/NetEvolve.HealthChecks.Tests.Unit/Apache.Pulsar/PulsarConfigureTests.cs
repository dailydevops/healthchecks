namespace NetEvolve.HealthChecks.Tests.Unit.Pulsar;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Apache.Pulsar;

[TestGroup($"{nameof(Apache)}.{nameof(Pulsar)}")]
public sealed class PulsarConfigureTests
{
    [Test]
    public async Task Configure_WithNameAndOptions_BindsConfigurationCorrectly()
    {
        var configValues = new Dictionary<string, string?>
        {
            ["HealthChecks:Pulsar:TestName:KeyedService"] = "test-key",
            ["HealthChecks:Pulsar:TestName:Timeout"] = "200",
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();

        var options = new PulsarOptions();
        var configure = new PulsarConfigure(configuration);

        configure.Configure("TestName", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(options.KeyedService).IsEqualTo("test-key");
            _ = await Assert.That(options.Timeout).IsEqualTo(200);
        }
    }

    [Test]
    public void Configure_WithNullName_ThrowsArgumentException()
    {
        var configuration = new ConfigurationBuilder().Build();
        var options = new PulsarOptions();
        var configure = new PulsarConfigure(configuration);
        const string? name = null;

        _ = Assert.Throws<ArgumentNullException>(() => configure.Configure(name, options));
    }

    [Test]
    public void Configure_WithEmptyName_ThrowsArgumentException()
    {
        var configuration = new ConfigurationBuilder().Build();
        var options = new PulsarOptions();
        var configure = new PulsarConfigure(configuration);
        var name = string.Empty;

        _ = Assert.Throws<ArgumentException>(() => configure.Configure(name, options));
    }

    [Test]
    public async Task Validate_WithValidOptions_ReturnsSuccess()
    {
        var configuration = new ConfigurationBuilder().Build();
        var options = new PulsarOptions { Timeout = 100 };
        var configure = new PulsarConfigure(configuration);

        var result = configure.Validate("TestName", options);

        _ = await Assert.That(result).IsEqualTo(ValidateOptionsResult.Success);
    }

    [Test]
    public async Task Validate_WithNullName_ReturnsFail()
    {
        var configuration = new ConfigurationBuilder().Build();
        var options = new PulsarOptions();
        var configure = new PulsarConfigure(configuration);

        var result = configure.Validate(null, options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WithEmptyName_ReturnsFail()
    {
        var configuration = new ConfigurationBuilder().Build();
        var options = new PulsarOptions();
        var configure = new PulsarConfigure(configuration);

        var result = configure.Validate("", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WithNullOptions_ReturnsFail()
    {
        var configuration = new ConfigurationBuilder().Build();
        var configure = new PulsarConfigure(configuration);

        var result = configure.Validate("TestName", null!);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The option cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WithInvalidTimeout_ReturnsFail()
    {
        var configuration = new ConfigurationBuilder().Build();
        var options = new PulsarOptions
        {
            Timeout = -2, // -1 is valid (Infinite), but -2 is not
        };
        var configure = new PulsarConfigure(configuration);

        var result = configure.Validate("TestName", options);

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
