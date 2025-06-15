namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Cluster;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.HealthChecks.Elasticsearch.Cluster;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Cluster.ContainerCluster;

public abstract class ElasticsearchClusterHealthCheckBaseTests : HealthCheckTestBase, IAsyncInitializer
{
    protected IContainerCluster _cluster { get; }
    private ElasticsearchClient _client = default!;

    protected ElasticsearchClusterHealthCheckBaseTests(IContainerCluster cluster) => _cluster = cluster;

    public async Task InitializeAsync()
    {
        var connectionStrings = _cluster.ConnectionStrings.ToArray();

        var nodes = connectionStrings.Select(connectionString => new Uri(connectionString)).ToArray();
        using var pool = new StaticNodePool(nodes);
        using var settings = new ElasticsearchClientSettings(pool);

        if (!string.IsNullOrWhiteSpace(_cluster.Password))
        {
            _ = settings.Authentication(new BasicAuthentication(_cluster.Username, _cluster.Password));
        }

        _client = new ElasticsearchClient(
            settings.ServerCertificateValidationCallback(CertificateValidations.AllowAll)
        );

        await Task.CompletedTask;
    }

    [Test]
    public async Task AddElasticsearchCluster_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddElasticsearchCluster("TestContainerHealthy", options => options.Timeout = 1000),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearchCluster_UseOptionsWithKeyedService_Healthy()
    {
        const string serviceKey = "options-test-key";

        await RunAndVerify(
            healthChecks =>
                healthChecks.AddElasticsearchCluster(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = serviceKey;
                        options.Timeout = 1000;
                    }
                ),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton(serviceKey, (_, _) => _client)
        );
    }

    [Test]
    public async Task AddElasticsearchCluster_UseOptionsWithInternalMode_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddElasticsearchCluster(
                    "TestContainerInternalHealthy",
                    options =>
                    {
                        options.Mode = ElasticsearchClusterClientCreationMode.Internal;
                        options.Timeout = 1000;
                        options.ConnectionStrings = _cluster.ConnectionStrings;
                        options.Username = _cluster.Username;
                        options.Password = _cluster.Password;
                    }
                ),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy
        );

    [Test]
    public async Task AddElasticsearchCluster_UseOptionsDoubleRegistered_Healthy() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks
                            .AddElasticsearchCluster("TestContainerHealthy")
                            .AddElasticsearchCluster("TestContainerHealthy"),
                    Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(_client)
                )
        );

    [Test]
    public async Task AddElasticsearchCluster_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddElasticsearchCluster(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (client, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);

                            return await ElasticsearchClusterHealthCheck
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
    public async Task AddElasticsearchCluster_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddElasticsearchCluster(
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
    public async Task AddElasticsearchCluster_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearchCluster("TestContainerHealthy"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ElasticsearchCluster:TestContainerHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearchCluster_UseConfigurationWithKeyedService_Healthy()
    {
        const string serviceKey = "config-test-key";

        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearchCluster("TestContainerKeyedHealthy"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:ElasticsearchCluster:TestContainerKeyedHealthy:KeyedService", serviceKey },
                    { "HealthChecks:ElasticsearchCluster:TestContainerKeyedHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton(serviceKey, (_, _) => _client)
        );
    }

    [Test]
    public async Task AddElasticsearchCluster_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearchCluster("TestContainerDegraded"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ElasticsearchCluster:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearchCluster_UseConfiguration_TimeoutMinusTwo_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearchCluster("TestNoValues"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ElasticsearchCluster:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearchCluster_UseConfiguration_UsernameNull_ThrowsException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearchCluster("TestNoValues"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:ElasticsearchCluster:TestNoValues:Mode",
                        $"{ElasticsearchClusterClientCreationMode.Internal}"
                    },
                    { "HealthChecks:ElasticsearchCluster:TestNoValues:ConnectionStrings", "connection-string" },
                    { "HealthChecks:ElasticsearchCluster:TestNoValues:Password", "password" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddElasticsearchCluster_UseConfiguration_PasswordNull_ThrowsException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearchCluster("TestNoValues"),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:ElasticsearchCluster:TestNoValues:Mode",
                        $"{ElasticsearchClusterClientCreationMode.Internal}"
                    },
                    { "HealthChecks:ElasticsearchCluster:TestNoValues:ConnectionStrings", "connection-string" },
                    { "HealthChecks:ElasticsearchCluster:TestNoValues:Username", "username" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );
}
