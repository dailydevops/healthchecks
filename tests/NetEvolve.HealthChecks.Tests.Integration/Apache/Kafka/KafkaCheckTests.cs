namespace NetEvolve.HealthChecks.Tests.Integration.Apache.Kafka;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Apache.Kafka;
using Xunit;

[TestGroup(nameof(Kafka))]
public class KafkaCheckTests : HealthCheckTestBase, IClassFixture<KafkaDatabase>
{
    private readonly KafkaDatabase _database;

    public KafkaCheckTests(KafkaDatabase database) => _database = database;

    [Fact]
    public async Task AddKafka_UseOptionsCreate_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddKafka(
                "TestContainerHealthy",
                options =>
                {
                    options.Configuration = new() { BootstrapServers = _database.BootstrapAddress };
                    options.Mode = ProducerHandleMode.Create;
                    options.Timeout = 5000;
                    options.Topic = "TestContainerHealthy";
                }
            );
        });

    [Fact]
    public async Task AddKafka_UseOptionsServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Mode = ProducerHandleMode.ServiceProvider;
                        options.Timeout = 5000;
                        options.Topic = "TestContainerHealthy";
                    }
                );
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

    [Fact]
    public async Task AddKafka_UseConfigurationCreate_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddKafka("TestContainerHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:Kafka:TestContainerHealthy:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:Kafka:TestContainerHealthy:Timeout", "5000" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Topic", "TestContainerHealthy" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddKafka_UseConfigurationServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddKafka("TestContainerHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:Kafka:TestContainerHealthy:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:Kafka:TestContainerHealthy:Timeout", "5000" },
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

    [Fact]
    public async Task AddKafka_UseOptionsCreate_EnableDeliveryReportsFalse_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
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
                    options.Timeout = 5000;
                    options.Topic = "TestContainerHealthy";
                }
            );
        });

    [Fact]
    public async Task AddKafka_UseConfigurationCreate_EnableDeliveryReportsFalse_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddKafka("TestContainerHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:Kafka:TestContainerHealthy:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:Kafka:TestContainerHealthy:Configuration:EnableDeliveryReports", "false" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Timeout", "5000" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Topic", "TestContainerHealthy" },
                    { "HealthChecks:Kafka:TestContainerHealthy:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddKafka_UseOptionsCreate_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddKafka(
                "TestContainerDegraded",
                options =>
                {
                    options.Configuration = new() { BootstrapServers = _database.BootstrapAddress };
                    options.Mode = ProducerHandleMode.Create;
                    options.Timeout = 0;
                    options.Topic = "TestContainerDegraded";
                }
            );
        });

    [Fact]
    public async Task AddKafka_UseOptionsServiceProvider_ShouldReturnDegraded() =>
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
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton(_ =>
                {
                    var config = new ProducerConfig { BootstrapServers = _database.BootstrapAddress };

                    return new ProducerBuilder<string, string>(config).Build();
                });
            }
        );

    [Fact]
    public async Task AddKafka_UseConfigurationCreate_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddKafka("TestContainerDegraded"),
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
                    { "HealthChecks:Kafka:TestContainerDegraded:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddKafka_UseConfigurationServiceProvider_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddKafka("TestContainerDegraded"),
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

    [Fact]
    public async Task AddKafka_UseOptionsCreate_EnableDeliveryReportsFalse_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
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
        });

    [Fact]
    public async Task AddKafka_UseConfigurationCreate_EnableDeliveryReportsFalse_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddKafka("TestContainerDegraded"),
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

    [Fact]
    public async Task AddKafka_UseOptionsCreate_ShouldReturnUnhealty() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddKafka(
                "TestContainerUnhealty",
                options =>
                {
                    options.Configuration = new()
                    {
                        BootstrapServers = _database.BootstrapAddress,
                        SocketTimeoutMs = 0,
                    };
                    options.Mode = ProducerHandleMode.Create;
                    options.Topic = "TestContainerUnhealty";
                }
            );
        });

    [Fact]
    public async Task AddKafka_UseOptionsServiceProvider_ShouldReturnUnhealty() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKafka(
                    "TestContainerUnhealty",
                    options =>
                    {
                        options.Mode = ProducerHandleMode.ServiceProvider;
                        options.Topic = "TestContainerUnhealty";
                    }
                );
            },
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

    [Fact]
    public async Task AddKafka_UseOptions_ConfigurationNull_ShouldReturnUnhealty() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddKafka(
                "TestContainerUnhealty",
                options =>
                {
                    options.Configuration = null!;
                    options.Mode = ProducerHandleMode.Create;
                    options.Topic = "TestContainerUnhealty";
                }
            );
        });

    [Fact]
    public async Task AddKafka_UseOptions_BootstrapAddressNull_ShouldReturnUnhealty() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddKafka(
                "TestContainerUnhealty",
                options =>
                {
                    options.Configuration = new() { BootstrapServers = null! };
                    options.Mode = ProducerHandleMode.Create;
                    options.Topic = "TestContainerUnhealty";
                }
            );
        });

    [Fact]
    public async Task AddKafka_UseOptions_BootstrapAddressWhiteSpace_ShouldReturnUnhealty() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddKafka(
                "TestContainerUnhealty",
                options =>
                {
                    options.Configuration = new() { BootstrapServers = " " };
                    options.Mode = ProducerHandleMode.Create;
                    options.Topic = "TestContainerUnhealty";
                }
            );
        });

    [Fact]
    public async Task AddKafka_UseOptions_BootstrapAddressInvalid_ShoudlReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddKafka(
                "TestContainerUnhealty",
                options =>
                {
                    options.Configuration = new()
                    {
                        BootstrapServers = _database.BootstrapAddress,
                        EnableDeliveryReports = false,
                    };
                    options.Mode = ProducerHandleMode.Create;
                    options.Topic = "TestContainerUnhealty";
                }
            );
        });
}
