namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Queues;

using System;
using System.Threading.Tasks;
using global::Azure.Storage.Queues;
using global::Azure.Storage.Sas;
using Microsoft.Extensions.Azure;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Queues;
using Xunit;

[TestGroup("AzureQueues")]
public class QueueClientAvailableHealthCheckTests : HealthCheckTestBase, IClassFixture<AzuriteAccess>
{
    private readonly AzuriteAccess _container;
    private readonly Uri _accountSasUri;
    private readonly Uri _uriQueueStorage;

    public QueueClientAvailableHealthCheckTests(AzuriteAccess container)
    {
        _container = container;

        var client = new QueueServiceClient(_container.ConnectionString);
        _uriQueueStorage = client.Uri;

        _accountSasUri = client.GenerateAccountSasUri(
            AccountSasPermissions.All,
            DateTimeOffset.UtcNow.AddDays(1),
            AccountSasResourceTypes.All
        );

        var queueClient = client.GetQueueClient("test");

        if (!queueClient.Exists())
        {
            _ = queueClient.Create();
        }
    }

    [Fact]
    public async Task AddQueueClientAvailability_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueServiceProviderHealthy",
                    options =>
                    {
                        options.QueueName = "test";
                        options.Mode = QueueClientCreationMode.ServiceProvider;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
    public async Task AddQueueClientAvailability_UseOptions_ModeServiceProvider_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueServiceProviderDegraded",
                    options =>
                    {
                        options.QueueName = "test";
                        options.Mode = QueueClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
    public async Task AddQueueClientAvailability_UseOptions_ModeServiceProvider_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueServiceProviderNotExistsUnhealthy",
                    options =>
                    {
                        options.QueueName = "notexists";
                        options.Mode = QueueClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
    public async Task AddQueueClientAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQueueClientAvailability(
                    "QueueServiceProviderWithConfigurationHealthy",
                    options =>
                    {
                        options.QueueName = "test";
                        options.Mode = QueueClientCreationMode.ServiceProvider;
                        options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddQueueServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
    public async Task AddQueueClientAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddQueueClientAvailability(
                "QueueConnectionStringHealthy",
                options =>
                {
                    options.QueueName = "test";
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = QueueClientCreationMode.ConnectionString;
                }
            );
        });

    [Fact]
    public async Task AddQueueClientAvailability_UseOptions_ModeConnectionString_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddQueueClientAvailability(
                "QueueConnectionStringDegraded",
                options =>
                {
                    options.QueueName = "test";
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = QueueClientCreationMode.ConnectionString;
                    options.Timeout = 0;
                }
            );
        });

    [Fact]
    public async Task AddQueueClientAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddQueueClientAvailability(
                "QueueSharedKeyHealthy",
                options =>
                {
                    options.QueueName = "test";
                    options.AccountKey = AzuriteAccess.AccountKey;
                    options.AccountName = AzuriteAccess.AccountName;
                    options.Mode = QueueClientCreationMode.SharedKey;
                    options.ServiceUri = _uriQueueStorage;
                    options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                }
            );
        });

    [Fact]
    public async Task AddQueueClientAvailability_UseOptions_ModeSharedKey_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddQueueClientAvailability(
                "QueueSharedKeyDegraded",
                options =>
                {
                    options.QueueName = "test";
                    options.AccountKey = AzuriteAccess.AccountKey;
                    options.AccountName = AzuriteAccess.AccountName;
                    options.Mode = QueueClientCreationMode.SharedKey;
                    options.Timeout = 0;
                    options.ServiceUri = _uriQueueStorage;
                }
            );
        });

    [Fact]
    public async Task AddQueueClientAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddQueueClientAvailability(
                "QueueAzureSasCredentialHealthy",
                options =>
                {
                    options.QueueName = "test";
                    options.Mode = QueueClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                }
            );
        });

    [Fact]
    public async Task AddQueueClientAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddQueueClientAvailability(
                "QueueAzureSasCredentialDegraded",
                options =>
                {
                    options.QueueName = "test";
                    options.Mode = QueueClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                    options.Timeout = 0;
                }
            );
        });
}
