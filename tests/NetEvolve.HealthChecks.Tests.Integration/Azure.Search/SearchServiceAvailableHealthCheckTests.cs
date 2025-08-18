namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Search;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Search;

[TestGroup($"{nameof(Azure)}.{nameof(Search)}")]
public class SearchServiceAvailableHealthCheckTests : HealthCheckTestBase
{
    // Note: These tests require a real Azure Cognitive Search service
    // They are skipped by default as they would need valid credentials and endpoint
    
    [Test]
    [Skip("Requires real Azure Cognitive Search service")]
    public async Task AddSearchServiceAvailability_UseOptions_ModeApiKey_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSearchServiceAvailability(
                    "SearchServiceApiKeyHealthy",
                    options =>
                    {
                        options.Mode = SearchClientCreationMode.ApiKey;
                        options.ServiceUri = new Uri("https://test-search-service.search.windows.net");
                        options.ApiKey = "test-api-key";
                        options.Timeout = 5000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    [Skip("Requires real Azure Cognitive Search service")]
    public async Task AddSearchServiceAvailability_UseOptions_ModeApiKey_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSearchServiceAvailability(
                    "SearchServiceApiKeyDegraded",
                    options =>
                    {
                        options.Mode = SearchClientCreationMode.ApiKey;
                        options.ServiceUri = new Uri("https://test-search-service.search.windows.net");
                        options.ApiKey = "test-api-key";
                        options.Timeout = 0; // This should cause timeout
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    [Skip("Requires real Azure Cognitive Search service")]
    public async Task AddSearchServiceAvailability_UseOptions_ModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSearchServiceAvailability(
                    "SearchServiceConnectionStringHealthy",
                    options =>
                    {
                        options.Mode = SearchClientCreationMode.ConnectionString;
                        options.ConnectionString = "Endpoint=https://test-search-service.search.windows.net;ApiKey=test-api-key";
                        options.Timeout = 5000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    [Skip("Requires real Azure Cognitive Search service")]
    public async Task AddSearchServiceAvailability_UseOptions_ModeDefaultAzureCredentials_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSearchServiceAvailability(
                    "SearchServiceDefaultCredentialsHealthy",
                    options =>
                    {
                        options.Mode = SearchClientCreationMode.DefaultAzureCredentials;
                        options.ServiceUri = new Uri("https://test-search-service.search.windows.net");
                        options.Timeout = 5000;
                    }
                );
            },
            HealthStatus.Healthy
        );
}