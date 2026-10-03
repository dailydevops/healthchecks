namespace NetEvolve.HealthChecks.Tests.Integration.Redpanda;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Redpanda;

[TestGroup(nameof(Redpanda))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<RedpandaDatabase>(Shared = SharedType.PerClass)]
public class RedpandaHealthCheckTests : HealthCheckTestBase
{
    private readonly RedpandaDatabase _database;

    public RedpandaHealthCheckTests(RedpandaDatabase database) => _database = database;

    [Test]
    public async Task AddRedpanda_UseOptionsCreate_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Configuration = new() { BootstrapServers = _database.BootstrapAddress };
                        options.Mode = ProducerHandleMode.Create;
                        options.Timeout = 10000;
                        options.Topic = "TestContainerHealthy";
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseOptionsServiceProvider_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Mode = ProducerHandleMode.ServiceProvider;
                        options.Timeout = 10000;
                        options.Topic = "TestContainerHealthy";
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton(_ =>
                {
                    var config = new ProducerConfig { BootstrapServers = _database.BootstrapAddress };

                    return new ProducerBuilder<string, string>(config).Build();
                });
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseConfigurationCreate_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedpanda("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:RedPanda:TestContainerHealthy:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Timeout", "10000" },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Topic", "TestContainerHealthy" },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseConfigurationServiceProvider_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedpanda("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:RedPanda:TestContainerHealthy:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Timeout", "10000" },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Topic", "TestContainerHealthy" },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Mode", "ServiceProvider" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton(_ =>
                {
                    var config = new ProducerConfig { BootstrapServers = _database.BootstrapAddress };

                    return new ProducerBuilder<string, string>(config).Build();
                });
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseOptionsCreate_EnableDeliveryReportsFalse_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Configuration = new()
                        {
                            BootstrapServers = _database.BootstrapAddress,
                            EnableDeliveryReports = false,
                        };
                        options.Mode = ProducerHandleMode.Create;
                        options.Timeout = 10000;
                        options.Topic = "TestContainerHealthy";
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseConfigurationCreate_EnableDeliveryReportsFalse_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedpanda("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:RedPanda:TestContainerHealthy:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Configuration:EnableDeliveryReports", "false" },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Timeout", "10000" },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Topic", "TestContainerHealthy" },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseOptionsCreate_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.Configuration = new() { BootstrapServers = _database.BootstrapAddress };
                        options.Mode = ProducerHandleMode.Create;
                        options.Timeout = 0;
                        options.Topic = "TestContainerDegraded";
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseOptionsServiceProvider_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.Mode = ProducerHandleMode.ServiceProvider;
                        options.Timeout = 0;
                        options.Topic = "TestContainerDegraded";
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton(_ =>
                {
                    var config = new ProducerConfig { BootstrapServers = _database.BootstrapAddress };

                    return new ProducerBuilder<string, string>(config).Build();
                });
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseConfigurationCreate_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedpanda("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:RedPanda:TestContainerDegraded:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:RedPanda:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:RedPanda:TestContainerDegraded:Topic", "TestContainerDegraded" },
                    { "HealthChecks:RedPanda:TestContainerDegraded:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseConfigurationServiceProvider_Degraded(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedpanda("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:RedPanda:TestContainerDegraded:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:RedPanda:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:RedPanda:TestContainerDegraded:Topic", "TestContainerDegraded" },
                    { "HealthChecks:RedPanda:TestContainerDegraded:Mode", "ServiceProvider" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton(_ =>
                {
                    var config = new ProducerConfig { BootstrapServers = _database.BootstrapAddress };

                    return new ProducerBuilder<string, string>(config).Build();
                });
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseOptionsCreate_EnableDeliveryReportsFalse_Degraded(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.Configuration = new()
                        {
                            BootstrapServers = _database.BootstrapAddress,
                            EnableDeliveryReports = false,
                        };
                        options.Mode = ProducerHandleMode.Create;
                        options.Timeout = 0;
                        options.Topic = "TestContainerDegraded";
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseConfigurationCreate_EnableDeliveryReportsFalse_Degraded(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedpanda("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:RedPanda:TestContainerDegraded:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:RedPanda:TestContainerDegraded:Configuration:EnableDeliveryReports", "false" },
                    { "HealthChecks:RedPanda:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:RedPanda:TestContainerDegraded:Topic", "TestContainerDegraded" },
                    { "HealthChecks:RedPanda:TestContainerDegraded:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseOptionsCreate_Unhealthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.Configuration = new()
                        {
                            BootstrapServers = _database.BootstrapAddress,
                            SocketTimeoutMs = 0,
                        };
                        options.Mode = ProducerHandleMode.Create;
                        options.Topic = "TestContainerUnhealthy";
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseOptionsServiceProvider_Unhealthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.Mode = ProducerHandleMode.ServiceProvider;
                        options.Topic = "TestContainerUnhealthy";
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton(_ =>
                {
                    var config = new ProducerConfig
                    {
                        BootstrapServers = _database.BootstrapAddress,
                        SocketTimeoutMs = 0,
                    };

                    return new ProducerBuilder<string, string>(config).Build();
                });
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseOptions_ConfigurationNull_Unhealthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.Configuration = null!;
                        options.Mode = ProducerHandleMode.Create;
                        options.Topic = "TestContainerUnhealthy";
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseOptions_BootstrapAddressNull_Unhealthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.Configuration = new() { BootstrapServers = null! };
                        options.Mode = ProducerHandleMode.Create;
                        options.Topic = "TestContainerUnhealthy";
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddRedpanda_UseOptions_BootstrapAddressWhiteSpace_Unhealthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.Configuration = new() { BootstrapServers = " " };
                        options.Mode = ProducerHandleMode.Create;
                        options.Topic = "TestContainerUnhealthy";
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );
}
