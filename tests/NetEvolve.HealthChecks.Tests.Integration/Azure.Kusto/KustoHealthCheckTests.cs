namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Kusto;

using System;
using System.Linq;
using System.Threading.Tasks;
using global::Kusto.Data;
using global::Kusto.Data.Common;
using global::Kusto.Data.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Kusto;

[TestGroup($"{nameof(Azure)}.{nameof(Kusto)}")]
[ClassDataSource<KustoAccess>(Shared = InstanceSharedType.Kusto)]
public class KustoHealthCheckTests : HealthCheckTestBase
{
    private readonly KustoAccess _container;

    public KustoHealthCheckTests(KustoAccess container) => _container = container;

    [Test]
    public async Task AddKusto_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKusto(
                    "KustoServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = KustoClientCreationMode.ServiceProvider;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                var connectionStringBuilder = new KustoConnectionStringBuilder(_container.ConnectionString);
                var queryProvider = KustoClientFactory.CreateCslQueryProvider(connectionStringBuilder);
                _ = services.AddSingleton<ICslQueryProvider>(queryProvider);
            }
        );

    [Test]
    public async Task AddKusto_UseOptions_ModeServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKusto(
                    "KustoServiceProviderDegraded",
                    options =>
                    {
                        options.Mode = KustoClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services =>
            {
                var connectionStringBuilder = new KustoConnectionStringBuilder(_container.ConnectionString);
                var queryProvider = KustoClientFactory.CreateCslQueryProvider(connectionStringBuilder);
                _ = services.AddSingleton<ICslQueryProvider>(_ => queryProvider);
            }
        );

    [Test]
    public async Task AddKusto_UseOptions_ModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKusto(
                    "KustoConnectionStringHealthy",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = KustoClientCreationMode.ConnectionString;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddKusto_UseOptions_ModeConnectionString_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKusto(
                    "KustoConnectionStringDegraded",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = KustoClientCreationMode.ConnectionString;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddKusto_UseOptions_WithConfigureConnectionStringBuilder_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKusto(
                    "KustoWithConfigurationHealthy",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = KustoClientCreationMode.ConnectionString;
                        options.ConfigureConnectionStringBuilder = builder =>
                        {
                            builder.ApplicationNameForTracing = "HealthCheckTest";
                        };
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );
}
