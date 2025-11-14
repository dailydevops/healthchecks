namespace NetEvolve.HealthChecks.Tests.Integration.Apache.Kafka;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Apache.Kafka;

[TestGroup($"{nameof(Apache)}.{nameof(Kafka)}")]
[TestGroup("Z00TestGroup")]
[ClassDataSource<KafkaContainer>(Shared = InstanceSharedType.Kafka)]
public class KafkaHealthCheckTests : HealthCheckTestBase
{
    private readonly KafkaContainer _database;

    public KafkaHealthCheckTests(KafkaContainer database) => _database = database;

    [Test]
    public async Task AddKafka_UseOptionsCreate_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Configuration = new()
                        {
                            BootstrapServers = _database.BootstrapAddress,
                            EnableDeliveryReports = true,
                        };
                        options.Mode = ProducerHandleMode.Create;
                        options.Timeout = 10000;
                        options.Topic = "TestContainerHealthy";
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddKafka_UseOptionsServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
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
            }
        );

    [Test]
    public async Task AddKafka_UseConfigurationCreate_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKafka("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:Kafka:TestContainerHealthy:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:Kafka:TestContainerDegraded:Configuration:EnableDeliveryReports", "true" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Timeout", "10000" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Topic", "TestContainerHealthy" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddKafka_UseConfigurationServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKafka("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:Kafka:TestContainerHealthy:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:Kafka:TestContainerHealthy:Timeout", "10000" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Topic", "TestContainerHealthy" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Mode", "ServiceProvider" },
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
            }
        );

    [Test]
    public async Task AddKafka_UseOptionsCreate_EnableDeliveryReportsFalse_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
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
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddKafka_UseConfigurationCreate_EnableDeliveryReportsFalse_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKafka("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:Kafka:TestContainerHealthy:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:Kafka:TestContainerHealthy:Configuration:EnableDeliveryReports", "false" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Timeout", "10000" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Topic", "TestContainerHealthy" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddKafka_UseOptionsCreate_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.Configuration = new()
                        {
                            BootstrapServers = _database.BootstrapAddress,
                            EnableDeliveryReports = true,
                        };
                        options.Mode = ProducerHandleMode.Create;
                        options.Timeout = 0;
                        options.Topic = "TestContainerDegraded";
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddKafka_UseOptionsServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
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
            }
        );

    [Test]
    public async Task AddKafka_UseConfigurationCreate_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKafka("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:Kafka:TestContainerDegraded:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:Kafka:TestContainerDegraded:Configuration:EnableDeliveryReports", "true" },
                    { "HealthChecks:Kafka:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:Kafka:TestContainerDegraded:Topic", "TestContainerDegraded" },
                    { "HealthChecks:Kafka:TestContainerDegraded:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddKafka_UseConfigurationServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKafka("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:Kafka:TestContainerDegraded:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:Kafka:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:Kafka:TestContainerDegraded:Topic", "TestContainerDegraded" },
                    { "HealthChecks:Kafka:TestContainerDegraded:Mode", "ServiceProvider" },
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
            }
        );

    [Test]
    public async Task AddKafka_UseOptionsCreate_EnableDeliveryReportsFalse_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
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
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddKafka_UseConfigurationCreate_EnableDeliveryReportsFalse_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKafka("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:Kafka:TestContainerDegraded:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:Kafka:TestContainerDegraded:Configuration:EnableDeliveryReports", "false" },
                    { "HealthChecks:Kafka:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:Kafka:TestContainerDegraded:Topic", "TestContainerDegraded" },
                    { "HealthChecks:Kafka:TestContainerDegraded:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddKafka_UseOptionsCreate_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
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
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddKafka_UseOptionsServiceProvider_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
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
            }
        );

    [Test]
    public async Task AddKafka_UseOptions_ConfigurationNull_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.Configuration = null!;
                        options.Mode = ProducerHandleMode.Create;
                        options.Topic = "TestContainerUnhealthy";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddKafka_UseOptions_BootstrapAddressNull_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.Configuration = new() { BootstrapServers = null! };
                        options.Mode = ProducerHandleMode.Create;
                        options.Topic = "TestContainerUnhealthy";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddKafka_UseOptions_BootstrapAddressWhiteSpace_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.Configuration = new() { BootstrapServers = " " };
                        options.Mode = ProducerHandleMode.Create;
                        options.Topic = "TestContainerUnhealthy";
                    }
                );
            },
            HealthStatus.Unhealthy
        );
}
