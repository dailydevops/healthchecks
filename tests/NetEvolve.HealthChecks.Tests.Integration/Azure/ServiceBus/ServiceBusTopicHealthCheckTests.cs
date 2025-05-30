namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ServiceBus;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
// [TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Topic")] - https://github.com/thomhurst/TUnit/issues/2481
[ClassDataSource<ServiceBusContainer>(Shared = SharedType.PerTestSession)]
public class ServiceBusTopicHealthCheckTests : HealthCheckTestBase
{
    private readonly ServiceBusContainer _container;

    public ServiceBusTopicHealthCheckTests(ServiceBusContainer container) => _container = container;

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusTopic(
                    "ServiceBusTopicServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = ServiceBusContainer.TopicName;
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
    public async Task AddAzureServiceBusTopic_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
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
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseOptions_ModeServiceProvider_TopicNotExists_ShouldReturnUnhealthy() =>
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
    public async Task AddAzureServiceBusTopic_UseOptions_ModeConnectionString_TopicNotExists_ShouldReturnUnhealthy() =>
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
    public async Task AddAzureServiceBusTopic_UseOptions_ModeServiceProvider_Timeout_ShouldReturnDegraded() =>
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
    public async Task AddAzureServiceBusTopic_UseOptions_ModeConnectionString_Timeout_ShouldReturnDegraded() =>
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
    public async Task AddAzureServiceBusTopic_UseOptions_ModeDefaultAzureCredentials_ShouldReturnUnhealthy() =>
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
    public async Task AddAzureServiceBusTopic_UseConfiguration_ShouldReturnHealthy() =>
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
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusTopic_UseConfiguration_TopicNotExists_ShouldReturnUnhealthy() =>
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
    public async Task AddAzureServiceBusTopic_UseConfiguration_ShouldReturnDegraded() =>
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
    public async Task AddAzureServiceBusTopic_UseConfiguration_DefaultAzureCredentials_ShouldReturnUnhealthy() =>
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
