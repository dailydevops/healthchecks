namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Tables;

using System;
using System.Linq;
using System.Threading.Tasks;
using global::Azure.Data.Tables;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Tables;

[TestGroup($"{nameof(Azure)}.{nameof(Tables)}")]
[TestGroup("Z02TestGroup")]
[ClassDataSource<AzuriteAccess>(Shared = SharedType.PerClass)]
public class TableClientAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly AzuriteAccess _container;

    public TableClientAvailableHealthCheckTests(AzuriteAccess container) => _container = container;

    [Test]
    public async Task AddTableClientAvailability_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableClientAvailability(
                    "TableServiceProviderHealthy",
                    options =>
                    {
                        options.TableName = "test";
                        options.Mode = TableClientCreationMode.ServiceProvider;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddTableClientAvailability_UseOptions_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableClientAvailability(
                    "TableServiceProviderKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "test-key";
                        options.TableName = "test";
                        options.Mode = TableClientCreationMode.ServiceProvider;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString));
                _ = services.AddKeyedSingleton(
                    "test-key",
                    (serviceProvider, _) => serviceProvider.GetRequiredService<TableServiceClient>()
                );
            }
        );

    [Test]
    public async Task AddTableClientAvailability_UseOptions_ModeServiceProvider_Degraded() =>
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
            HealthStatus.Degraded,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddTableClientAvailability_UseOptions_ModeServiceProvider_Unhealthy() =>
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
            HealthStatus.Unhealthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString)),
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

    [Test]
    public async Task AddTableClientAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_Healthy() =>
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
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddTableClientAvailability_UseOptions_ModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableClientAvailability(
                    "TableConnectionStringHealthy",
                    options =>
                    {
                        options.TableName = "test";
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = TableClientCreationMode.ConnectionString;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddTableClientAvailability_UseOptions_ModeConnectionString_Degraded() =>
        await RunAndVerify(
            healthChecks =>
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
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddTableClientAvailability_UseOptions_ModeSharedKey_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableClientAvailability(
                    "TableSharedKeyHealthy",
                    options =>
                    {
                        options.TableName = "test";
                        options.AccountKey = AzuriteAccess.AccountKey;
                        options.AccountName = AzuriteAccess.AccountName;
                        options.Mode = TableClientCreationMode.SharedKey;
                        options.ServiceUri = _container.TableServiceEndpoint;
                        options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddTableClientAvailability_UseOptions_ModeSharedKey_Degraded() =>
        await RunAndVerify(
            healthChecks =>
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
                        options.ServiceUri = _container.TableServiceEndpoint;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddTableClientAvailability_UseOptions_ModeAzureSasCredential_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableClientAvailability(
                    "TableAzureSasCredentialHealthy",
                    options =>
                    {
                        options.TableName = "test";
                        options.Mode = TableClientCreationMode.AzureSasCredential;
                        options.ServiceUri = _container.TableAccountSasUri;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddTableClientAvailability_UseOptions_ModeAzureSasCredential_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableClientAvailability(
                    "TableAzureSasCredentialDegraded",
                    options =>
                    {
                        options.TableName = "test";
                        options.Mode = TableClientCreationMode.AzureSasCredential;
                        options.ServiceUri = _container.TableAccountSasUri;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    // Configuration-based tests

    [Test]
    public async Task AddTableClientAvailability_UseConfiguration_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddTableClientAvailability("TableConfigurationHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureTableClient:TableConfigurationHealthy:Mode",
                        nameof(TableClientCreationMode.ServiceProvider)
                    },
                    { "HealthChecks:AzureTableClient:TableConfigurationHealthy:TableName", "test" },
                    { "HealthChecks:AzureTableClient:TableConfigurationHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddTableClientAvailability_UseConfiguration_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddTableClientAvailability("TableConfigurationKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AzureTableClient:TableConfigurationKeyedHealthy:KeyedService", "test-key" },
                    {
                        "HealthChecks:AzureTableClient:TableConfigurationKeyedHealthy:Mode",
                        nameof(TableClientCreationMode.ServiceProvider)
                    },
                    { "HealthChecks:AzureTableClient:TableConfigurationKeyedHealthy:TableName", "test" },
                    { "HealthChecks:AzureTableClient:TableConfigurationKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString));
                _ = services.AddKeyedSingleton(
                    "test-key",
                    (serviceProvider, _) => serviceProvider.GetRequiredService<TableServiceClient>()
                );
            }
        );

    [Test]
    public async Task AddTableClientAvailability_UseConfiguration_ModeServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddTableClientAvailability("TableConfigurationDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureTableClient:TableConfigurationDegraded:Mode",
                        nameof(TableClientCreationMode.ServiceProvider)
                    },
                    { "HealthChecks:AzureTableClient:TableConfigurationDegraded:TableName", "test" },
                    { "HealthChecks:AzureTableClient:TableConfigurationDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddTableClientAvailability_UseConfiguration_ModeServiceProvider_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddTableClientAvailability("TableConfigurationNotExistsUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureTableClient:TableConfigurationNotExistsUnhealthy:Mode",
                        nameof(TableClientCreationMode.ServiceProvider)
                    },
                    { "HealthChecks:AzureTableClient:TableConfigurationNotExistsUnhealthy:TableName", "notexists" },
                    { "HealthChecks:AzureTableClient:TableConfigurationNotExistsUnhealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString)),
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
}
