namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Search;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Search;
using NetEvolve.HealthChecks.Tests.Integration.Internals;

[TestGroup($"{nameof(Azure)}.{nameof(Search)}")]
[TestGroup("Z02TestGroup")]
public class SearchAvailableHealthCheckTests : HealthCheckTestBase
{
    [Test]
    public async Task AddSearchIndexAvailability_UseOptions_ModeServiceProvider_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureSearch(
                    "SearchIndexServiceProviderUnhealthy",
                    options =>
                    {
                        options.IndexName = "test-index";
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.Timeout = 100;
                    }
                );
            },
            HealthStatus.Unhealthy // No SearchClient registered
        );

    [Test]
    public async Task AddSearchIndexAvailability_UseOptions_ModeAzureKeyCredential_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureSearch(
                    "SearchIndexAzureKeyCredentialUnhealthy",
                    options =>
                    {
                        options.ServiceUri = new Uri("https://invalid-search-service.search.windows.net");
                        options.ApiKey = "invalid-key";
                        options.IndexName = "test-index";
                        options.Mode = ClientCreationMode.AzureKeyCredential;
                        options.Timeout = 100;
                    }
                );
            },
            HealthStatus.Unhealthy // Invalid credentials
        );

    [Test]
    public async Task AddSearchIndexAvailability_UseOptions_MissingIndexName_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureSearch(
                    "SearchIndexMissingIndexName",
                    options =>
                    {
                        options.ServiceUri = new Uri("https://invalid-search-service.search.windows.net");
                        options.ApiKey = "invalid-key";
                        options.Mode = ClientCreationMode.AzureKeyCredential;
                        options.Timeout = 100;
                    }
                );
            },
            HealthStatus.Unhealthy // Missing IndexName
        );

    [Test]
    public async Task AddSearchIndexAvailability_UseConfiguration_ModeServiceProvider_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureSearch("SearchIndexConfigurationUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AzureSearchIndex:SearchIndexConfigurationUnhealthy:IndexName", "test-index" },
                    {
                        "HealthChecks:AzureSearchIndex:SearchIndexConfigurationUnhealthy:Mode",
                        nameof(ClientCreationMode.ServiceProvider)
                    },
                    { "HealthChecks:AzureSearchIndex:SearchIndexConfigurationUnhealthy:Timeout", "100" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSearchIndexAvailability_UseConfiguration_ModeAzureKeyCredential_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureSearch("SearchIndexConfigKeyUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureSearchIndex:SearchIndexConfigKeyUnhealthy:ServiceUri",
                        "https://invalid-search-service.search.windows.net"
                    },
                    { "HealthChecks:AzureSearchIndex:SearchIndexConfigKeyUnhealthy:ApiKey", "invalid-key" },
                    { "HealthChecks:AzureSearchIndex:SearchIndexConfigKeyUnhealthy:IndexName", "test-index" },
                    {
                        "HealthChecks:AzureSearchIndex:SearchIndexConfigKeyUnhealthy:Mode",
                        nameof(ClientCreationMode.AzureKeyCredential)
                    },
                    { "HealthChecks:AzureSearchIndex:SearchIndexConfigKeyUnhealthy:Timeout", "100" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
