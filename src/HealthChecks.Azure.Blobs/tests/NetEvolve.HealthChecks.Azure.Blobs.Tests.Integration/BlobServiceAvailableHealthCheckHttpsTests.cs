namespace NetEvolve.HealthChecks.Azure.Blobs.Tests.Integration;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using global::Azure.Storage.Blobs;
using global::Azure.Storage.Sas;
using Microsoft.Extensions.Azure;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Tests.Integration;
using NetEvolve.HealthChecks.Tests;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
[SetCulture("en-US")]
public class BlobServiceAvailableHealthCheckHttpsTests
    : HealthCheckTestBase,
        IClassFixture<AzuriteHttpsAccess>
{
    private readonly AzuriteHttpsAccess _container;
    private readonly Uri _accountSasUri;

    // TODO: Remove when TestContainers is providing this settíng
    private readonly Uri _uriBlobStorage;

    public BlobServiceAvailableHealthCheckHttpsTests(AzuriteHttpsAccess container)
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
                    "ServiceHttpsServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddBlobServiceClient(_container.ConnectionString)
                );
            }
        );

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeServiceProvider_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceHttpsServiceProviderDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddBlobServiceClient(_container.ConnectionString)
                );
            }
        );

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceHttpsServiceProviderWithConfigurationHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.ConfigureClientOptions = clientOptions =>
                        {
                            clientOptions.Retry.MaxRetries = 0;
                        };
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddBlobServiceClient(_container.ConnectionString)
                );
            }
        );

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceHttpsConnectionStringHealthy",
                options =>
                {
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = ClientCreationMode.ConnectionString;
                }
            );
        });

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeConnectionString_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceHttpsConnectionStringDegraded",
                options =>
                {
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.Timeout = 0;
                }
            );
        });

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceHttpsSharedKeyHealthy",
                options =>
                {
                    options.AccountKey = AzuriteHttpAccess.AccountKey;
                    options.AccountName = AzuriteHttpAccess.AccountName;
                    options.Mode = ClientCreationMode.SharedKey;
                    // TODO: Change when TestContainers is providing this settíng
                    // options.ServiceUri = _container.BlobEndpoint;
                    options.ServiceUri = _uriBlobStorage;
                    options.ConfigureClientOptions = clientOptions =>
                    {
                        clientOptions.Retry.MaxRetries = 0;
                    };
                }
            );
        });

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeSharedKey_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceHttpsSharedKeyDegraded",
                options =>
                {
                    options.AccountKey = AzuriteHttpAccess.AccountKey;
                    options.AccountName = AzuriteHttpAccess.AccountName;
                    options.Mode = ClientCreationMode.SharedKey;
                    options.Timeout = 0;
                    // TODO: Change when TestContainers is providing this settíng
                    // options.ServiceUri = _container.BlobEndpoint;
                    options.ServiceUri = _uriBlobStorage;
                }
            );
        });

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceHttpsAzureSasCredentialHealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                }
            );
        });

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceHttpsAzureSasCredentialDegraded",
                options =>
                {
                    options.Mode = ClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                    options.Timeout = 0;
                }
            );
        });
}
