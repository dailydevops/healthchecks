namespace NetEvolve.HealthChecks.Tests.Integration.GCP.BigQuery;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.BigQuery;

[TestGroup($"GCP.{nameof(BigQuery)}")]
[ClassDataSource<BigQueryDatabase>(Shared = InstanceSharedType.BigQuery)]
public sealed class BigQueryHealthCheckTests : HealthCheckTestBase
{
    private readonly BigQueryDatabase _database;

    public BigQueryHealthCheckTests(BigQueryDatabase database) => _database = database;

    [Test]
    public async Task AddBigQuery_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddBigQuery("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddSingleton(_ => _database.Client)
        );

    [Test]
    public async Task AddBigQuery_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddBigQuery("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => _ = services.AddSingleton(_ => _database.Client)
        );

    [Test]
    public async Task AddBigQuery_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBigQuery(
                    "TestContainerKeyedServiceHealthy",
                    options =>
                    {
                        options.Timeout = 10000;
                        options.KeyedService = "bigquery";
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddKeyedSingleton("bigquery", (_, _) => _database.Client)
        );

    [Test]
    public async Task AddBigQuery_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddBigQuery("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:BigQuery:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => _database.Client)
        );

    [Test]
    public async Task AddBigQuery_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddBigQuery("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:BigQuery:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => _database.Client)
        );

    [Test]
    public async Task AddBigQuery_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddBigQuery("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:BigQuery:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => _database.Client)
        );
}
