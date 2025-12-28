namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ApplicationInsights;

using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ApplicationInsights;

[TestGroup($"{nameof(Azure)}.{nameof(ApplicationInsights)}")]
public sealed class ClientCreationTests
{
    [Test]
    public async Task CreateTelemetryClient_WhenConnectionStringMode_ShouldReturnClient()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.ConnectionString,
            ConnectionString =
                "InstrumentationKey=12345678-1234-1234-1234-123456789abc;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/",
        };

        // Act
        var client = ClientCreation.CreateTelemetryClient(options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(client).IsNotNull();
            _ = await Assert.That(client).IsTypeOf<TelemetryClient>();
            _ = await Assert.That(client.TelemetryConfiguration.ConnectionString).IsEqualTo(options.ConnectionString);
        }
    }

    [Test]
    public async Task CreateTelemetryClient_WhenInstrumentationKeyMode_ShouldReturnClient()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.InstrumentationKey,
            InstrumentationKey = "12345678-1234-1234-1234-123456789abc",
        };

        // Act
        var client = ClientCreation.CreateTelemetryClient(options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(client).IsNotNull();
            _ = await Assert.That(client).IsTypeOf<TelemetryClient>();
            _ = await Assert
                .That(client.TelemetryConfiguration.InstrumentationKey)
                .IsEqualTo(options.InstrumentationKey);
        }
    }

    [Test]
    public void CreateTelemetryClient_WhenInvalidMode_ThrowsException()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions { Mode = (ApplicationInsightsClientCreationMode)999 };

        // Act & Assert
        _ = Assert.Throws<System.Diagnostics.UnreachableException>(() => ClientCreation.CreateTelemetryClient(options));
    }

    [Test]
    public async Task GetTelemetryClient_WhenCalledMultipleTimes_ShouldReturnSameInstance()
    {
        // Arrange
        var clientCreation = new ClientCreation();
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.ConnectionString,
            ConnectionString =
                "InstrumentationKey=12345678-1234-1234-1234-123456789abc;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/",
        };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        const string name = "test";

        // Act
        var client1 = clientCreation.GetTelemetryClient(name, options, serviceProvider);
        var client2 = clientCreation.GetTelemetryClient(name, options, serviceProvider);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(client1).IsNotNull();
            _ = await Assert.That(client2).IsNotNull();
            _ = await Assert.That(client1).IsEqualTo(client2);
        }
    }
}
