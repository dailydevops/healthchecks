namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.HealthChecks.Elasticsearch;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Cluster;

public abstract class ElasticsearchHealthCheckBaseTests : HealthCheckTestBase, IAsyncInitializer
{
    protected IContainerCluster _cluster { get; }
    private ElasticsearchClient _client = default!;

    protected ElasticsearchHealthCheckBaseTests(IContainerCluster cluster) => _cluster = cluster;

    public async Task InitializeAsync()
    {
        var connectionStrings = _cluster.ConnectionStrings.ToArray();

#pragma warning disable CA2000 // Dispose objects before losing scope
        var clientSettings = new ElasticsearchClientSettings(new Uri(connectionStrings[0]));
#pragma warning restore CA2000 // Dispose objects before losing scope

        if (!string.IsNullOrWhiteSpace(_cluster.Password))
        {
            _ = clientSettings.Authentication(new BasicAuthentication(_cluster.Username, _cluster.Password));
        }

        _client = new ElasticsearchClient(
            clientSettings.ServerCertificateValidationCallback(CertificateValidations.AllowAll)
        );

        await Task.CompletedTask;
    }

    [Test]
    public async Task AddElasticsearch_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearch("TestContainerHealthy", options => options.Timeout = 5000),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearch_UseOptionsWithKeyedService_Healthy()
    {
        const string serviceKey = "options-test-key";

        await RunAndVerify(
            healthChecks =>
                healthChecks.AddElasticsearch(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = serviceKey;
                        options.Timeout = 5000;
                    }
                ),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton(serviceKey, (_, _) => _client)
        );
    }

    [Test]
    public async Task AddElasticsearch_UseOptionsWithUsernameAndPasswordMode_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddElasticsearch(
                    "TestContainerUsernameAndPasswordHealthy",
                    options =>
                    {
                        options.Mode = ElasticsearchClientCreationMode.UsernameAndPassword;
                        options.Timeout = 5000;
                        options.Username = _cluster.Username;
                        options.Password = _cluster.Password;
                        options.ConnectionStrings.Add(_cluster.ConnectionStrings.ElementAt(0));
                    }
                ),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy
        );

    [Test]
    public async Task AddElasticsearch_UseOptionsDoubleRegistered_Healthy() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks.AddElasticsearch("TestContainerHealthy").AddElasticsearch("TestContainerHealthy"),
                    Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(_client)
                )
        );

    [Test]
    public async Task AddElasticsearch_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddElasticsearch(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (client, cancellationToken) =>
                        {
                            await Task.Delay(5000, cancellationToken);
                            return await ElasticsearchHealthCheck
                                .DefaultCommandAsync(client, cancellationToken)
                                .ConfigureAwait(false);
                        };
                        options.Timeout = 0;
                    }
                ),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearch_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddElasticsearch(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.CommandAsync = async (_, _) =>
                        {
                            await Task.CompletedTask;
                            throw new InvalidOperationException("test");
                        };
                    }
                );
            },
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearch_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearch("TestContainerHealthy"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Elasticsearch:TestContainerHealthy:Timeout", "5000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearch_UseConfigurationWithKeyedService_Healthy()
    {
        const string serviceKey = "config-test-key";

        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearch("TestContainerKeyedHealthy"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Elasticsearch:TestContainerKeyedHealthy:KeyedService", serviceKey },
                    { "HealthChecks:Elasticsearch:TestContainerKeyedHealthy:Timeout", "5000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton(serviceKey, (_, _) => _client)
        );
    }

    [Test]
    public async Task AddElasticsearch_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearch("TestContainerDegraded"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Elasticsearch:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearch_UseConfiguration_TimeoutMinusTwo_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearch("TestNoValues"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Elasticsearch:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearch_UseConfiguration_ConnectionStringEmpty_ThrowsException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearch("TestNoValues"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:Elasticsearch:TestNoValues:Mode",
                        $"{ElasticsearchClientCreationMode.UsernameAndPassword}"
                    },
                    { "HealthChecks:Elasticsearch:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearch_UseConfiguration_UsernameNull_ThrowsException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearch("TestNoValues"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:Elasticsearch:TestNoValues:Mode",
                        $"{ElasticsearchClientCreationMode.UsernameAndPassword}"
                    },
                    { "HealthChecks:Elasticsearch:TestNoValues:ConnectionString", "connection-string" },
                    { "HealthChecks:Elasticsearch:TestNoValues:Password", "password" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearch_UseConfiguration_PasswordNull_ThrowsException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearch("TestNoValues"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:Elasticsearch:TestNoValues:Mode",
                        $"{ElasticsearchClientCreationMode.UsernameAndPassword}"
                    },
                    { "HealthChecks:Elasticsearch:TestNoValues:ConnectionString", "connection-string" },
                    { "HealthChecks:Elasticsearch:TestNoValues:Username", "username" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearch_UseClusterClient_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddElasticsearch(
                    "TestClusterHealthy",
                    options =>
                    {
                        options.Mode = ElasticsearchClientCreationMode.UsernameAndPassword;
                        options.Timeout = 5000;
                        options.Username = _cluster.Username;
                        options.Password = _cluster.Password;

                        foreach (var connectionString in _cluster.ConnectionStrings)
                        {
                            options.ConnectionStrings.Add(connectionString);
                        }
                    }
                ),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy
        );

    [Test]
    public async Task AddElasticsearch_UseClusterClient_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddElasticsearch(
                    "TestClusterDegraded",
                    options =>
                    {
                        options.Mode = ElasticsearchClientCreationMode.UsernameAndPassword;
                        options.Timeout = 0;
                        options.Username = _cluster.Username;
                        options.Password = _cluster.Password;

                        foreach (var connectionString in _cluster.ConnectionStrings)
                        {
                            options.ConnectionStrings.Add(connectionString);
                        }
                    }
                ),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded
        );

    [Test]
    public async Task AddElasticsearch_UseClusterClient_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddElasticsearch(
                    "TestClusterUnhealthy",
                    options =>
                    {
                        options.Mode = ElasticsearchClientCreationMode.UsernameAndPassword;
                        options.Timeout = 5000;
                        options.Username = _cluster.Username;
                        options.Password = _cluster.Password;

                        foreach (var connectionString in _cluster.ConnectionStrings)
                        {
                            options.ConnectionStrings.Add(connectionString);
                        }

                        options.CommandAsync = async (_, _) =>
                        {
                            await Task.CompletedTask;
                            throw new InvalidOperationException("test");
                        };
                    }
                ),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy
        );
}
