namespace NetEvolve.HealthChecks.Tests.Unit.Azure.IotHub;

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.IotHub;

[TestGroup($"{nameof(Azure)}.{nameof(IotHub)}")]
public class IotHubAvailabilityOptionsConfigureTests
{
    [Test]
    public void Configure_WhenConfigurationIsNull_ExpectedDefaults()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new IotHubAvailabilityOptionsConfigure(configuration);
        var options = new IotHubAvailabilityOptions();

        // Act
        configure.Configure("Test", options);

        // Assert
        Assert.That(options.ConnectionString, Is.Null);
        Assert.That(options.FullyQualifiedHostname, Is.Null);
        Assert.That(options.Mode, Is.Null);
        Assert.That(options.Timeout, Is.EqualTo(100));
    }

    [Test]
    public void Configure_WhenConfigurationIsNotNull_ExpectedValues()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["HealthChecks:AzureIotHubAvailability:Test:ConnectionString"] = "HostName=test.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=test",
                    ["HealthChecks:AzureIotHubAvailability:Test:FullyQualifiedHostname"] = "test.azure-devices.net",
                    ["HealthChecks:AzureIotHubAvailability:Test:Mode"] = "ConnectionString",
                    ["HealthChecks:AzureIotHubAvailability:Test:Timeout"] = "1000"
                }
            )
            .Build();
        var configure = new IotHubAvailabilityOptionsConfigure(configuration);
        var options = new IotHubAvailabilityOptions();

        // Act
        configure.Configure("Test", options);

        // Assert
        Assert.That(options.ConnectionString, Is.EqualTo("HostName=test.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=test"));
        Assert.That(options.FullyQualifiedHostname, Is.EqualTo("test.azure-devices.net"));
        Assert.That(options.Mode, Is.EqualTo(ClientCreationMode.ConnectionString));
        Assert.That(options.Timeout, Is.EqualTo(1000));
    }

    [Test]
    public void Validate_WhenOptionsAreInvalid_ExpectFailure()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new IotHubAvailabilityOptionsConfigure(configuration);
        var options = new IotHubAvailabilityOptions();

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        Assert.That(result.Failed, Is.True);
        Assert.That(result.FailureMessage, Is.EqualTo("The client creation mode cannot be null."));
    }

    [Test]
    public void Validate_WhenOptionsAreValid_ExpectSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new IotHubAvailabilityOptionsConfigure(configuration);
        var options = new IotHubAvailabilityOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "HostName=test.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=test"
        };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        Assert.That(result.Succeeded, Is.True);
    }
}