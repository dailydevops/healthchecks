namespace NetEvolve.HealthChecks.Tests.Integration.Apache.Solr;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Apache.Solr;

[TestGroup($"{nameof(Apache)}.{nameof(Solr)}")]
[TestGroup("Z00TestGroup")]
[ClassDataSource<SolrContainer>(Shared = SharedType.PerClass)]
public class SolrHealthCheckTests : HealthCheckTestBase
{
    private readonly SolrContainer _database;

    public SolrHealthCheckTests(SolrContainer database) => _database = database;

    [Test]
    public async Task AddSolr_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSolr(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.BaseUrl = new Uri(_database.BaseUrl);
                        options.Core = _database.Core;
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddSolr_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSolr("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Solr:TestContainerHealthy:BaseUrl", _database.BaseUrl },
                    { "HealthChecks:Solr:TestContainerHealthy:Core", _database.Core },
                    { "HealthChecks:Solr:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSolr_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSolr(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.BaseUrl = new Uri(_database.BaseUrl);
                        options.Core = _database.Core;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddSolr_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSolr("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Solr:TestContainerDegraded:BaseUrl", _database.BaseUrl },
                    { "HealthChecks:Solr:TestContainerDegraded:Core", _database.Core },
                    { "HealthChecks:Solr:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSolr_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSolr(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.BaseUrl = new Uri("http://localhost:1");
                        options.Core = "nonexistent";
                        options.Timeout = 100;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddSolr_UseOptions_InvalidCore_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSolr(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.BaseUrl = new Uri(_database.BaseUrl);
                        options.Core = "nonexistent";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Unhealthy
        );
}
