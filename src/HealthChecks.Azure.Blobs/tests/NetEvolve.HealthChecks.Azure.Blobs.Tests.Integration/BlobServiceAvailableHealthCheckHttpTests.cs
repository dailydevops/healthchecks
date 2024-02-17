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
public class BlobServiceAvailableHealthCheckHttpTests
    : HealthCheckTestBase,
        IClassFixture<AzuriteHttpAccess>
{
    private readonly AzuriteHttpAccess _container;
    private readonly Uri _accountSasUri;

    public BlobServiceAvailableHealthCheckHttpTests(AzuriteHttpAccess container)
    {
        _container = container;

        var client = new BlobServiceClient(_container.ConnectionString);

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
                    "ServiceHttpServiceProviderHealthy",
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
    public async Task AddBlobServiceAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceHttpServiceProviderWithConfigurationHealthy",
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
    public async Task AddBlobServiceAvailability_UseOptions_ModeServiceProvider_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobServiceAvailability(
                    "ServiceHttpServiceProviderDegraded",
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
    public async Task AddBlobServiceAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceHttpConnectionStringHealthy",
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
                "ServiceHttpConnectionStringDegraded",
                options =>
                {
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.Timeout = 0;
                }
            );
        });

    //[Fact]
    //public async Task AddBlobServiceAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
    //    await RunAndVerify(healthChecks =>
    //    {
    //        _ = healthChecks.AddBlobServiceAvailability(
    //            "ServiceHttpSharedKeyHealthy",
    //            options =>
    //            {
    //                options.AccountKey = AzuriteHttpAccess.AccountKey;
    //                options.AccountName = AzuriteHttpAccess.AccountName;
    //                options.Mode = ClientCreationMode.SharedKey;
    //                options.ServiceUri = _container.BlobEndpoint;
    //            }
    //        );
    //    });

    //[Fact]
    //public async Task AddBlobServiceAvailability_UseOptions_ModeSharedKey_ShouldReturnDegraded() =>
    //    await RunAndVerify(healthChecks =>
    //    {
    //        _ = healthChecks.AddBlobServiceAvailability(
    //            "ServiceHttpSharedKeyDegraded",
    //            options =>
    //            {
    //                options.AccountKey = AzuriteHttpAccess.AccountKey;
    //                options.AccountName = AzuriteHttpAccess.AccountName;
    //                options.Mode = ClientCreationMode.SharedKey;
    //                options.ServiceUri = _container.BlobEndpoint;
    //                options.Timeout = 0;
    //            }
    //        );
    //    });

    [Fact]
    public async Task AddBlobServiceAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobServiceAvailability(
                "ServiceHttpAzureSasCredentialHealthy",
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
                "ServiceHttpAzureSasCredentialDegraded",
                options =>
                {
                    options.Mode = ClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                    options.Timeout = 0;
                }
            );
        });
}
