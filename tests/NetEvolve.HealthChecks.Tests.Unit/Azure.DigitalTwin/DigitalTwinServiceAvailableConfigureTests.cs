namespace NetEvolve.HealthChecks.Tests.Unit.Azure.DigitalTwin;

using System;
using System.Collections.Generic;
using global::Azure.DigitalTwins.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.DigitalTwin;

[TestGroup($"{nameof(Azure)}.{nameof(DigitalTwin)}")]
public sealed class DigitalTwinServiceAvailableConfigureTests
{
    [Test]
    public void Configure_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new DigitalTwinServiceAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new DigitalTwinServiceAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        DigitalTwinServiceAvailableOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new DigitalTwinServiceAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsEqualTo(expectedResult);
            if (expectedResult)
            {
                _ = await Assert.That(result.Failures).IsNull();
            }
            else
            {
                _ = await Assert.That(result.FailureMessage).IsEqualTo(expectedMessage);
            }
        }
    }

    public static IEnumerable<(bool expectedResult, string? expectedMessage, string? name, DigitalTwinServiceAvailableOptions options)> GetValidateTestCases()
    {
        yield return (false, "The name cannot be null or whitespace.", null, new DigitalTwinServiceAvailableOptions());
        yield return (false, "The name cannot be null or whitespace.", "", new DigitalTwinServiceAvailableOptions());
        yield return (false, "The option cannot be null.", "Name", null!);
        yield return (false, "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.", "Name", new DigitalTwinServiceAvailableOptions { Timeout = -2 });
        yield return (false, "The mode `null` is not supported.", "Name", new DigitalTwinServiceAvailableOptions { Mode = null });
        
        yield return (
            false, 
            "No service of type `DigitalTwinsClient` registered. Please execute `builder.AddAzureClients()`.", 
            "Name", 
            new DigitalTwinServiceAvailableOptions { Mode = DigitalTwinClientCreationMode.ServiceProvider }
        );

        yield return (
            false, 
            "The service url cannot be null when using `DefaultAzureCredentials` mode.", 
            "Name", 
            new DigitalTwinServiceAvailableOptions { Mode = DigitalTwinClientCreationMode.DefaultAzureCredentials, ServiceUri = null }
        );

        yield return (
            false, 
            "The service url must be an absolute url when using `DefaultAzureCredentials` mode.", 
            "Name", 
            new DigitalTwinServiceAvailableOptions { Mode = DigitalTwinClientCreationMode.DefaultAzureCredentials, ServiceUri = new Uri("/relative", UriKind.Relative) }
        );
    }

    [Test]
    public async Task Validate_WithRegisteredService_Succeeds()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(new DigitalTwinsClient(new Uri("https://test.api.wcus.digitaltwins.azure.net"), default));
        var configure = new DigitalTwinServiceAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        var options = new DigitalTwinServiceAvailableOptions { Mode = DigitalTwinClientCreationMode.ServiceProvider };

        // Act
        var result = configure.Validate("Name", options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WithDefaultAzureCredentialsMode_Succeeds()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new DigitalTwinServiceAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        var options = new DigitalTwinServiceAvailableOptions 
        { 
            Mode = DigitalTwinClientCreationMode.DefaultAzureCredentials,
            ServiceUri = new Uri("https://test.api.wcus.digitaltwins.azure.net")
        };

        // Act
        var result = configure.Validate("Name", options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }
}