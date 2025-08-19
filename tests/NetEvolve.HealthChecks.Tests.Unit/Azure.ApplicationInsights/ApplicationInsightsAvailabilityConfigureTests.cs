namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ApplicationInsights;

using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ApplicationInsights;

[TestGroup($"{nameof(Azure)}.{nameof(ApplicationInsights)}")]
public sealed class ApplicationInsightsAvailabilityConfigureTests
{
    [Test]
    public void Configure_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ApplicationInsightsAvailabilityOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        ApplicationInsightsAvailabilityOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsEqualTo(expectedResult);
            _ = await Assert.That(result.FailureMessage).IsEqualTo(expectedMessage);
        }
    }

    public static IEnumerable<
        Func<(bool, string?, string?, ApplicationInsightsAvailabilityOptions)>
    > GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The option cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.",
                "name",
                new ApplicationInsightsAvailabilityOptions { Timeout = -2 }
            );

        // ConnectionString mode validation
        yield return () =>
            (
                false,
                "The connection string cannot be null or whitespace when using `ConnectionString` mode.",
                "name",
                new ApplicationInsightsAvailabilityOptions
                {
                    Mode = ApplicationInsightsClientCreationMode.ConnectionString,
                    ConnectionString = null,
                }
            );
        yield return () =>
            (
                false,
                "The connection string cannot be null or whitespace when using `ConnectionString` mode.",
                "name",
                new ApplicationInsightsAvailabilityOptions
                {
                    Mode = ApplicationInsightsClientCreationMode.ConnectionString,
                    ConnectionString = "\t",
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new ApplicationInsightsAvailabilityOptions
                {
                    Mode = ApplicationInsightsClientCreationMode.ConnectionString,
                    ConnectionString =
                        "InstrumentationKey=12345678-1234-1234-1234-123456789abc;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/",
                }
            );

        // InstrumentationKey mode validation
        yield return () =>
            (
                false,
                "The instrumentation key cannot be null or whitespace when using `InstrumentationKey` mode.",
                "name",
                new ApplicationInsightsAvailabilityOptions
                {
                    Mode = ApplicationInsightsClientCreationMode.InstrumentationKey,
                    InstrumentationKey = null,
                }
            );
        yield return () =>
            (
                false,
                "The instrumentation key cannot be null or whitespace when using `InstrumentationKey` mode.",
                "name",
                new ApplicationInsightsAvailabilityOptions
                {
                    Mode = ApplicationInsightsClientCreationMode.InstrumentationKey,
                    InstrumentationKey = "\t",
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new ApplicationInsightsAvailabilityOptions
                {
                    Mode = ApplicationInsightsClientCreationMode.InstrumentationKey,
                    InstrumentationKey = "12345678-1234-1234-1234-123456789abc",
                }
            );

        // ServiceProvider mode validation (without TelemetryClient registered)
        yield return () =>
            (
                false,
                "No service of type `TelemetryClient` registered. Please register Application Insights using AddApplicationInsightsTelemetry().",
                "name",
                new ApplicationInsightsAvailabilityOptions
                {
                    Mode = ApplicationInsightsClientCreationMode.ServiceProvider,
                }
            );

        // ServiceProvider mode validation (with TelemetryClient registered)
        var servicesWithTelemetry = new ServiceCollection();
        servicesWithTelemetry.AddSingleton<TelemetryClient>();
        var providerWithTelemetry = servicesWithTelemetry.BuildServiceProvider();
        var configureWithTelemetry = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            providerWithTelemetry
        );

        yield return () =>
            (
                true,
                null,
                "name",
                new ApplicationInsightsAvailabilityOptions
                {
                    Mode = ApplicationInsightsClientCreationMode.ServiceProvider,
                }
            );

        // Unsupported mode
        yield return () =>
            (
                false,
                "The mode `` is not supported.",
                "name",
                new ApplicationInsightsAvailabilityOptions { Mode = null }
            );
    }
}
