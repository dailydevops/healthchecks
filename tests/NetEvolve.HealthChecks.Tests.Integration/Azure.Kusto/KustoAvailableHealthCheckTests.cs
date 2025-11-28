namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Kusto;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Kusto;

[TestGroup($"{nameof(Azure)}.{nameof(Kusto)}")]
[TestGroup("Z05TestGroup")]
[ClassDataSource<KustoAccess>(Shared = SharedType.PerClass)]
public class KustoAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly KustoAccess _container;

    public KustoAvailableHealthCheckTests(KustoAccess container) => _container = container;

    [Test]
    public async Task AddKustoAvailability_UseOptions_ConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKustoAvailability(
                    "KustoConnectionStringHealthy",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddKustoAvailability_UseOptions_ConnectionString_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKustoAvailability(
                    "KustoConnectionStringDegraded",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddKustoAvailability_UseOptions_ClusterUri_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKustoAvailability(
                    "KustoClusterUriHealthy",
                    options =>
                    {
                        options.ClusterUri = new Uri(_container.ConnectionString);
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddKustoAvailability_UseOptions_WithDatabaseName_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKustoAvailability(
                    "KustoWithDatabaseHealthy",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.DatabaseName = "NetDefaultDB";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy
        );
}
