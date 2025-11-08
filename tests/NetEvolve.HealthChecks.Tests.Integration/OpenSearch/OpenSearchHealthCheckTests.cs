namespace NetEvolve.HealthChecks.Tests.Integration.OpenSearch;

using System.Collections.Generic;
using System.Threading.Tasks;
using global::OpenSearch.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.OpenSearch;

[TestGroup(nameof(OpenSearch))]
[ClassDataSource<OpenSearchContainer>(Shared = InstanceSharedType.OpenSearch)]
public class OpenSearchHealthCheckTests : HealthCheckTestBase
{
    private readonly OpenSearchContainer _database;

    public OpenSearchHealthCheckTests(OpenSearchContainer database) => _database = database;

    [Test]
    public async Task AddOpenSearch_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOpenSearch(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionStrings.Add(_database.GetConnectionString());
                        options.Mode = OpenSearchClientCreationMode.UsernameAndPassword;
                        options.Username = "admin";
                        options.Password = "admin";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddOpenSearch_UseOptionsServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOpenSearch("TestContainerHealthy", options => options.Timeout = 10000);
            },
            HealthStatus.Healthy,
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton(services =>
                {
                    var uri = new Uri(_database.GetConnectionString());
                    var settings = new ConnectionSettings(uri);
                    _ = settings.BasicAuthentication("admin", "admin");
                    _ = settings.ServerCertificateValidationCallback((o, certificate, chain, errors) => true);

                    return new OpenSearchClient(settings);
                });
            }
        );

    [Test]
    public async Task AddOpenSearch_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOpenSearch(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionStrings.Add(_database.GetConnectionString());
                        options.Mode = OpenSearchClientCreationMode.UsernameAndPassword;
                        options.Username = "admin";
                        options.Password = "admin";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddOpenSearch_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOpenSearch("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:OpenSearch:TestContainerHealthy:ConnectionStrings:0",
                        _database.GetConnectionString()
                    },
                    { "HealthChecks:OpenSearch:TestContainerHealthy:Mode", "UsernameAndPassword" },
                    { "HealthChecks:OpenSearch:TestContainerHealthy:Username", "admin" },
                    { "HealthChecks:OpenSearch:TestContainerHealthy:Password", "admin" },
                    { "HealthChecks:OpenSearch:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOpenSearch_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOpenSearch("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:OpenSearch:TestContainerDegraded:ConnectionStrings:0",
                        _database.GetConnectionString()
                    },
                    { "HealthChecks:OpenSearch:TestContainerDegraded:Mode", "UsernameAndPassword" },
                    { "HealthChecks:OpenSearch:TestContainerDegraded:Username", "admin" },
                    { "HealthChecks:OpenSearch:TestContainerDegraded:Password", "admin" },
                    { "HealthChecks:OpenSearch:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOpenSearch_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOpenSearch("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:OpenSearch:TestNoValues:ConnectionStrings:0", "" },
                    { "HealthChecks:OpenSearch:TestNoValues:Mode", "UsernameAndPassword" },
                    { "HealthChecks:OpenSearch:TestNoValues:Username", "admin" },
                    { "HealthChecks:OpenSearch:TestNoValues:Password", "admin" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOpenSearch_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOpenSearch("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:OpenSearch:TestNoValues:ConnectionStrings:0", _database.GetConnectionString() },
                    { "HealthChecks:OpenSearch:TestNoValues:Mode", "UsernameAndPassword" },
                    { "HealthChecks:OpenSearch:TestNoValues:Username", "admin" },
                    { "HealthChecks:OpenSearch:TestNoValues:Password", "admin" },
                    { "HealthChecks:OpenSearch:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
