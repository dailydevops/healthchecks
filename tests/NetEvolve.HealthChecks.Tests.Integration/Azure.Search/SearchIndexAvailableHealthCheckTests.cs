namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Search;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Search;

[TestGroup($"{nameof(Azure)}.{nameof(Search)}")]
public class SearchIndexAvailableHealthCheckTests : HealthCheckTestBase
{
    // Note: These tests require a real Azure Cognitive Search service with an existing index
    // They are skipped by default as they would need valid credentials, endpoint, and index
    
    [Test]
    [Skip("Requires real Azure Cognitive Search service with existing index")]
    public async Task AddSearchIndexAvailability_UseOptions_ModeApiKey_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSearchIndexAvailability(
                    "SearchIndexApiKeyHealthy",
                    options =>
                    {
                        options.Mode = SearchClientCreationMode.ApiKey;
                        options.ServiceUri = new Uri("https://test-search-service.search.windows.net");
                        options.ApiKey = "test-api-key";
                        options.IndexName = "test-index";
                        options.Timeout = 5000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    [Skip("Requires real Azure Cognitive Search service with existing index")]
    public async Task AddSearchIndexAvailability_UseOptions_ModeApiKey_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSearchIndexAvailability(
                    "SearchIndexApiKeyDegraded",
                    options =>
                    {
                        options.Mode = SearchClientCreationMode.ApiKey;
                        options.ServiceUri = new Uri("https://test-search-service.search.windows.net");
                        options.ApiKey = "test-api-key";
                        options.IndexName = "test-index";
                        options.Timeout = 0; // This should cause timeout
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    [Skip("Requires real Azure Cognitive Search service with existing index")]
    public async Task AddSearchIndexAvailability_UseOptions_ModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSearchIndexAvailability(
                    "SearchIndexConnectionStringHealthy",
                    options =>
                    {
                        options.Mode = SearchClientCreationMode.ConnectionString;
                        options.ConnectionString = "Endpoint=https://test-search-service.search.windows.net;ApiKey=test-api-key";
                        options.IndexName = "test-index";
                        options.Timeout = 5000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    [Skip("Requires real Azure Cognitive Search service with existing index")]
    public async Task AddSearchIndexAvailability_UseOptions_IndexNotExists_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSearchIndexAvailability(
                    "SearchIndexNotExistsUnhealthy",
                    options =>
                    {
                        options.Mode = SearchClientCreationMode.ApiKey;
                        options.ServiceUri = new Uri("https://test-search-service.search.windows.net");
                        options.ApiKey = "test-api-key";
                        options.IndexName = "non-existent-index";
                        options.Timeout = 5000;
                    }
                );
            },
            HealthStatus.Unhealthy
        );
}