namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ApplicationInsights;

using System;
using Microsoft.ApplicationInsights.Extensibility;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ApplicationInsights;

[TestGroup($"{nameof(Azure)}.{nameof(ApplicationInsights)}")]
public class ApplicationInsightsAvailabilityOptionsTests
{
    [Test]
    public async Task Create_WhenDefault_ShouldReturnEmptyOptions()
    {
        // Arrange & Act
        var options = new ApplicationInsightsAvailabilityOptions();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options).IsNotNull();
            _ = await Assert.That(options.ConnectionString).IsNull();
            _ = await Assert.That(options.InstrumentationKey).IsNull();
            _ = await Assert.That(options.Mode).IsNull();
            _ = await Assert.That(options.Timeout).IsEqualTo(100);
            _ = await Assert.That(options.ConfigureConfiguration).IsNull();
        }
    }

    [Test]
    public async Task SetConnectionString_WhenProvidedValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions();
        var connectionString =
            "InstrumentationKey=12345678-1234-1234-1234-123456789abc;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/";

        // Act
        options.ConnectionString = connectionString;

        // Assert
        _ = await Assert.That(options.ConnectionString).IsEqualTo(connectionString);
    }

    [Test]
    public async Task SetInstrumentationKey_WhenProvidedValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions();
        var instrumentationKey = "12345678-1234-1234-1234-123456789abc";

        // Act
        options.InstrumentationKey = instrumentationKey;

        // Assert
        _ = await Assert.That(options.InstrumentationKey).IsEqualTo(instrumentationKey);
    }

    [Test]
    public async Task SetMode_WhenProvidedValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions();
        var mode = ApplicationInsightsClientCreationMode.ConnectionString;

        // Act
        options.Mode = mode;

        // Assert
        _ = await Assert.That(options.Mode).IsEqualTo(mode);
    }

    [Test]
    public async Task SetTimeout_WhenProvidedValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions();
        var timeout = 5000;

        // Act
        options.Timeout = timeout;

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(timeout);
    }

    [Test]
    public async Task SetConfigureConfiguration_WhenProvidedValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions();
        static void ConfigureAction(TelemetryConfiguration config) => throw new NotImplementedException();

        // Act
        options.ConfigureConfiguration = ConfigureAction;

        // Assert
        _ = await Assert.That(options.ConfigureConfiguration).IsEqualTo(ConfigureAction);
    }
}
