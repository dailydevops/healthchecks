namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Blobs;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Blobs;

[TestGroup($"{nameof(Azure)}.{nameof(Blobs)}")]
[ClassDataSource<AzuriteAccess>(Shared = SharedType.PerClass)]
[TestGroup("Z02TestGroup")]
public class BlobServiceAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly AzuriteAccess _container;

    public BlobServiceAvailableHealthCheckTests(AzuriteAccess container) => _container = container;

    [Test]
    public async Task AddBlobServiceAvailability_UseOptions_ModeServiceProvider_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = BlobClientCreationMode.ServiceProvider;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString)),
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddBlobServiceAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceServiceProviderWithConfigurationHealthy",
                    options =>
                    {
                        options.Mode = BlobClientCreationMode.ServiceProvider;
                        options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString)),
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddBlobServiceAvailability_UseOptions_ModeServiceProvider_Degraded(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceServiceProviderDegraded",
                    options =>
                    {
                        options.Mode = BlobClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString)),
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddBlobServiceAvailability_UseOptions_ModeConnectionString_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceConnectionStringHealthy",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = BlobClientCreationMode.ConnectionString;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddBlobServiceAvailability_UseOptions_ModeConnectionString_Degraded(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceConnectionStringDegraded",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = BlobClientCreationMode.ConnectionString;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddBlobServiceAvailability_UseOptions_ModeSharedKey_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceSharedKeyHealthy",
                    options =>
                    {
                        options.AccountKey = AzuriteAccess.AccountKey;
                        options.AccountName = AzuriteAccess.AccountName;
                        options.Mode = BlobClientCreationMode.SharedKey;
                        options.ServiceUri = _container.BlobServiceEndpoint;
                        options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddBlobServiceAvailability_UseOptions_ModeSharedKey_Degraded(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceSharedKeyDegraded",
                    options =>
                    {
                        options.AccountKey = AzuriteAccess.AccountKey;
                        options.AccountName = AzuriteAccess.AccountName;
                        options.Mode = BlobClientCreationMode.SharedKey;
                        options.Timeout = 0;
                        options.ServiceUri = _container.BlobServiceEndpoint;
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddBlobServiceAvailability_UseOptions_ModeAzureSasCredential_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceAzureSasCredentialHealthy",
                    options =>
                    {
                        options.Mode = BlobClientCreationMode.AzureSasCredential;
                        options.ServiceUri = _container.BlobAccountSasUri;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddBlobServiceAvailability_UseOptions_ModeAzureSasCredential_Degraded(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceAzureSasCredentialDegraded",
                    options =>
                    {
                        options.Mode = BlobClientCreationMode.AzureSasCredential;
                        options.ServiceUri = _container.BlobAccountSasUri;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );
}
