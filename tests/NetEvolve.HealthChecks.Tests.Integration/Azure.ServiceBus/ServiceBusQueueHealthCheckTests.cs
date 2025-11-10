namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ServiceBus;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Azure.Messaging.ServiceBus;
using global::Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Queue")]
[ClassDataSource<ServiceBusContainer>(Shared = InstanceSharedType.AzureServiceBus)]
public class ServiceBusQueueHealthCheckTests : HealthCheckTestBase
{
    private readonly ServiceBusContainer _container;

    public ServiceBusQueueHealthCheckTests(ServiceBusContainer container) => _container = container;

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusQueue_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.QueueName = ServiceBusContainer.QueueName;
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
    public async Task AddAzureServiceBusQueue_UseOptions_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueKeyedServiceHealthy",
                    options =>
                    {
                        options.KeyedService = "test-key";
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.QueueName = ServiceBusContainer.QueueName;
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

    [Test]
    public async Task AddAzureServiceBusQueue_UseOptions_EnablePeekModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueServiceProviderPeekHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.QueueName = ServiceBusContainer.QueueName;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddAzureServiceBusQueue_UseOptions_EnablePeekModeKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueKeyedServicePeekHealthy",
                    options =>
                    {
                        options.KeyedService = "test-key";
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.QueueName = ServiceBusContainer.QueueName;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString));
                _ = services.AddKeyedSingleton(
                    "test-key",
                    (serviceProvider, _) => serviceProvider.GetRequiredService<ServiceBusClient>()
                );
            }
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusQueue_UseOptions_ModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueConnectionStringHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.QueueName = ServiceBusContainer.QueueName;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddAzureServiceBusQueue_UseOptions_EnablePeekModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueConnectionStringPeekHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.EnablePeekMode = true;
                        options.QueueName = ServiceBusContainer.QueueName;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusQueue_UseOptions_ModeServiceProvider_QueueNotExists_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueServiceProviderUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.QueueName = "nonexistent-queue";
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

    [Test]
    public async Task AddAzureServiceBusQueue_UseOptions_EnablePeekModeServiceProvider_QueueNotExists_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueServiceProviderPeekUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.QueueName = "nonexistent-queue";
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString))
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusQueue_UseOptions_ModeConnectionString_QueueNotExists_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueConnectionStringUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.QueueName = "nonexistent-queue";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddAzureServiceBusQueue_UseOptions_EnablePeekModeConnectionString_QueueNotExists_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueConnectionStringPeekUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.EnablePeekMode = true;
                        options.QueueName = "nonexistent-queue";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test, Skip("Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusQueue_UseOptions_ModeServiceProvider_Timeout_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueServiceProviderDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.QueueName = ServiceBusContainer.QueueName;
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

    [Test]
    public async Task AddAzureServiceBusQueue_UseOptions_EnablePeekModeServiceProvider_Timeout_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueServiceProviderPeekDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.QueueName = ServiceBusContainer.QueueName;
                        options.Timeout = 0; // Set timeout to 0 to force timeout
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString))
        );

    // Configuration-based tests

    [Test]
    public async Task AddAzureServiceBusQueue_UseConfiguration_EnablePeekMode_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureServiceBusQueue("ConfigurationHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureServiceBusQueue:ConfigurationHealthy:ConnectionString",
                        _container.ConnectionString
                    },
                    {
                        "HealthChecks:AzureServiceBusQueue:ConfigurationHealthy:QueueName",
                        ServiceBusContainer.QueueName
                    },
                    { "HealthChecks:AzureServiceBusQueue:ConfigurationHealthy:EnablePeekMode", "true" },
                    {
                        "HealthChecks:AzureServiceBusQueue:ConfigurationHealthy:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                    { "HealthChecks:AzureServiceBusQueue:ConfigurationHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddAzureServiceBusQueue_UseConfiguration_EnablePeekModeKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureServiceBusQueue("ConfigurationKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AzureServiceBusQueue:ConfigurationKeyedHealthy:KeyedService", "test-key" },
                    {
                        "HealthChecks:AzureServiceBusQueue:ConfigurationKeyedHealthy:Mode",
                        nameof(ClientCreationMode.ServiceProvider)
                    },
                    { "HealthChecks:AzureServiceBusQueue:ConfigurationKeyedHealthy:EnablePeekMode", "true" },
                    {
                        "HealthChecks:AzureServiceBusQueue:ConfigurationKeyedHealthy:QueueName",
                        ServiceBusContainer.QueueName
                    },
                    { "HealthChecks:AzureServiceBusQueue:ConfigurationKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString));
                _ = services.AddKeyedSingleton(
                    "test-key",
                    (serviceProvider, _) => serviceProvider.GetRequiredService<ServiceBusClient>()
                );
            }
        );

    [Test]
    public async Task AddAzureServiceBusQueue_UseConfiguration_EnablePeekMode_QueueNotExists_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureServiceBusQueue("ConfigurationUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureServiceBusQueue:ConfigurationUnhealthy:ConnectionString",
                        _container.ConnectionString
                    },
                    { "HealthChecks:AzureServiceBusQueue:ConfigurationUnhealthy:QueueName", "nonexistent-queue" },
                    { "HealthChecks:AzureServiceBusQueue:ConfigurationUnhealthy:EnablePeekMode", "true" },
                    {
                        "HealthChecks:AzureServiceBusQueue:ConfigurationUnhealthy:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddAzureServiceBusQueue_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureServiceBusQueue("ConfigurationDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureServiceBusQueue:ConfigurationDegraded:ConnectionString",
                        _container.ConnectionString
                    },
                    {
                        "HealthChecks:AzureServiceBusQueue:ConfigurationDegraded:QueueName",
                        ServiceBusContainer.QueueName
                    },
                    { "HealthChecks:AzureServiceBusQueue:ConfigurationDegraded:EnablePeekMode", "true" },
                    {
                        "HealthChecks:AzureServiceBusQueue:ConfigurationDegraded:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                    { "HealthChecks:AzureServiceBusQueue:ConfigurationDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
