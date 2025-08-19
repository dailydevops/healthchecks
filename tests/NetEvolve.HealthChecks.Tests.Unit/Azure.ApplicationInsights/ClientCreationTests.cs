namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ApplicationInsights;

using System;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ApplicationInsights;

[TestGroup($"{nameof(Azure)}.{nameof(ApplicationInsights)}")]
public sealed class ClientCreationTests
{
    [Test]
    public void CreateTelemetryClient_WhenConnectionStringMode_ShouldReturnClient()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.ConnectionString,
            ConnectionString =
                "InstrumentationKey=12345678-1234-1234-1234-123456789abc;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/",
        };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        var client = ClientCreation.CreateTelemetryClient(options, serviceProvider);

        // Assert
        _ = Assert.That(client).IsNotNull();
        _ = Assert.That(client).IsOfType<TelemetryClient>();
        _ = Assert.That(client.TelemetryConfiguration.ConnectionString).IsEqualTo(options.ConnectionString);
    }

    [Test]
    public void CreateTelemetryClient_WhenInstrumentationKeyMode_ShouldReturnClient()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.InstrumentationKey,
            InstrumentationKey = "12345678-1234-1234-1234-123456789abc",
        };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        var client = ClientCreation.CreateTelemetryClient(options, serviceProvider);

        // Assert
        _ = Assert.That(client).IsNotNull();
        _ = Assert.That(client).IsOfType<TelemetryClient>();
        _ = Assert.That(client.TelemetryConfiguration.InstrumentationKey).IsEqualTo(options.InstrumentationKey);
    }

    [Test]
    public void CreateTelemetryClient_WhenServiceProviderMode_ShouldReturnRegisteredClient()
    {
        // Arrange
        var expectedClient = new TelemetryClient();
        var services = new ServiceCollection();
        services.AddSingleton(expectedClient);
        var serviceProvider = services.BuildServiceProvider();

        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.ServiceProvider,
        };

        // Act
        var client = ClientCreation.CreateTelemetryClient(options, serviceProvider);

        // Assert
        _ = Assert.That(client).IsNotNull();
        _ = Assert.That(client).IsEqualTo(expectedClient);
    }

    [Test]
    public void CreateTelemetryClient_WhenInvalidMode_ThrowsException()
    {
        // Arrange
        var options = new ApplicationInsightsAvailabilityOptions { Mode = (ApplicationInsightsClientCreationMode)999 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act & Assert
        _ = Assert.Throws<System.Diagnostics.UnreachableException>(() =>
            ClientCreation.CreateTelemetryClient(options, serviceProvider)
        );
    }

    [Test]
    public void GetTelemetryClient_WhenCalledMultipleTimes_ShouldReturnSameInstance()
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
        var name = "test";

        // Act
        var client1 = clientCreation.GetTelemetryClient(name, options, serviceProvider);
        var client2 = clientCreation.GetTelemetryClient(name, options, serviceProvider);

        // Assert
        _ = Assert.That(client1).IsNotNull();
        _ = Assert.That(client2).IsNotNull();
        _ = Assert.That(client1).IsEqualTo(client2);
    }
}
