namespace NetEvolve.HealthChecks.Tests.Integration.Azure.EventHubs;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.EventHubs;

[TestGroup($"{nameof(Azure)}.{nameof(EventHubs)}")]
[ClassDataSource<EventHubsContainer>(Shared = InstanceSharedType.AzureEventHubs)]
public class EventHubHealthCheckTests : HealthCheckTestBase
{
    private readonly EventHubsContainer _container;

    public EventHubHealthCheckTests(EventHubsContainer container) => _container = container;

    [Test, Skip("No EventHubs emulator available. Requires actual Azure EventHubs connection string.")]
    public async Task AddAzureEventHub_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHub(
                    "EventHubServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EventHubName = EventHubsContainer.EventHubName;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddEventHubProducerClient(_container.ConnectionString, EventHubsContainer.EventHubName)
                );
            }
        );

    [Test, Skip("No EventHubs emulator available. Requires actual Azure EventHubs connection string.")]
    public async Task AddAzureEventHub_UseOptions_ModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHub(
                    "EventHubConnectionStringHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.EventHubName = EventHubsContainer.EventHubName;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test, Skip("No EventHubs emulator available. Requires actual Azure EventHubs connection string.")]
    public async Task AddAzureEventHub_UseOptions_ModeServiceProvider_EventHubNotExists_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHub(
                    "EventHubServiceProviderUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EventHubName = "nonexistent-eventhub";
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddEventHubProducerClient(_container.ConnectionString, "nonexistent-eventhub")
                );
            }
        );

    [Test, Skip("No EventHubs emulator available. Requires actual Azure EventHubs connection string.")]
    public async Task AddAzureEventHub_UseOptions_ModeConnectionString_EventHubNotExists_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHub(
                    "EventHubConnectionStringUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.EventHubName = "nonexistent-eventhub";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test, Skip("No EventHubs emulator available. Requires actual Azure EventHubs connection string.")]
    public async Task AddAzureEventHub_UseOptions_ModeServiceProvider_Timeout_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHub(
                    "EventHubServiceProviderDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EventHubName = EventHubsContainer.EventHubName;
                        options.Timeout = 0; // Set timeout to 0 to force timeout
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddEventHubProducerClient(_container.ConnectionString, EventHubsContainer.EventHubName)
                );
            }
        );

    [Test, Skip("No EventHubs emulator available. Requires actual Azure EventHubs connection string.")]
    public async Task AddAzureEventHub_UseOptions_ModeConnectionString_Timeout_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHub(
                    "EventHubConnectionStringDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.EventHubName = EventHubsContainer.EventHubName;
                        options.Timeout = 0; // Set timeout to 0 to force timeout
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test, Skip("No EventHubs emulator available. Requires actual Azure EventHubs connection string.")]
    public async Task AddAzureEventHub_UseOptions_ModeDefaultAzureCredentials_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHub(
                    "EventHubDefaultAzureCredentialsUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.DefaultAzureCredentials;
                        options.FullyQualifiedNamespace = "namespace.servicebus.windows.net";
                        options.EventHubName = EventHubsContainer.EventHubName;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    // Configuration-based tests

    [Test, Skip("No EventHubs emulator available. Requires actual Azure EventHubs connection string.")]
    public async Task AddAzureEventHub_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureEventHub("ConfigurationHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureEventHub:ConfigurationHealthy:ConnectionString",
                        _container.ConnectionString
                    },
                    {
                        "HealthChecks:AzureEventHub:ConfigurationHealthy:EventHubName",
                        EventHubsContainer.EventHubName
                    },
                    {
                        "HealthChecks:AzureEventHub:ConfigurationHealthy:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                    { "HealthChecks:AzureEventHub:ConfigurationHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test, Skip("No EventHubs emulator available. Requires actual Azure EventHubs connection string.")]
    public async Task AddAzureEventHub_UseConfiguration_EventHubNotExists_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureEventHub("ConfigurationUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureEventHub:ConfigurationUnhealthy:ConnectionString",
                        _container.ConnectionString
                    },
                    { "HealthChecks:AzureEventHub:ConfigurationUnhealthy:EventHubName", "nonexistent-eventhub" },
                    {
                        "HealthChecks:AzureEventHub:ConfigurationUnhealthy:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test, Skip("No EventHubs emulator available. Requires actual Azure EventHubs connection string.")]
    public async Task AddAzureEventHub_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureEventHub("ConfigurationDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureEventHub:ConfigurationDegraded:ConnectionString",
                        _container.ConnectionString
                    },
                    {
                        "HealthChecks:AzureEventHub:ConfigurationDegraded:EventHubName",
                        EventHubsContainer.EventHubName
                    },
                    {
                        "HealthChecks:AzureEventHub:ConfigurationDegraded:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                    { "HealthChecks:AzureEventHub:ConfigurationDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test, Skip("No EventHubs emulator available. Requires actual Azure EventHubs connection string.")]
    public async Task AddAzureEventHub_UseConfiguration_DefaultAzureCredentials_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureEventHub("ConfigurationCredentialsUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureEventHub:ConfigurationCredentialsUnhealthy:FullyQualifiedNamespace",
                        "namespace.servicebus.windows.net"
                    },
                    {
                        "HealthChecks:AzureEventHub:ConfigurationCredentialsUnhealthy:EventHubName",
                        EventHubsContainer.EventHubName
                    },
                    {
                        "HealthChecks:AzureEventHub:ConfigurationCredentialsUnhealthy:Mode",
                        nameof(ClientCreationMode.DefaultAzureCredentials)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}