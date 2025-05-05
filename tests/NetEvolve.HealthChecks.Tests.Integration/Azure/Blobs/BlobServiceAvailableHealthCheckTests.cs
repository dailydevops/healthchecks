namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Blobs;

using System;
using System.Threading.Tasks;
using global::Azure.Storage.Blobs;
using global::Azure.Storage.Sas;
using Microsoft.Extensions.Azure;
using NetEvolve.HealthChecks.Azure.Blobs;
using Xunit;

public class BlobServiceAvailableHealthCheckTests : HealthCheckTestBase, IClassFixture<AzuriteAccess>
{
    private readonly AzuriteAccess _container;
    private readonly Uri _accountSasUri;
    private readonly Uri _uriBlobStorage;

    public BlobServiceAvailableHealthCheckTests(AzuriteAccess container)
    {
        _container = container;

        var client = new BlobServiceClient(_container.ConnectionString);
        _uriBlobStorage = client.Uri;

        _accountSasUri = client.GenerateAccountSasUri(
            AccountSasPermissions.All,
            DateTimeOffset.UtcNow.AddDays(1),
            AccountSasResourceTypes.All
        );
    }

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceServiceProviderHealthy",
                    options => options.Mode = BlobClientCreationMode.ServiceProvider
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceServiceProviderWithConfigurationHealthy",
                    options =>
                    {
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
    public async Task AddBlobServiceAvailability_UseOptions_ModeServiceProvider_ShouldReturnDegraded() =>
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
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddBlobServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceConnectionStringHealthy",
                options =>
                {
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = BlobClientCreationMode.ConnectionString;
                }
            );
        });

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeConnectionString_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
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
        });

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceSharedKeyHealthy",
                options =>
                {
                    options.AccountKey = AzuriteAccess.AccountKey;
                    options.AccountName = AzuriteAccess.AccountName;
                    options.Mode = BlobClientCreationMode.SharedKey;
                    options.ServiceUri = _uriBlobStorage;
                    options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                }
            );
        });

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeSharedKey_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceSharedKeyDegraded",
                options =>
                {
                    options.AccountKey = AzuriteAccess.AccountKey;
                    options.AccountName = AzuriteAccess.AccountName;
                    options.Mode = BlobClientCreationMode.SharedKey;
                    options.Timeout = 0;
                    options.ServiceUri = _uriBlobStorage;
                }
            );
        });

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceAzureSasCredentialHealthy",
                options =>
                {
                    options.Mode = BlobClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                }
            );
        });

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceAzureSasCredentialDegraded",
                options =>
                {
                    options.Mode = BlobClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                    options.Timeout = 0;
                }
            );
        });
}
