namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ApplicationInsights;

using System;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ApplicationInsights;

[TestGroup($"{nameof(Azure)}.{nameof(ApplicationInsights)}")]
public class ApplicationInsightsAvailabilityOptionsTests
{
    [Test]
    public void Create_WhenDefault_ShouldReturnEmptyOptions()
    {
        // Arrange & Act
        var options = new ApplicationInsightsAvailabilityOptions();

        // Assert
        _ = Assert.That(options.ConnectionString).IsNull();
        _ = Assert.That(options.InstrumentationKey).IsNull();
        _ = Assert.That(options.Mode).IsNull();
        _ = Assert.That(options.Timeout).IsEqualTo(100);
        _ = Assert.That(options.ConfigureConfiguration).IsNull();
    }

    [Test]
    public void SetConnectionString_WhenProvidedValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions();
        var connectionString = "InstrumentationKey=12345678-1234-1234-1234-123456789abc;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/";

        // Act
        options.ConnectionString = connectionString;

        // Assert
        _ = Assert.That(options.ConnectionString).IsEqualTo(connectionString);
    }

    [Test]
    public void SetInstrumentationKey_WhenProvidedValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions();
        var instrumentationKey = "12345678-1234-1234-1234-123456789abc";

        // Act
        options.InstrumentationKey = instrumentationKey;

        // Assert
        _ = Assert.That(options.InstrumentationKey).IsEqualTo(instrumentationKey);
    }

    [Test]
    public void SetMode_WhenProvidedValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions();
        var mode = ApplicationInsightsClientCreationMode.ConnectionString;

        // Act
        options.Mode = mode;

        // Assert
        _ = Assert.That(options.Mode).IsEqualTo(mode);
    }

    [Test]
    public void SetTimeout_WhenProvidedValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions();
        var timeout = 5000;

        // Act
        options.Timeout = timeout;

        // Assert
        _ = Assert.That(options.Timeout).IsEqualTo(timeout);
    }

    [Test]
    public void SetConfigureConfiguration_WhenProvidedValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions();
        static void ConfigureAction(Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration config) { }

        // Act
        options.ConfigureConfiguration = ConfigureAction;

        // Assert
        _ = Assert.That(options.ConfigureConfiguration).IsEqualTo((Action<Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration>)ConfigureAction);
    }
}