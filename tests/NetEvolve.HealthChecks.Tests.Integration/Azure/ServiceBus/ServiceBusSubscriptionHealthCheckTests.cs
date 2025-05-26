namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ServiceBus;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public class ServiceBusSubscriptionHealthCheckTests : HealthCheckTestBase, IClassFixture<ServiceBusContainer>
{
    private readonly ServiceBusContainer _container;

    public ServiceBusSubscriptionHealthCheckTests(ServiceBusContainer container) => _container = container;

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusSubscription_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusSubscription(
                    "ServiceBusSubscriptionServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.SubscriptionName = ServiceBusContainer.SubscriptionName;
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
    public async Task AddAzureServiceBusSubscription_UseOptions_EnablePeekModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusSubscription(
                    "ServiceBusSubscriptionServiceProviderPeekHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.SubscriptionName = ServiceBusContainer.SubscriptionName;
                    }
                );
            },
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString))
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusSubscription_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddAzureServiceBusSubscription(
                "ServiceBusSubscriptionConnectionStringHealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.TopicName = ServiceBusContainer.TopicName;
                    options.SubscriptionName = ServiceBusContainer.SubscriptionName;
                }
            );
        });

    [Fact]
    public async Task AddAzureServiceBusSubscription_UseOptions_EnablePeekModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddAzureServiceBusSubscription(
                "ServiceBusSubscriptionConnectionStringPeekHealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.EnablePeekMode = true;
                    options.TopicName = ServiceBusContainer.TopicName;
                    options.SubscriptionName = ServiceBusContainer.SubscriptionName;
                }
            );
        });

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusSubscription_UseOptions_ModeServiceProvider_SubscriptionNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusSubscription(
                    "ServiceBusSubscriptionServiceProviderUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.SubscriptionName = "nonexistent-subscription";
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
    public async Task AddAzureServiceBusSubscription_UseOptions_EnablePeekModeServiceProvider_SubscriptionNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusSubscription(
                    "ServiceBusSubscriptionServiceProviderPeekUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.SubscriptionName = "nonexistent-subscription";
                    }
                );
            },
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString))
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusSubscription_UseOptions_ModeConnectionString_SubscriptionNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddAzureServiceBusSubscription(
                "ServiceBusSubscriptionConnectionStringUnhealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.TopicName = ServiceBusContainer.TopicName;
                    options.SubscriptionName = "nonexistent-subscription";
                }
            );
        });

    [Fact]
    public async Task AddAzureServiceBusSubscription_UseOptions_EnablePeekModeConnectionString_SubscriptionNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddAzureServiceBusSubscription(
                "ServiceBusSubscriptionConnectionStringPeekUnhealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.EnablePeekMode = true;
                    options.TopicName = ServiceBusContainer.TopicName;
                    options.SubscriptionName = "nonexistent-subscription";
                }
            );
        });

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusSubscription_UseOptions_ModeServiceProvider_TopicNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusSubscription(
                    "ServiceBusSubscriptionTopicNotExistsUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = "nonexistent-topic";
                        options.SubscriptionName = ServiceBusContainer.SubscriptionName;
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
    public async Task AddAzureServiceBusSubscription_UseOptions_EnablePeekModeServiceProvider_TopicNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusSubscription(
                    "ServiceBusSubscriptionTopicNotExistsPeekUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.TopicName = "nonexistent-topic";
                        options.SubscriptionName = ServiceBusContainer.SubscriptionName;
                    }
                );
            },
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString))
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusSubscription_UseOptions_ModeServiceProvider_Timeout_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusSubscription(
                    "ServiceBusSubscriptionServiceProviderDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.SubscriptionName = ServiceBusContainer.SubscriptionName;
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
    public async Task AddAzureServiceBusSubscription_UseOptions_EnablePeekModeServiceProvider_Timeout_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureServiceBusSubscription(
                    "ServiceBusSubscriptionServiceProviderPeekDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.SubscriptionName = ServiceBusContainer.SubscriptionName;
                        options.Timeout = 0; // Set timeout to 0 to force timeout
                    }
                );
            },
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString))
        );

    // Configuration-based tests

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusSubscription_UseConfiguration_EnablePeekMode_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddAzureServiceBusSubscription("ConfigurationHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationHealthy:ConnectionString",
                        _container.ConnectionString
                    },
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationHealthy:TopicName",
                        ServiceBusContainer.TopicName
                    },
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationHealthy:SubscriptionName",
                        ServiceBusContainer.SubscriptionName
                    },
                    { "HealthChecks:AzureServiceBusSubscription:ConfigurationHealthy:EnablePeekMode", "true" },
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationHealthy:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusSubscription_UseConfiguration_EnablePeekMode_SubscriptionNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddAzureServiceBusSubscription("ConfigurationUnhealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationUnhealthy:ConnectionString",
                        _container.ConnectionString
                    },
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationUnhealthy:TopicName",
                        ServiceBusContainer.TopicName
                    },
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationUnhealthy:SubscriptionName",
                        "nonexistent-subscription"
                    },
                    { "HealthChecks:AzureServiceBusSubscription:ConfigurationUnhealthy:EnablePeekMode", "true" },
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationUnhealthy:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddAzureServiceBusSubscription_UseConfiguration_EnablePeekMode_TopicNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddAzureServiceBusSubscription("ConfigurationUnhealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationUnhealthy:ConnectionString",
                        _container.ConnectionString
                    },
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationUnhealthy:TopicName",
                        "nonexistent-topic"
                    },
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationUnhealthy:SubscriptionName",
                        ServiceBusContainer.SubscriptionName
                    },
                    { "HealthChecks:AzureServiceBusSubscription:ConfigurationUnhealthy:EnablePeekMode", "true" },
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationUnhealthy:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddAzureServiceBusSubscription_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddAzureServiceBusSubscription("ConfigurationDegraded"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationDegraded:ConnectionString",
                        _container.ConnectionString
                    },
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationDegraded:TopicName",
                        ServiceBusContainer.TopicName
                    },
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationDegraded:SubscriptionName",
                        ServiceBusContainer.SubscriptionName
                    },
                    { "HealthChecks:AzureServiceBusSubscription:ConfigurationDegraded:EnablePeekMode", "true" },
                    {
                        "HealthChecks:AzureServiceBusSubscription:ConfigurationDegraded:Mode",
                        nameof(ClientCreationMode.ConnectionString)
                    },
                    { "HealthChecks:AzureServiceBusSubscription:ConfigurationDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
