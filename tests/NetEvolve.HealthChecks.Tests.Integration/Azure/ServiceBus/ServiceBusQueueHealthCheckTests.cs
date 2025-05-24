namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ServiceBus;

using Microsoft.Extensions.Azure;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public class ServiceBusQueueHealthCheckTests : HealthCheckTestBase, IClassFixture<ServiceBusContainer>
{
    private readonly ServiceBusContainer _container;

    public ServiceBusQueueHealthCheckTests(ServiceBusContainer container) => _container = container;

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusQueueHealthCheck_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusQueueHealthCheck(
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
    public async Task AddServiceBusQueueHealthCheck_UseOptions_EnablePeekModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusQueueHealthCheck(
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
            {
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString));
            }
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusQueueHealthCheck_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddServiceBusQueueHealthCheck(
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
    public async Task AddServiceBusQueueHealthCheck_UseOptions_EnablePeekModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddServiceBusQueueHealthCheck(
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

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusQueueHealthCheck_UseOptions_ModeServiceProvider_QueueNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusQueueHealthCheck(
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
    public async Task AddServiceBusQueueHealthCheck_UseOptions_EnablePeekModeServiceProvider_QueueNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusQueueHealthCheck(
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
            {
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString));
            }
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusQueueHealthCheck_UseOptions_ModeConnectionString_QueueNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddServiceBusQueueHealthCheck(
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
    public async Task AddServiceBusQueueHealthCheck_UseOptions_EnablePeekModeConnectionString_QueueNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddServiceBusQueueHealthCheck(
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

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusQueueHealthCheck_UseOptions_ModeServiceProvider_Timeout_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusQueueHealthCheck(
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
    public async Task AddServiceBusQueueHealthCheck_UseOptions_EnablePeekModeServiceProvider_Timeout_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusQueueHealthCheck(
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
            {
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString));
            }
        );
}
