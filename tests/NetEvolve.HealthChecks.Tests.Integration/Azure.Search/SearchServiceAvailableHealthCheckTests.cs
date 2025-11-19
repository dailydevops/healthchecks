namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Search;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Search;
using NetEvolve.HealthChecks.Tests.Integration.Internals;

[TestGroup($"{nameof(Azure)}.{nameof(Search)}")]
[TestGroup("Z02TestGroup")]
public class SearchServiceAvailableHealthCheckTests : HealthCheckTestBase
{
    [Test]
    public async Task AddSearchServiceAvailability_UseOptions_ModeServiceProvider_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSearchServiceAvailability(
                    "SearchServiceProviderUnhealthy",
                    options =>
                    {
                        options.Mode = SearchIndexClientCreationMode.ServiceProvider;
                        options.Timeout = 100;
                    }
                );
            },
            HealthStatus.Unhealthy // No SearchIndexClient registered
        );

    [Test]
    public async Task AddSearchServiceAvailability_UseOptions_ModeAzureKeyCredential_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSearchServiceAvailability(
                    "SearchServiceAzureKeyCredentialUnhealthy",
                    options =>
                    {
                        options.ServiceUri = new Uri("https://invalid-search-service.search.windows.net");
                        options.ApiKey = "invalid-key";
                        options.Mode = SearchIndexClientCreationMode.AzureKeyCredential;
                        options.Timeout = 100;
                    }
                );
            },
            HealthStatus.Unhealthy // Invalid credentials
        );

    [Test]
    public async Task AddSearchServiceAvailability_UseConfiguration_ModeServiceProvider_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSearchServiceAvailability("SearchServiceConfigurationUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureSearchService:SearchServiceConfigurationUnhealthy:Mode",
                        nameof(SearchIndexClientCreationMode.ServiceProvider)
                    },
                    { "HealthChecks:AzureSearchService:SearchServiceConfigurationUnhealthy:Timeout", "100" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSearchServiceAvailability_UseConfiguration_ModeAzureKeyCredential_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSearchServiceAvailability("SearchServiceConfigKeyUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureSearchService:SearchServiceConfigKeyUnhealthy:ServiceUri",
                        "https://invalid-search-service.search.windows.net"
                    },
                    { "HealthChecks:AzureSearchService:SearchServiceConfigKeyUnhealthy:ApiKey", "invalid-key" },
                    {
                        "HealthChecks:AzureSearchService:SearchServiceConfigKeyUnhealthy:Mode",
                        nameof(SearchIndexClientCreationMode.AzureKeyCredential)
                    },
                    { "HealthChecks:AzureSearchService:SearchServiceConfigKeyUnhealthy:Timeout", "100" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
