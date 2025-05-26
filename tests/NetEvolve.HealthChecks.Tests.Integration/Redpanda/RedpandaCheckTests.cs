namespace NetEvolve.HealthChecks.Tests.Integration.Redpanda;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Redpanda;
using Xunit;

[TestGroup(nameof(Redpanda))]
public class RedpandaCheckTests : HealthCheckTestBase, IClassFixture<RedpandaDatabase>
{
    private readonly RedpandaDatabase _database;

    public RedpandaCheckTests(RedpandaDatabase database) => _database = database;

    [Fact]
    public async Task AddRedpanda_UseOptionsCreate_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddRedpanda(
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
    public async Task AddRedpanda_UseOptionsServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
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
    public async Task AddRedpanda_UseConfigurationCreate_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedpanda("TestContainerHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:RedPanda:TestContainerHealthy:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Timeout", "5000" },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Topic", "TestContainerHealthy" },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddRedpanda_UseConfigurationServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedpanda("TestContainerHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:RedPanda:TestContainerHealthy:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Timeout", "5000" },
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
            }
        );

    [Fact]
    public async Task AddRedpanda_UseOptionsCreate_EnableDeliveryReportsFalse_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
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
                    options.Timeout = 5000;
                    options.Topic = "TestContainerHealthy";
                }
            );
        });

    [Fact]
    public async Task AddRedpanda_UseConfigurationCreate_EnableDeliveryReportsFalse_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedpanda("TestContainerHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:RedPanda:TestContainerHealthy:Configuration:BootstrapServers",
                        _database.BootstrapAddress
                    },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Configuration:EnableDeliveryReports", "false" },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Timeout", "5000" },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Topic", "TestContainerHealthy" },
                    { "HealthChecks:RedPanda:TestContainerHealthy:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddRedpanda_UseOptionsCreate_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
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
        });

    [Fact]
    public async Task AddRedpanda_UseOptionsServiceProvider_ShouldReturnDegraded() =>
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
    public async Task AddRedpanda_UseConfigurationCreate_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedpanda("TestContainerDegraded"),
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
            }
        );

    [Fact]
    public async Task AddRedpanda_UseConfigurationServiceProvider_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedpanda("TestContainerDegraded"),
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
            }
        );

    [Fact]
    public async Task AddRedpanda_UseOptionsCreate_EnableDeliveryReportsFalse_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
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
        });

    [Fact]
    public async Task AddRedpanda_UseConfigurationCreate_EnableDeliveryReportsFalse_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedpanda("TestContainerDegraded"),
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
            }
        );

    [Fact]
    public async Task AddRedpanda_UseOptionsCreate_ShouldReturnUnhealty() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddRedpanda(
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
    public async Task AddRedpanda_UseOptionsServiceProvider_ShouldReturnUnhealty() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedpanda(
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
    public async Task AddRedpanda_UseOptions_ConfigurationNull_ShouldReturnUnhealty() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddRedpanda(
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
    public async Task AddRedpanda_UseOptions_BootstrapAddressNull_ShouldReturnUnhealty() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddRedpanda(
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
    public async Task AddRedpanda_UseOptions_BootstrapAddressWhiteSpace_ShouldReturnUnhealty() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddRedpanda(
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
    public async Task AddRedpanda_UseOptions_BootstrapAddressInvalid_ShoudlReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddRedpanda(
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
