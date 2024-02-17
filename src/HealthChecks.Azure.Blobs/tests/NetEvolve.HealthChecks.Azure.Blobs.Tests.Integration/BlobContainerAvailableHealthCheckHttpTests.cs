namespace NetEvolve.HealthChecks.Azure.Blobs.Tests.Integration;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using global::Azure.Storage.Blobs;
using global::Azure.Storage.Sas;
using Microsoft.Extensions.Azure;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Blobs;
using NetEvolve.HealthChecks.Azure.Tests.Integration;
using NetEvolve.HealthChecks.Tests;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
[SetCulture("en-US")]
public class BlobContainerAvailableHealthCheckHttpTests
    : HealthCheckTestBase,
        IClassFixture<AzuriteHttpAccess>
{
    private readonly AzuriteHttpAccess _container;
    private readonly Uri _accountSasUri;

    public BlobContainerAvailableHealthCheckHttpTests(AzuriteHttpAccess container)
    {
        _container = container;

        var client = new BlobServiceClient(_container.ConnectionString);

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
                    "BlobHttpServiceProviderHealthy",
                    options =>
                    {
                        options.ContainerName = "test";
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
    public async Task AddBlobContainerAvailability_UseOptions_ModeServiceProvider_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobHttpServiceProviderDegraded",
                    options =>
                    {
                        options.ContainerName = "test";
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
    public async Task AddBlobContainerAvailability_UseOptions_ModeServiceProvider_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobHttpServiceProviderNotExistsUnhealthy",
                    options =>
                    {
                        options.ContainerName = "notexists";
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
    public async Task AddBlobContainerAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobHttpServiceProviderWithConfigurationHealthy",
                    options =>
                    {
                        options.ContainerName = "test";
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
    public async Task AddBlobContainerAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobContainerAvailability(
                "BlobHttpConnectionStringHealthy",
                options =>
                {
                    options.ContainerName = "test";
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = ClientCreationMode.ConnectionString;
                }
            );
        });

    [Fact]
    public async Task AddBlobContainerAvailability_UseOptions_ModeConnectionString_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobContainerAvailability(
                "BlobHttpConnectionStringDegraded",
                options =>
                {
                    options.ContainerName = "test";
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.Timeout = 0;
                }
            );
        });

    //[Fact]
    //public async Task AddBlobContainerAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
    //    await RunAndVerify(healthChecks =>
    //    {
    //        _ = healthChecks.AddBlobContainerAvailability(
    //            "BlobHttpSharedKeyHealthy",
    //            options =>
    //            {
    //                options.ContainerName = "test";
    //                options.AccountKey = AzuriteHttpAccess.AccountKey;
    //                options.AccountName = AzuriteHttpAccess.AccountName;
    //                options.Mode = ClientCreationMode.SharedKey;
    //                options.ServiceUri = _container.BlobEndpoint;
    //            }
    //        );
    //    });

    //[Fact]
    //public async Task AddBlobContainerAvailability_UseOptions_ModeSharedKey_ShouldReturnDegraded() =>
    //    await RunAndVerify(healthChecks =>
    //    {
    //        _ = healthChecks.AddBlobContainerAvailability(
    //            "BlobHttpSharedKeyDegraded",
    //            options =>
    //            {
    //                options.ContainerName = "test";
    //                options.AccountKey = AzuriteHttpAccess.AccountKey;
    //                options.AccountName = AzuriteHttpAccess.AccountName;
    //                options.Mode = ClientCreationMode.SharedKey;
    //                options.ServiceUri = _container.BlobEndpoint;
    //                options.Timeout = 0;
    //            }
    //        );
    //    });

    [Fact]
    public async Task AddBlobContainerAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobContainerAvailability(
                "BlobHttpAzureSasCredentialHealthy",
                options =>
                {
                    options.ContainerName = "test";
                    options.Mode = ClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                }
            );
        });

    [Fact]
    public async Task AddBlobContainerAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobContainerAvailability(
                "BlobHttpAzureSasCredentialDegraded",
                options =>
                {
                    options.ContainerName = "test";
                    options.Mode = ClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                    options.Timeout = 0;
                }
            );
        });
}
