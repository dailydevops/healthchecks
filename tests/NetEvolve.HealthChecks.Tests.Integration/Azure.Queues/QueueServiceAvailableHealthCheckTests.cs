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
public class QueueServiceAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly AzuriteAccess _container;

    public QueueServiceAvailableHealthCheckTests(AzuriteAccess container) => _container = container;

    [Test]
    public async Task AddQueueServiceAvailability_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "ServiceServiceProviderHealthy",
                    options =>
                    {
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
    public async Task AddQueueServiceAvailability_UseOptions_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "KeyedServiceServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = QueueClientCreationMode.ServiceProvider;
                        options.KeyedService = "azure-queue-service-test";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString));
                _ = services.AddKeyedSingleton(
                    "azure-queue-service-test",
                    (serviceProvider, _) => serviceProvider.GetRequiredService<QueueServiceClient>()
                );
            }
        );

    [Test]
    public async Task AddQueueServiceAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "ServiceServiceProviderWithConfigurationHealthy",
                    options =>
                    {
                        options.Mode = QueueClientCreationMode.ServiceProvider;
                        options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddQueueServiceAvailability_UseOptions_ModeServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "ServiceServiceProviderDegraded",
                    options =>
                    {
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
    public async Task AddQueueServiceAvailability_UseOptions_ModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "ServiceConnectionStringHealthy",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = QueueClientCreationMode.ConnectionString;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddQueueServiceAvailability_UseOptions_ModeConnectionString_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "ServiceConnectionStringDegraded",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = QueueClientCreationMode.ConnectionString;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddQueueServiceAvailability_UseOptions_ModeSharedKey_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "ServiceSharedKeyHealthy",
                    options =>
                    {
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
    public async Task AddQueueServiceAvailability_UseOptions_ModeSharedKey_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "ServiceSharedKeyDegraded",
                    options =>
                    {
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
    public async Task AddQueueServiceAvailability_UseOptions_ModeAzureSasCredential_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "ServiceAzureSasCredentialHealthy",
                    options =>
                    {
                        options.Mode = QueueClientCreationMode.AzureSasCredential;
                        options.ServiceUri = _container.QueueAccountSasUri;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddQueueServiceAvailability_UseOptions_ModeAzureSasCredential_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "ServiceAzureSasCredentialDegraded",
                    options =>
                    {
                        options.Mode = QueueClientCreationMode.AzureSasCredential;
                        options.ServiceUri = _container.QueueAccountSasUri;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddQueueServiceAvailability_UseConfiguration_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddQueueServiceAvailability("ServiceServiceProviderHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureQueueService:ServiceServiceProviderHealthy:Mode",
                        nameof(QueueClientCreationMode.ServiceProvider)
                    },
                    { "HealthChecks:AzureQueueService:ServiceServiceProviderHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddQueueServiceAvailability_UseConfiguration_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddQueueServiceAvailability("KeyedServiceServiceProviderHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureQueueService:KeyedServiceServiceProviderHealthy:Mode",
                        nameof(QueueClientCreationMode.ServiceProvider)
                    },
                    {
                        "HealthChecks:AzureQueueService:KeyedServiceServiceProviderHealthy:KeyedService",
                        "azure-queue-service-test"
                    },
                    { "HealthChecks:AzureQueueService:KeyedServiceServiceProviderHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString));
                _ = services.AddKeyedSingleton(
                    "azure-queue-service-test",
                    (serviceProvider, _) => serviceProvider.GetRequiredService<QueueServiceClient>()
                );
            }
        );
}
