namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ApplicationInsights;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ApplicationInsights;
using NetEvolve.HealthChecks.Tests.Integration.Internals;

[TestGroup($"{nameof(Azure)}.{nameof(ApplicationInsights)}")]
public class ApplicationInsightsAvailabilityHealthCheckTests : HealthCheckTestBase
{
    [Test]
    public async Task AddApplicationInsightsAvailability_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddApplicationInsightsAvailability(
                    "AppInsightsServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = ApplicationInsightsClientCreationMode.ServiceProvider;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton<TelemetryClient>()
        );

    [Test]
    public async Task AddApplicationInsightsAvailability_UseOptions_ModeServiceProvider_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddApplicationInsightsAvailability(
                    "AppInsightsServiceProviderUnhealthy",
                    options =>
                    {
                        options.Mode = ApplicationInsightsClientCreationMode.ServiceProvider;
                        options.Timeout = 1;
                    }
                );
            },
            HealthStatus.Unhealthy // No TelemetryClient registered
        );

    [Test]
    public async Task AddApplicationInsightsAvailability_UseOptions_ModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddApplicationInsightsAvailability(
                    "AppInsightsConnectionStringHealthy",
                    options =>
                    {
                        options.Mode = ApplicationInsightsClientCreationMode.ConnectionString;
                        options.ConnectionString =
                            "InstrumentationKey=12345678-1234-1234-1234-123456789abc;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddApplicationInsightsAvailability_UseOptions_ModeInstrumentationKey_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddApplicationInsightsAvailability(
                    "AppInsightsInstrumentationKeyHealthy",
                    options =>
                    {
                        options.Mode = ApplicationInsightsClientCreationMode.InstrumentationKey;
                        options.InstrumentationKey = "12345678-1234-1234-1234-123456789abc";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    // Configuration-based tests
    [Test]
    public async Task AddApplicationInsightsAvailability_UseConfiguration_ConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddApplicationInsightsAvailability("ConfigurationHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:ApplicationInsightsAvailability:ConfigurationHealthy:ConnectionString",
                        "InstrumentationKey=12345678-1234-1234-1234-123456789abc;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/"
                    },
                    {
                        "HealthChecks:ApplicationInsightsAvailability:ConfigurationHealthy:Mode",
                        nameof(ApplicationInsightsClientCreationMode.ConnectionString)
                    },
                    { "HealthChecks:ApplicationInsightsAvailability:ConfigurationHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddApplicationInsightsAvailability_UseConfiguration_InstrumentationKey_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddApplicationInsightsAvailability("ConfigurationInstrumentationKey"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:ApplicationInsightsAvailability:ConfigurationInstrumentationKey:InstrumentationKey",
                        "12345678-1234-1234-1234-123456789abc"
                    },
                    {
                        "HealthChecks:ApplicationInsightsAvailability:ConfigurationInstrumentationKey:Mode",
                        nameof(ApplicationInsightsClientCreationMode.InstrumentationKey)
                    },
                    { "HealthChecks:ApplicationInsightsAvailability:ConfigurationInstrumentationKey:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddApplicationInsightsAvailability_UseConfiguration_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddApplicationInsightsAvailability("ConfigurationUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:ApplicationInsightsAvailability:ConfigurationUnhealthy:ConnectionString",
                        "invalid-connection-string"
                    },
                    {
                        "HealthChecks:ApplicationInsightsAvailability:ConfigurationUnhealthy:Mode",
                        nameof(ApplicationInsightsClientCreationMode.ConnectionString)
                    },
                    { "HealthChecks:ApplicationInsightsAvailability:ConfigurationUnhealthy:Timeout", "1" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
