namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ServiceBus;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Queue")]
[Collection($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public class ServiceBusQueueHealthCheckTests : HealthCheckTestBase
{
    private readonly ServiceBusContainer _container;

    public ServiceBusQueueHealthCheckTests(ServiceBusContainer container) => _container = container;

    [NotExecutableFact(
        "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17"
    )]
    public async Task AddAzureServiceBusQueue_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.QueueName = ServiceBusContainer.QueueName;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddServiceBusAdministrationClient(_container.ConnectionString)
                );
            }
        );

    [Fact]
    public async Task AddAzureServiceBusQueue_UseOptions_EnablePeekModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusQueue(
                    "ServiceBusQueueServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.QueueName = ServiceBusContainer.QueueName;
                    }
                );
            },
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString))
        );

    [NotExecutableFact(
        "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17"
    )]
    public async Task AddAzureServiceBusQueue_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddAzureServiceBusQueue(
                "ServiceBusQueueConnectionStringHealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.QueueName = ServiceBusContainer.QueueName;
                }
            );
        });

    [Fact]
    public async Task AddAzureServiceBusQueue_UseOptions_EnablePeekModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddAzureServiceBusQueue(
                "ServiceBusQueueConnectionStringPeekHealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.EnablePeekMode = true;
                    options.QueueName = ServiceBusContainer.QueueName;
                    options.Timeout = Timeout.Infinite;
                }
            );
        });

    [NotExecutableFact(
        "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17"
    )]
    public async Task AddAzureServiceBusQueue_UseOptions_ModeServiceProvider_QueueNotExists_ShouldReturnUnhealthy() =>
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
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddServiceBusAdministrationClient(_container.ConnectionString)
                );
            }
        );

    [Fact]
    public async Task AddAzureServiceBusQueue_UseOptions_EnablePeekModeServiceProvider_QueueNotExists_ShouldReturnUnhealthy() =>
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
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString))
        );

    [NotExecutableFact(
        "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17"
    )]
    public async Task AddAzureServiceBusQueue_UseOptions_ModeConnectionString_QueueNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
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
        });

    [Fact]
    public async Task AddAzureServiceBusQueue_UseOptions_EnablePeekModeConnectionString_QueueNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
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
        });

    [NotExecutableFact(
        "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17"
    )]
    public async Task AddAzureServiceBusQueue_UseOptions_ModeServiceProvider_Timeout_ShouldReturnDegraded() =>
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
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddServiceBusAdministrationClient(_container.ConnectionString)
                );
            }
        );

    [Fact]
    public async Task AddAzureServiceBusQueue_UseOptions_EnablePeekModeServiceProvider_Timeout_ShouldReturnDegraded() =>
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
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString))
        );

    // Configuration-based tests

    [Fact]
    public async Task AddAzureServiceBusQueue_UseConfiguration_EnablePeekMode_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureServiceBusQueue("ConfigurationHealthy"),
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
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddAzureServiceBusQueue_UseConfiguration_EnablePeekMode_QueueNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureServiceBusQueue("ConfigurationUnhealthy"),
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

    [Fact]
    public async Task AddAzureServiceBusQueue_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAzureServiceBusQueue("ConfigurationDegraded"),
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
