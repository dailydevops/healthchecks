namespace NetEvolve.HealthChecks.Tests.Unit.Garnet;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Garnet;

[TestGroup(nameof(Garnet))]
public class GarnetHealthConfigureTests
{
    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var configure = new GarnetConfigure(configuration);
        var options = new GarnetOptions();
        const string? name = default;

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), Act);
    }

    [Test]
    public async Task Validate_WhenNameNull_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var configure = new GarnetConfigure(configuration);
        var options = new GarnetOptions();
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
    public async Task Validate_WhenOptionsNull_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var configure = new GarnetConfigure(configuration);
        const GarnetOptions? options = default;
        const string name = "Test";

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
    public async Task Validate_WhenTimeoutMinusTwo_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var configure = new GarnetConfigure(configuration);
        var options = new GarnetOptions { Timeout = -2 };
        const string name = "Test";

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }
    }

    [Test]
    public async Task Validate_WhenConnectionStringEmpty_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var configure = new GarnetConfigure(configuration);
        var options = new GarnetOptions { ConnectionString = string.Empty, Mode = ConnectionHandleMode.Create };
        const string name = "Test";

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The property ConnectionString cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Configure_WhenSettingsValid_SetHealthOptions()
    {
        // Arrange
        var values = new Dictionary<string, string?>
        {
            { "HealthChecks:GarnetDatabase:Test:ConnectionString", "localhost:6379" },
            { "HealthChecks:GarnetDatabase:Test:Timeout", "1000" },
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        var configure = new GarnetConfigure(configuration);
        var options = new GarnetOptions();

        // Act
        configure.Configure("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.ConnectionString).IsEqualTo("localhost:6379");
            _ = await Assert.That(options.Timeout).IsEqualTo(1000);
        }
    }
}
