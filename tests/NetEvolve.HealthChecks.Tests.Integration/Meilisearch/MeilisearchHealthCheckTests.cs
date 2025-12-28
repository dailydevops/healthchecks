namespace NetEvolve.HealthChecks.Tests.Integration.Meilisearch;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Meilisearch;

[TestGroup(nameof(Meilisearch))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<MeilisearchContainer>(Shared = SharedType.PerClass)]
public class MeilisearchHealthCheckTests : HealthCheckTestBase
{
    private readonly MeilisearchContainer _database;

    public MeilisearchHealthCheckTests(MeilisearchContainer database) => _database = database;

    [Test]
    public async Task AddMeilisearch_UseOptionsInternal_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMeilisearch(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Host = _database.Host;
                        options.Mode = MeilisearchClientCreationMode.Internal;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddMeilisearch_UseOptionsServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMeilisearch(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Mode = MeilisearchClientCreationMode.ServiceProvider;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: builder =>
                _ = builder.AddSingleton(_ => new global::Meilisearch.MeilisearchClient(_database.Host))
        );

    [Test]
    public async Task AddMeilisearch_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMeilisearch(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.Host = _database.Host;
                        options.Timeout = 0;
                        options.Mode = MeilisearchClientCreationMode.Internal;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddMeilisearch_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddMeilisearch("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                _ = config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        { "HealthChecks:Meilisearch:TestContainerHealthy:Host", _database.Host },
                        {
                            "HealthChecks:Meilisearch:TestContainerHealthy:Mode",
                            nameof(MeilisearchClientCreationMode.Internal)
                        },
                        { "HealthChecks:Meilisearch:TestContainerHealthy:Timeout", "10000" },
                    }
                );
            }
        );
}
