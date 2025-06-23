namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.HealthChecks.Elasticsearch;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

public abstract class ElasticsearchHealthCheckBaseTests : HealthCheckTestBase, IAsyncInitializer, IAsyncDisposable
{
    private readonly ContainerBase _container;
    private readonly bool _isCluster;
    private ElasticsearchClientSettings _clientSettings;
    private ElasticsearchClient _client = default!;

    protected ElasticsearchHealthCheckBaseTests(ContainerBase container, bool isCluster = false)
    {
        _container = container;
        _isCluster = isCluster;
        _clientSettings = default!;
    }

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "As designed.")]
    public async Task InitializeAsync()
    {
        _clientSettings = _isCluster
            ? new ElasticsearchClientSettings(new StaticNodePool(GetClusterConnectionStrings()))
            : new ElasticsearchClientSettings(_container.ConnectionString);
        _clientSettings = new ElasticsearchClientSettings();

        if (!string.IsNullOrWhiteSpace(_container.Password))
        {
            _ = _clientSettings.Authentication(new BasicAuthentication(_container.Username, _container.Password));
        }

        _client = new ElasticsearchClient(
            _clientSettings.ServerCertificateValidationCallback(CertificateValidations.AllowAll)
        );

        await Task.CompletedTask;
    }

    private Uri[] GetClusterConnectionStrings()
    {
        if (_isCluster)
        {
            return Enumerable.Range(0, 3).Select(_ => _container.ConnectionString).ToArray();
        }

        return [_container.ConnectionString];
    }

    public ValueTask DisposeAsync()
    {
        if (_clientSettings is IDisposable disposableClientSettings)
        {
            disposableClientSettings.Dispose();
        }

        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
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
                        options.Username = _container.Username;
                        options.Password = _container.Password;

                        foreach (var connectionString in GetClusterConnectionStrings())
                        {
                            options.ConnectionStrings.Add(connectionString.ToString());
                        }
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
                            return true;
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
                        options.CommandAsync = (_, _) =>
                        {
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
                    { "HealthChecks:Elasticsearch:TestNoValues:ConnectionStrings:0", "" },
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
                    { "HealthChecks:Elasticsearch:TestNoValues:ConnectionStrings:0", "connection-string" },
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
                    { "HealthChecks:Elasticsearch:TestNoValues:ConnectionStrings:0", "connection-string" },
                    { "HealthChecks:Elasticsearch:TestNoValues:Username", "username" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );
}
