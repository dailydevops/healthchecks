﻿namespace NetEvolve.HealthChecks.Azure.Blobs.Tests.Integration;

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
public class BlobContainerAvailableHealthCheckHttpsTests
    : HealthCheckTestBase,
        IClassFixture<AzuriteHttpsAccess>
{
    private readonly AzuriteHttpsAccess _container;
    private readonly Uri _accountSasUri;

    // TODO: Remove when TestContainers is providing this settíng
    private readonly Uri _uriBlobStorage;

    public BlobContainerAvailableHealthCheckHttpsTests(AzuriteHttpsAccess container)
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
                    "BlobHttpsServiceProviderHealthy",
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
    public async Task AddBlobContainerAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobHttpsServiceProviderWithConfigurationHealthy",
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
    public async Task AddBlobContainerAvailability_UseOptions_ModeServiceProvider_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBlobContainerAvailability(
                    "BlobHttpsServiceProviderDegraded",
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
                    "BlobHttpsServiceProviderNotExistsUnhealty",
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
    public async Task AddBlobContainerAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobContainerAvailability(
                "BlobHttpsConnectionStringHealthy",
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
                "BlobHttpsConnectionStringDegraded",
                options =>
                {
                    options.ContainerName = "test";
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.Timeout = 0;
                }
            );
        });

    [Fact]
    public async Task AddBlobContainerAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobContainerAvailability(
                "BlobHttpsSharedKeyHealthy",
                options =>
                {
                    options.ContainerName = "test";
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
    public async Task AddBlobContainerAvailability_UseOptions_ModeSharedKey_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobContainerAvailability(
                "BlobHttpsSharedKeyDegraded",
                options =>
                {
                    options.ContainerName = "test";
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
    public async Task AddBlobContainerAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddBlobContainerAvailability(
                "BlobHttpsAzureSasCredentialHealthy",
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
                "BlobHttpsAzureSasCredentialDegraded",
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
