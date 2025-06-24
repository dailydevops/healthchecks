namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Blobs;

using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Blobs;

[TestGroup($"{nameof(Azure)}.{nameof(Blobs)}")]
[ClassDataSource<AzuriteAccess>(Shared = InstanceSharedType.Azure)]
public class BlobContainerAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly AzuriteAccess _container;

    public BlobContainerAvailableHealthCheckTests(AzuriteAccess container) => _container = container;

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobServiceProviderHealthy",
                    options =>
                    {
                        options.ContainerName = "test";
                        options.Mode = BlobClientCreationMode.ServiceProvider;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobServiceProviderDegraded",
                    options =>
                    {
                        options.ContainerName = "test";
                        options.Mode = BlobClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeServiceProvider_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobServiceProviderNotExistsUnhealthy",
                    options =>
                    {
                        options.ContainerName = "notexists";
                        options.Mode = BlobClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobServiceProviderWithConfigurationHealthy",
                    options =>
                    {
                        options.ContainerName = "test";
                        options.Mode = BlobClientCreationMode.ServiceProvider;
                        options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobConnectionStringHealthy",
                    options =>
                    {
                        options.ContainerName = "test";
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = BlobClientCreationMode.ConnectionString;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeConnectionString_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobConnectionStringDegraded",
                    options =>
                    {
                        options.ContainerName = "test";
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = BlobClientCreationMode.ConnectionString;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeSharedKey_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobSharedKeyHealthy",
                    options =>
                    {
                        options.ContainerName = "test";
                        options.AccountKey = AzuriteAccess.AccountKey;
                        options.AccountName = AzuriteAccess.AccountName;
                        options.Mode = BlobClientCreationMode.SharedKey;
                        options.ServiceUri = _container.BlobServiceEndpoint;
                        options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeSharedKey_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobSharedKeyDegraded",
                    options =>
                    {
                        options.ContainerName = "test";
                        options.AccountKey = AzuriteAccess.AccountKey;
                        options.AccountName = AzuriteAccess.AccountName;
                        options.Mode = BlobClientCreationMode.SharedKey;
                        options.Timeout = 0;
                        options.ServiceUri = _container.BlobServiceEndpoint;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeAzureSasCredential_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobAzureSasCredentialHealthy",
                    options =>
                    {
                        options.ContainerName = "test";
                        options.Mode = BlobClientCreationMode.AzureSasCredential;
                        options.ServiceUri = _container.BlobAccountSasUri;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeAzureSasCredential_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobAzureSasCredentialDegraded",
                    options =>
                    {
                        options.ContainerName = "test";
                        options.Mode = BlobClientCreationMode.AzureSasCredential;
                        options.ServiceUri = _container.BlobAccountSasUri;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );
}
