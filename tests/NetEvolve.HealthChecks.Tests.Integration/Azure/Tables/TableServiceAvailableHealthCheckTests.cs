namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Tables;

using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Tables;

[TestGroup($"{nameof(Azure)}.{nameof(Tables)}")]
[ClassDataSource<AzuriteAccess>(Shared = SharedType.PerTestSession)]
public class TableServiceAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly AzuriteAccess _container;

    public TableServiceAvailableHealthCheckTests(AzuriteAccess container) => _container = container;

    [Test]
    public async Task AddTableServiceAvailability_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableServiceAvailability(
                    "ServiceServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = TableClientCreationMode.ServiceProvider;
                        options.Timeout = 1000;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddTableServiceAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableServiceAvailability(
                    "ServiceServiceProviderWithConfigurationHealthy",
                    options =>
                    {
                        options.Mode = TableClientCreationMode.ServiceProvider;
                        options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                        options.Timeout = 1000;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddTableServiceClient(_container.ConnectionString))
        );

    [Test]
    public async Task AddTableServiceAvailability_UseOptions_ModeServiceProvider_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableServiceAvailability(
                    "ServiceServiceProviderDegraded",
                    options =>
                    {
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
    public async Task AddTableServiceAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableServiceAvailability(
                    "ServiceConnectionStringHealthy",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = TableClientCreationMode.ConnectionString;
                        options.Timeout = 1000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddTableServiceAvailability_UseOptions_ModeConnectionString_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableServiceAvailability(
                    "ServiceConnectionStringDegraded",
                    options =>
                    {
                        options.ConnectionString = _container.ConnectionString;
                        options.Mode = TableClientCreationMode.ConnectionString;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddTableServiceAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableServiceAvailability(
                    "ServiceSharedKeyHealthy",
                    options =>
                    {
                        options.AccountKey = AzuriteAccess.AccountKey;
                        options.AccountName = AzuriteAccess.AccountName;
                        options.Mode = TableClientCreationMode.SharedKey;
                        options.ServiceUri = _container.TableServiceEndpoint;
                        options.ConfigureClientOptions = clientOptions => clientOptions.Retry.MaxRetries = 0;
                        options.Timeout = 1000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddTableServiceAvailability_UseOptions_ModeSharedKey_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableServiceAvailability(
                    "ServiceSharedKeyDegraded",
                    options =>
                    {
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
}
