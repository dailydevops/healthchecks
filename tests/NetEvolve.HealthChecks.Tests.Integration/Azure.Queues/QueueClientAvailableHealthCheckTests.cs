namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Queues;

using System.Threading.Tasks;
using global::Azure.Storage.Queues;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Queues;

[TestGroup($"{nameof(Azure)}.{nameof(Queues)}")]
[TestGroup("Z02TestGroup")]
[ClassDataSource<AzuriteAccess>(Shared = InstanceSharedType.Azure)]
public class QueueClientAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly AzuriteAccess _container;

    public QueueClientAvailableHealthCheckTests(AzuriteAccess container) => _container = container;

    [Test]
    public async Task AddQueueClientAvailability_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueServiceProviderHealthy",
                    options =>
                    {
                        options.QueueName = "test";
                        options.Mode = QueueClientCreationMode.ServiceProvider;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddQueueClientAvailability_UseOptions_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "KeyedQueueServiceProviderHealthy",
                    options =>
                    {
                        options.QueueName = "test";
                        options.Mode = QueueClientCreationMode.ServiceProvider;
                        options.KeyedService = "azure-queue-client-test";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString));
                _ = services.AddKeyedSingleton(
                    "azure-queue-client-test",
                    (serviceProvider, _) => serviceProvider.GetRequiredService<QueueServiceClient>()
                );
            }
        );

    [Test]
    public async Task AddQueueClientAvailability_UseOptions_ModeServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueServiceProviderDegraded",
                    options =>
                    {
                        options.QueueName = "test";
                        options.Mode = QueueClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddQueueClientAvailability_UseOptions_ModeServiceProvider_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueServiceProviderNotExistsUnhealthy",
                    options =>
                    {
                        options.QueueName = "notexists";
                        options.Mode = QueueClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddQueueClientAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueServiceProviderWithConfigurationHealthy",
                    options =>
                    {
                        options.QueueName = "test";
                        options.Mode = QueueClientCreationMode.ServiceProvider;
                        options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddQueueClientAvailability_UseOptions_ModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueConnectionStringHealthy",
                    options =>
                    {
                        options.QueueName = "test";
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = QueueClientCreationMode.ConnectionString;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddQueueClientAvailability_UseOptions_ModeConnectionString_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueConnectionStringDegraded",
                    options =>
                    {
                        options.QueueName = "test";
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = QueueClientCreationMode.ConnectionString;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddQueueClientAvailability_UseOptions_ModeSharedKey_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueSharedKeyHealthy",
                    options =>
                    {
                        options.QueueName = "test";
                        options.AccountKey = AzuriteAccess.AccountKey;
                        options.AccountName = AzuriteAccess.AccountName;
                        options.Mode = QueueClientCreationMode.SharedKey;
                        options.ServiceUri = _container.QueueServiceEndpoint;
                        options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddQueueClientAvailability_UseOptions_ModeSharedKey_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueSharedKeyDegraded",
                    options =>
                    {
                        options.QueueName = "test";
                        options.AccountKey = AzuriteAccess.AccountKey;
                        options.AccountName = AzuriteAccess.AccountName;
                        options.Mode = QueueClientCreationMode.SharedKey;
                        options.Timeout = 0;
                        options.ServiceUri = _container.QueueServiceEndpoint;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddQueueClientAvailability_UseOptions_ModeAzureSasCredential_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueAzureSasCredentialHealthy",
                    options =>
                    {
                        options.QueueName = "test";
                        options.Mode = QueueClientCreationMode.AzureSasCredential;
                        options.ServiceUri = _container.QueueAccountSasUri;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddQueueClientAvailability_UseOptions_ModeAzureSasCredential_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueAzureSasCredentialDegraded",
                    options =>
                    {
                        options.QueueName = "test";
                        options.Mode = QueueClientCreationMode.AzureSasCredential;
                        options.ServiceUri = _container.QueueAccountSasUri;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddQueueClientAvailability_UseConfiguration_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddQueueClientAvailability("QueueServiceProviderHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AzureQueueClient:QueueServiceProviderHealthy:QueueName", "test" },
                    {
                        "HealthChecks:AzureQueueClient:QueueServiceProviderHealthy:Mode",
                        nameof(QueueClientCreationMode.ServiceProvider)
                    },
                    { "HealthChecks:AzureQueueClient:QueueServiceProviderHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddQueueClientAvailability_UseConfiguration_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddQueueClientAvailability("KeyedQueueServiceProviderHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AzureQueueClient:KeyedQueueServiceProviderHealthy:QueueName", "test" },
                    {
                        "HealthChecks:AzureQueueClient:KeyedQueueServiceProviderHealthy:Mode",
                        nameof(QueueClientCreationMode.ServiceProvider)
                    },
                    {
                        "HealthChecks:AzureQueueClient:KeyedQueueServiceProviderHealthy:KeyedService",
                        "azure-queue-client-test"
                    },
                    { "HealthChecks:AzureQueueClient:KeyedQueueServiceProviderHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString));
                _ = services.AddKeyedSingleton(
                    "azure-queue-client-test",
                    (serviceProvider, _) => serviceProvider.GetRequiredService<QueueServiceClient>()
                );
            }
        );
}
