namespace NetEvolve.HealthChecks.Tests.Unit.Dapr;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Dapr;

[TestGroup(nameof(Dapr))]
public sealed class DaprConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new DaprOptions();
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
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
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(DaprOptions);

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
    public async Task Validate_WhenArgumentTimeoutLessThanInfinite_ThrowArgumentException()
    {
        // Arrange
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new DaprOptions { Timeout = -2 };

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
    public async Task Validate_EverythingFine_Expected()
    {
        // Arrange
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new DaprOptions { Timeout = 100 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new DaprOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public void ConfigureWithoutName_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
        var options = new DaprOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Configure_WithNameParameter_BindsConfigurationCorrectly()
    {
        // Arrange
        var configValues = new Dictionary<string, string?> { { "HealthChecks:TestDapr:Timeout", "10000" } };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();

        var configure = new DaprConfigure(configuration);
        var options = new DaprOptions();
        const string? name = "TestDapr";

        // Act
        configure.Configure(name, options);

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(500);
    }

    [Test]
    public void Configure_WithDefaultName_ThrowsArgumentException()
    {
        // Arrange
        var configure = new DaprConfigure(new ConfigurationBuilder().Build());
        var options = new DaprOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
