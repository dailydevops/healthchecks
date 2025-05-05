namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Tables;

using System;
using System.Linq;
using System.Threading.Tasks;
using global::Azure.Data.Tables;
using global::Azure.Data.Tables.Sas;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Tables;
using Xunit;

[TestGroup("AzureTables")]
public class TableClientAvailableHealthCheckTests : HealthCheckTestBase, IClassFixture<AzuriteAccess>
{
    private readonly AzuriteAccess _container;
    private readonly Uri _accountSasUri;
    private readonly Uri _uriTableStorage;

    public TableClientAvailableHealthCheckTests(AzuriteAccess container)
    {
        _container = container;

        var client = new TableServiceClient(_container.ConnectionString);
        _uriTableStorage = client.Uri;

        var tableClient = client.GetTableClient("test");
        _ = tableClient.CreateIfNotExists();

        _accountSasUri = tableClient.GenerateSasUri(TableSasPermissions.All, DateTimeOffset.UtcNow.AddDays(1));
    }

    [Fact]
    public async Task AddTableClientAvailability_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableClientAvailability(
                    "TableServiceProviderHealthy",
                    options =>
                    {
                        options.TableName = "test";
                        options.Mode = TableClientCreationMode.ServiceProvider;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
    public async Task AddTableClientAvailability_UseOptions_ModeServiceProvider_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableClientAvailability(
                    "TableServiceProviderDegraded",
                    options =>
                    {
                        options.TableName = "test";
                        options.Mode = TableClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
    public async Task AddTableClientAvailability_UseOptions_ModeServiceProvider_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableClientAvailability(
                    "TableServiceProviderNotExistsUnhealthy",
                    options =>
                    {
                        options.TableName = "notexists";
                        options.Mode = TableClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString));
            },
            clearJToken: token =>
            {
                if (token is null)
                {
                    return null;
                }

                if (
                    token.Value<string>("status") is string status
                    && status.Equals(nameof(HealthCheckResult.Unhealthy), StringComparison.OrdinalIgnoreCase)
                )
                {
                    var results = token["results"].FirstOrDefault();

                    if (results?["exception"] is not null)
                    {
                        results["exception"]!["message"] = null;
                    }
                }

                return token;
            }
        );

    [Fact]
    public async Task AddTableClientAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableClientAvailability(
                    "TableServiceProviderWithConfigurationHealthy",
                    options =>
                    {
                        options.TableName = "test";
                        options.Mode = TableClientCreationMode.ServiceProvider;
                        options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString));
            }
        );

    [Fact]
    public async Task AddTableClientAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddTableClientAvailability(
                "TableConnectionStringHealthy",
                options =>
                {
                    options.TableName = "test";
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = TableClientCreationMode.ConnectionString;
                }
            );
        });

    [Fact]
    public async Task AddTableClientAvailability_UseOptions_ModeConnectionString_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddTableClientAvailability(
                "TableConnectionStringDegraded",
                options =>
                {
                    options.TableName = "test";
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = TableClientCreationMode.ConnectionString;
                    options.Timeout = 0;
                }
            );
        });

    [Fact]
    public async Task AddTableClientAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddTableClientAvailability(
                "TableSharedKeyHealthy",
                options =>
                {
                    options.TableName = "test";
                    options.AccountKey = AzuriteAccess.AccountKey;
                    options.AccountName = AzuriteAccess.AccountName;
                    options.Mode = TableClientCreationMode.SharedKey;
                    options.ServiceUri = _uriTableStorage;
                    options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                }
            );
        });

    [Fact]
    public async Task AddTableClientAvailability_UseOptions_ModeSharedKey_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddTableClientAvailability(
                "TableSharedKeyDegraded",
                options =>
                {
                    options.TableName = "test";
                    options.AccountKey = AzuriteAccess.AccountKey;
                    options.AccountName = AzuriteAccess.AccountName;
                    options.Mode = TableClientCreationMode.SharedKey;
                    options.Timeout = 0;
                    options.ServiceUri = _uriTableStorage;
                }
            );
        });

    [Fact]
    public async Task AddTableClientAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddTableClientAvailability(
                "TableAzureSasCredentialHealthy",
                options =>
                {
                    options.TableName = "test";
                    options.Mode = TableClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                }
            );
        });

    [Fact]
    public async Task AddTableClientAvailability_UseOptions_ModeAzureSasCredential_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddTableClientAvailability(
                "TableAzureSasCredentialDegraded",
                options =>
                {
                    options.TableName = "test";
                    options.Mode = TableClientCreationMode.AzureSasCredential;
                    options.ServiceUri = _accountSasUri;
                    options.Timeout = 0;
                }
            );
        });
}
