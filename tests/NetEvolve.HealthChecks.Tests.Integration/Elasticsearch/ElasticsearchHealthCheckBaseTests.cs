namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.HealthChecks.Elasticsearch;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

public abstract class ElasticsearchHealthCheckBaseTests : HealthCheckTestBase, IAsyncInitializer
{
    protected ContainerBase _container { get; }
    private ElasticsearchClient _client = default!;

    protected ElasticsearchHealthCheckBaseTests(ContainerBase container) => _container = container;

    public async Task InitializeAsync()
    {
        using var settings = new ElasticsearchClientSettings(new Uri(_container.ConnectionString));

        if (!string.IsNullOrWhiteSpace(_container.Password))
        {
            _ = settings.Authentication(new BasicAuthentication(_container.Username, _container.Password));
        }

        _client = new ElasticsearchClient(
            settings.ServerCertificateValidationCallback(CertificateValidations.AllowAll)
        );

        await Task.CompletedTask;
    }

    [Test]
    public async Task AddElasticsearch_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddElasticsearch("TestContainerHealthy", options => options.Timeout = 1000),
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
                        options.Timeout = 1000;
                    }
                ),
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton(serviceKey, (_, _) => _client)
        );
    }

    [Test]
    public async Task AddElasticsearch_UseOptionsWithInternalMode_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddElasticsearch(
                    "TestContainerInternalHealthy",
                    options =>
                    {
                        options.Mode = ElasticsearchClientCreationMode.Internal;
                        options.Timeout = 1000;
                        options.ConnectionString = _container.ConnectionString;
                        options.Username = _container.Username;
                        options.Password = _container.Password;
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
                            await Task.Delay(1000, cancellationToken);

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
                    { "HealthChecks:Elasticsearch:TestContainerHealthy:Timeout", "1000" },
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
                    { "HealthChecks:Elasticsearch:TestContainerKeyedHealthy:Timeout", "1000" },
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
                    { "HealthChecks:Elasticsearch:TestNoValues:Mode", $"{ElasticsearchClientCreationMode.Internal}" },
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
                    { "HealthChecks:Elasticsearch:TestNoValues:Mode", $"{ElasticsearchClientCreationMode.Internal}" },
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
                    { "HealthChecks:Elasticsearch:TestNoValues:Mode", $"{ElasticsearchClientCreationMode.Internal}" },
                    { "HealthChecks:Elasticsearch:TestNoValues:ConnectionString", "connection-string" },
                    { "HealthChecks:Elasticsearch:TestNoValues:Username", "username" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );
}
