namespace NetEvolve.HealthChecks.Azure.Tables.Tests.Integration;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using global::Azure.Data.Tables;
using global::Azure.Data.Tables.Sas;
using Microsoft.Extensions.Azure;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Tests;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
[SetCulture("en-US")]
public class TableServiceAvailableHealthCheckTests
    : HealthCheckTestBase,
        IClassFixture<AzuriteAccess>
{
    private readonly AzuriteAccess _container;
    private readonly Uri _uriTableStorage;

    public TableServiceAvailableHealthCheckTests(AzuriteAccess container)
    {
        _container = container;

        var client = new TableServiceClient(_container.ConnectionString);
        _uriTableStorage = client.Uri;
    }

    [Fact]
    public async Task AddTableServiceAvailability_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableServiceAvailability(
                    "ServiceServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = TableClientCreationMode.ServiceProvider;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddTableServiceClient(_container.ConnectionString)
                );
            }
        );

    [Fact]
    public async Task AddTableServiceAvailability_UseOptionsWithAdditionalConfiguration_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddTableServiceAvailability(
                    "ServiceServiceProviderWithConfigurationHealthy",
                    options =>
                    {
                        options.Mode = TableClientCreationMode.ServiceProvider;
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
                    _ = clients.AddTableServiceClient(_container.ConnectionString)
                );
            }
        );

    [Fact]
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
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddTableServiceClient(_container.ConnectionString)
                );
            }
        );

    [Fact]
    public async Task AddTableServiceAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddTableServiceAvailability(
                "ServiceConnectionStringHealthy",
                options =>
                {
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = TableClientCreationMode.ConnectionString;
                }
            );
        });

    [Fact]
    public async Task AddTableServiceAvailability_UseOptions_ModeConnectionString_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
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
        });

    [Fact]
    public async Task AddTableServiceAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddTableServiceAvailability(
                "ServiceSharedKeyHealthy",
                options =>
                {
                    options.AccountKey = AzuriteAccess.AccountKey;
                    options.AccountName = AzuriteAccess.AccountName;
                    options.Mode = TableClientCreationMode.SharedKey;
                    options.ServiceUri = _uriTableStorage;
                    options.ConfigureClientOptions = clientOptions =>
                    {
                        clientOptions.Retry.MaxRetries = 0;
                    };
                }
            );
        });

    [Fact]
    public async Task AddTableServiceAvailability_UseOptions_ModeSharedKey_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddTableServiceAvailability(
                "ServiceSharedKeyDegraded",
                options =>
                {
                    options.AccountKey = AzuriteAccess.AccountKey;
                    options.AccountName = AzuriteAccess.AccountName;
                    options.Mode = TableClientCreationMode.SharedKey;
                    options.Timeout = 0;
                    options.ServiceUri = _uriTableStorage;
                }
            );
        });
}
