namespace NetEvolve.HealthChecks.Tests.Unit.GCP.Bigtable;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.Bigtable;

[TestGroup($"GCP.{nameof(Bigtable)}")]
public sealed class BigtableConfigureTests
{
    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new BigtableOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new BigtableOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public async Task Configure_WhenConfigurationValid_OptionsConfigured()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    { "HealthChecks:GCP:Bigtable:Test:Timeout", "5000" },
                    { "HealthChecks:GCP:Bigtable:Test:KeyedService", "my-bigtable" },
                }
            )
            .Build();

        var configure = new BigtableOptionsConfigure(config);
        var options = new BigtableOptions();

        // Act
        configure.Configure("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.Timeout).IsEqualTo(5000);
            _ = await Assert.That(options.KeyedService).IsEqualTo("my-bigtable");
        }
    }

    [Test]
    public async Task Configure_WhenConfigurationMissing_DefaultValuesUsed()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();
        var configure = new BigtableOptionsConfigure(config);
        var options = new BigtableOptions();

        // Act
        configure.Configure("Test", options);

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(100);
    }

    [Test]
    public async Task Configure_WhenTimeoutNegative_ValueSet()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { { "HealthChecks:GCP:Bigtable:Test:Timeout", "-1" } }
            )
            .Build();

        var configure = new BigtableOptionsConfigure(config);
        var options = new BigtableOptions();

        // Act
        configure.Configure("Test", options);

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(-1);
    }

    [Test]
    public async Task Validate_WhenTimeoutLessThanMinusOne_ReturnsFailure()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();
        var configure = new BigtableOptionsConfigure(config);
        var options = new BigtableOptions { Timeout = -2 };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).Contains("timeout");
        }
    }

    [Test]
    public async Task Validate_WhenTimeoutValid_ReturnsSuccess()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();
        var configure = new BigtableOptionsConfigure(config);
        var options = new BigtableOptions { Timeout = 1000 };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WhenNameNull_ReturnsFailure()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();
        var configure = new BigtableOptionsConfigure(config);
        var options = new BigtableOptions();

        // Act
        var result = configure.Validate(null, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenOptionsNull_ReturnsFailure()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();
        var configure = new BigtableOptionsConfigure(config);

        // Act
        var result = configure.Validate("Test", null!);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }
}
