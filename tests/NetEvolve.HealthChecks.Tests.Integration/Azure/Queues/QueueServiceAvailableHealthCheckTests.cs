namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Queues;

using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Queues;

[TestGroup($"{nameof(Azure)}.{nameof(Queues)}")]
[ClassDataSource<AzuriteAccess>(Shared = SharedType.PerTestSession)]
public class QueueServiceAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly AzuriteAccess _container;

    public QueueServiceAvailableHealthCheckTests(AzuriteAccess container) => _container = container;

    [Test]
    public async Task AddQueueServiceAvailability_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "ServiceServiceProviderHealthy",
                    options => options.Mode = QueueClientCreationMode.ServiceProvider
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddQueueServiceAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_ShouldReturnHealthy() =>
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
    public async Task AddQueueServiceAvailability_UseOptions_ModeServiceProvider_ShouldReturnDegraded() =>
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
    public async Task AddQueueServiceAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "ServiceConnectionStringHealthy",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = QueueClientCreationMode.ConnectionString;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddQueueServiceAvailability_UseOptions_ModeConnectionString_ShouldReturnDegraded() =>
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
    public async Task AddQueueServiceAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
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
                        options.Timeout = 1000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddQueueServiceAvailability_UseOptions_ModeSharedKey_ShouldReturnDegraded() =>
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
    public async Task AddQueueServiceAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueServiceAvailability(
                    "ServiceAzureSasCredentialHealthy",
                    options =>
                    {
                        options.Mode = QueueClientCreationMode.AzureSasCredential;
                        options.ServiceUri = _container.QueueAccountSasUri;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddQueueServiceAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnDegraded() =>
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
}
