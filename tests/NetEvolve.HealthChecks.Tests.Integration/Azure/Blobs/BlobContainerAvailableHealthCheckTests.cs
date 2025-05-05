namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Blobs;

using System;
using System.Threading.Tasks;
using global::Azure.Storage.Blobs;
using global::Azure.Storage.Sas;
using Microsoft.Extensions.Azure;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Blobs;

[TestGroup($"{nameof(Azure)}.{nameof(Blobs)}")]
public class BlobContainerAvailableHealthCheckTests : HealthCheckTestBase, IClassFixture<AzuriteAccess>
{
    private readonly AzuriteAccess _container;
    private readonly Uri _accountSasUri;
    private readonly Uri _uriBlobStorage;

    public BlobContainerAvailableHealthCheckTests(AzuriteAccess container)
    {
        _container = container;

        var client = new BlobServiceClient(_container.ConnectionString);
        _uriBlobStorage = client.Uri;

        _accountSasUri = client.GenerateAccountSasUri(
            AccountSasPermissions.All,
            DateTimeOffset.UtcNow.AddDays(1),
            AccountSasResourceTypes.All
        );

        var containerClient = client.GetBlobContainerClient("test");

        if (!containerClient.Exists())
        {
            _ = containerClient.Create();
        }
    }

    [Fact]
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
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
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
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
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
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
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
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
    public async Task AddBlobContainerAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
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
        });

    [Fact]
    public async Task AddBlobContainerAvailability_UseOptions_ModeConnectionString_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
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
        });

    [Fact]
    public async Task AddBlobContainerAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobContainerAvailability(
                "BlobSharedKeyHealthy",
                options =>
                {
                    options.ContainerName = "test";
                    options.AccountKey = AzuriteAccess.AccountKey;
                    options.AccountName = AzuriteAccess.AccountName;
                    options.Mode = BlobClientCreationMode.SharedKey;
                    options.ServiceUri = _uriBlobStorage;
                    options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                }
            );
        });

    [Fact]
    public async Task AddBlobContainerAvailability_UseOptions_ModeSharedKey_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
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
                    options.ServiceUri = _uriBlobStorage;
                }
            );
        });

    [Fact]
    public async Task AddBlobContainerAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobContainerAvailability(
                "BlobAzureSasCredentialHealthy",
                options =>
                {
                    options.ContainerName = "test";
                    options.Mode = BlobClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                }
            );
        });

    [Fact]
    public async Task AddBlobContainerAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobContainerAvailability(
                "BlobAzureSasCredentialDegraded",
                options =>
                {
                    options.ContainerName = "test";
                    options.Mode = BlobClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                    options.Timeout = 0;
                }
            );
        });
}
