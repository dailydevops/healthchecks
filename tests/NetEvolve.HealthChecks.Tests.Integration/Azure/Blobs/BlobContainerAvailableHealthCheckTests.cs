namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Blobs;

using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Blobs;

[TestGroup($"{nameof(Azure)}.{nameof(Blobs)}")]
[ClassDataSource<AzuriteAccess>(Shared = SharedType.PerTestSession)]
public class BlobContainerAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly AzuriteAccess _container;

    public BlobContainerAvailableHealthCheckTests(AzuriteAccess container) => _container = container;

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobServiceProviderHealthy",
                    options =>
                    {
                        options.ContainerName = "test";
                        options.Mode = BlobClientCreationMode.ServiceProvider;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeServiceProvider_ShouldReturnDegraded() =>
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
    public async Task AddBlobContainerAvailability_UseOptions_ModeServiceProvider_ShouldReturnUnhealthy() =>
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
    public async Task AddBlobContainerAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_ShouldReturnHealthy() =>
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
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
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
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeConnectionString_ShouldReturnDegraded() =>
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
    public async Task AddBlobContainerAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
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
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeSharedKey_ShouldReturnDegraded() =>
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
    public async Task AddBlobContainerAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnHealthy() =>
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
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddBlobContainerAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnDegraded() =>
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
