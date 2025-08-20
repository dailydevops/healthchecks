namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Queues;

using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Queues;

[TestGroup($"{nameof(Azure)}.{nameof(Queues)}")]
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
}
