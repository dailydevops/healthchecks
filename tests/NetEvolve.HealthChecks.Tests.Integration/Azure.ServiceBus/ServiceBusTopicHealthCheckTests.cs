namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ServiceBus;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Topic")]
[TestGroup("Z02TestGroup")]
[ClassDataSource<ServiceBusContainer>(Shared = InstanceSharedType.AzureServiceBus)]
public class ServiceBusTopicHealthCheckTests : HealthCheckTestBase
{
    private readonly ServiceBusContainer _container;

    public ServiceBusTopicHealthCheckTests(ServiceBusContainer container) => _container = container;

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusTopic(
                    "ServiceBusTopicServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddServiceBusAdministrationClient(_container.ConnectionString)
                );
            }
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseOptions_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusTopic(
                    "ServiceBusTopicKeyedServiceHealthy",
                    options =>
                    {
                        options.KeyedService = "test-key";
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddServiceBusAdministrationClient(_container.ConnectionString)
                );
                _ = services.AddKeyedSingleton(
                    "test-key",
                    (serviceProvider, _) => serviceProvider.GetRequiredService<ServiceBusAdministrationClient>()
                );
            }
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseOptions_ModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusTopic(
                    "ServiceBusTopicConnectionStringHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseOptions_ModeServiceProvider_TopicNotExists_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusTopic(
                    "ServiceBusTopicServiceProviderUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = "nonexistent-topic";
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddServiceBusAdministrationClient(_container.ConnectionString)
                );
            }
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseOptions_ModeConnectionString_TopicNotExists_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusTopic(
                    "ServiceBusTopicConnectionStringUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.TopicName = "nonexistent-topic";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseOptions_ModeServiceProvider_Timeout_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusTopic(
                    "ServiceBusTopicServiceProviderDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.Timeout = 0; // Set timeout to 0 to force timeout
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddServiceBusAdministrationClient(_container.ConnectionString)
                );
            }
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseOptions_ModeConnectionString_Timeout_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusTopic(
                    "ServiceBusTopicConnectionStringDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.Timeout = 0; // Set timeout to 0 to force timeout
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseOptions_ModeDefaultAzureCredentials_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusTopic(
                    "ServiceBusTopicDefaultAzureCredentialsUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.DefaultAzureCredentials;
                        options.FullyQualifiedNamespace = "namespace.servicebus.windows.net";
                        options.TopicName = ServiceBusContainer.TopicName;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    // Configuration-based tests

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureServiceBusTopic("ConfigurationHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationHealthy:ConnectionString",
                        _container.ConnectionString
                    },
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationHealthy:TopicName",
                        ServiceBusContainer.TopicName
                    },
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationHealthy:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                    { "HealthChecks:AzureServiceBusTopic:ConfigurationHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseConfiguration_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureServiceBusTopic("ConfigurationKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AzureServiceBusTopic:ConfigurationKeyedHealthy:KeyedService", "test-key" },
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationKeyedHealthy:Mode",
                        nameof(ClientCreationMode.ServiceProvider)
                    },
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationKeyedHealthy:TopicName",
                        ServiceBusContainer.TopicName
                    },
                    { "HealthChecks:AzureServiceBusTopic:ConfigurationKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddServiceBusAdministrationClient(_container.ConnectionString)
                );
                _ = services.AddKeyedSingleton(
                    "test-key",
                    (serviceProvider, _) => serviceProvider.GetRequiredService<ServiceBusAdministrationClient>()
                );
            }
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseConfiguration_TopicNotExists_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureServiceBusTopic("ConfigurationUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationUnhealthy:ConnectionString",
                        _container.ConnectionString
                    },
                    { "HealthChecks:AzureServiceBusTopic:ConfigurationUnhealthy:TopicName", "nonexistent-topic" },
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationUnhealthy:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureServiceBusTopic("ConfigurationDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationDegraded:ConnectionString",
                        _container.ConnectionString
                    },
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationDegraded:TopicName",
                        ServiceBusContainer.TopicName
                    },
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationDegraded:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                    { "HealthChecks:AzureServiceBusTopic:ConfigurationDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseConfiguration_DefaultAzureCredentials_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureServiceBusTopic("ConfigurationCredentialsUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationCredentialsUnhealthy:FullyQualifiedNamespace",
                        "namespace.servicebus.windows.net"
                    },
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationCredentialsUnhealthy:TopicName",
                        ServiceBusContainer.TopicName
                    },
                    {
                        "HealthChecks:AzureServiceBusTopic:ConfigurationCredentialsUnhealthy:Mode",
                        nameof(ClientCreationMode.DefaultAzureCredentials)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
