namespace NetEvolve.HealthChecks.Tests.Integration.GCP.Bigtable;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.Bigtable;

[TestGroup($"GCP.{nameof(Bigtable)}")]
[TestGroup("Z05TestGroup")]
[ClassDataSource<BigtableDatabase>(Shared = SharedType.PerClass)]
public sealed class BigtableHealthCheckTests : HealthCheckTestBase
{
    private readonly BigtableDatabase _database;

    public BigtableHealthCheckTests(BigtableDatabase database) => _database = database;

    [Test]
    public async Task AddBigtable_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddBigtable(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Timeout = 10000;
                        options.ProjectName = BigtableDatabase.ProjectId;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddSingleton(_ => _database.Client)
        );

    [Test]
    public async Task AddBigtable_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddBigtable(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.Timeout = 0;
                        options.ProjectName = BigtableDatabase.ProjectId;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => _ = services.AddSingleton(_ => _database.Client)
        );

    [Test]
    public async Task AddBigtable_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBigtable(
                    "TestContainerKeyedServiceHealthy",
                    options =>
                    {
                        options.Timeout = 10000;
                        options.KeyedService = "bigtable";
                        options.ProjectName = BigtableDatabase.ProjectId;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddKeyedSingleton("bigtable", (_, _) => _database.Client)
        );

    [Test]
    public async Task AddBigtable_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddBigtable("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:Bigtable:TestContainerHealthy:Timeout", "10000" },
                    { "HealthChecks:GCP:Bigtable:TestContainerHealthy:ProjectName", BigtableDatabase.ProjectId },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => _database.Client)
        );

    [Test]
    public async Task AddBigtable_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddBigtable("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:Bigtable:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:GCP:Bigtable:TestContainerDegraded:ProjectName", BigtableDatabase.ProjectId },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => _database.Client)
        );

    [Test]
    public async Task AddBigtable_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddBigtable("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:Bigtable:TestNoValues:Timeout", "-2" },
                    { "HealthChecks:GCP:Bigtable:TestNoValues:ProjectName", BigtableDatabase.ProjectId },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => _database.Client)
        );
}
