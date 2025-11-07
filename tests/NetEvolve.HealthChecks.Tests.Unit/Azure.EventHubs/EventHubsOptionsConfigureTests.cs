namespace NetEvolve.HealthChecks.Tests.Unit.Azure.EventHubs;

using System;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.EventHubs;

[TestGroup($"{nameof(Azure)}.{nameof(EventHubs)}")]
public sealed class EventHubsOptionsConfigureTests
{
    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        var options = new EventHubsOptions();

        // Act
        void Act() => configure.Configure(null, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Validate_WhenValidOptions_ReturnSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new System.Collections.Generic.Dictionary<string, string?>
                {
                    ["HealthChecks:AzureEventHubs:Test:Mode"] = "ConnectionString",
                    ["HealthChecks:AzureEventHubs:Test:ConnectionString"] = "Test",
                    ["HealthChecks:AzureEventHubs:Test:EventHubName"] = "Test",
                }
            )
            .Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        var options = new EventHubsOptions();

        configure.Configure("Test", options);

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(result.Succeeded).IsTrue();
        }
    }

    [Test]
    public async Task Validate_WhenInvalidOptions_ReturnFailure()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        var options = new EventHubsOptions();

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(result.Failed).IsTrue();
        }
    }
}
