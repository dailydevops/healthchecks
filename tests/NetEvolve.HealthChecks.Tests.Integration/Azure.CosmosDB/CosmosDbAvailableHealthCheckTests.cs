namespace NetEvolve.HealthChecks.Tests.Integration.Azure.CosmosDB;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.CosmosDB;
using NetEvolve.HealthChecks.Tests.Integration.Internals;

[TestGroup($"{nameof(Azure)}.{nameof(CosmosDB)}")]
[ClassDataSource<CosmosDbAccess>(Shared = SharedType.PerClass)]
[TestGroup("Z02TestGroup")]
public class CosmosDbAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly CosmosDbAccess _container;

    public CosmosDbAvailableHealthCheckTests(CosmosDbAccess container) => _container = container;

    [Test]
    public async Task AddCosmosDbAvailability_UseOptions_ModeConnectionString_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCosmosDbAvailability(
                    "CosmosDbConnectionStringHealthy",
                    options =>
                    {
                        options.Mode = CosmosDbClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddCosmosDbAvailability_UseOptions_ModeConnectionString_WithDatabaseId_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCosmosDbAvailability(
                    "CosmosDbConnectionStringWithDatabaseHealthy",
                    options =>
                    {
                        options.Mode = CosmosDbClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.DatabaseId = "testdb";
                        options.Timeout = 10000; // Set a reasonable timeout
                        options.ClientConfiguration = _container.ClientConfiguration;
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddCosmosDbAvailability_UseOptions_ModeConnectionString_Degraded(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCosmosDbAvailability(
                    "CosmosDbConnectionStringDegraded",
                    options =>
                    {
                        options.Mode = CosmosDbClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddCosmosDbAvailability_UseOptions_ModeServiceProvider_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCosmosDbAvailability(
                    "CosmosDbServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = CosmosDbClientCreationMode.ServiceProvider;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_container.CosmosClient),
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddCosmosDbAvailability_UseOptions_ModeServiceProvider_WithDatabaseId_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCosmosDbAvailability(
                    "CosmosDbServiceProviderWithDatabaseHealthy",
                    options =>
                    {
                        options.Mode = CosmosDbClientCreationMode.ServiceProvider;
                        options.DatabaseId = "testdb";
                        options.Timeout = 10000; // Set a reasonable timeout
                        options.KeyedService = "test";
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("test", _container.CosmosClient),
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddCosmosDbAvailability_UseOptions_ModeServiceProvider_WithNonExistingDatabaseId_Unhealthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCosmosDbAvailability(
                    "CosmosDbServiceProviderWithNonExistingDatabaseIdUnhealthy",
                    options =>
                    {
                        options.Mode = CosmosDbClientCreationMode.ServiceProvider;
                        options.DatabaseId = "testdb-nonexisting";
                        options.Timeout = 10000; // Set a reasonable timeout
                        options.KeyedService = "test";
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddKeyedSingleton("test", _container.CosmosClient),
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddCosmosDbAvailability_UseOptions_ModeAccountKey_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCosmosDbAvailability(
                    "CosmosDbAccountKeyHealthy",
                    options =>
                    {
                        options.Mode = CosmosDbClientCreationMode.AccountKey;
                        options.AccountEndpoint = _container.AccountEndpoint;
                        options.AccountKey = CosmosDbAccess.AccountKey;
                        options.ClientConfiguration = _container.ClientConfiguration;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddCosmosDbAvailability_UseOptions_ModeAccountKey_WithDatabaseId_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCosmosDbAvailability(
                    "CosmosDbAccountKeyWithDatabaseHealthy",
                    options =>
                    {
                        options.Mode = CosmosDbClientCreationMode.AccountKey;
                        options.AccountEndpoint = _container.AccountEndpoint;
                        options.AccountKey = CosmosDbAccess.AccountKey;
                        options.DatabaseId = "testdb";
                        options.ClientConfiguration = _container.ClientConfiguration;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddCosmosDbAvailability_UseOptions_ModeAccountKey_Degraded(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCosmosDbAvailability(
                    "CosmosDbAccountKeyDegraded",
                    options =>
                    {
                        options.Mode = CosmosDbClientCreationMode.AccountKey;
                        options.AccountEndpoint = _container.AccountEndpoint;
                        options.AccountKey = CosmosDbAccess.AccountKey;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddCosmosDbAvailability_UseOptions_ModeAccountKey_WithNonExistingDatabaseId_Unhealthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCosmosDbAvailability(
                    "CosmosDbAccountKeyWithNonExistingDatabaseIdUnhealthy",
                    options =>
                    {
                        options.Mode = CosmosDbClientCreationMode.AccountKey;
                        options.AccountEndpoint = _container.AccountEndpoint;
                        options.AccountKey = CosmosDbAccess.AccountKey;
                        options.DatabaseId = "testdb-nonexisting";
                        options.ClientConfiguration = _container.ClientConfiguration;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );
}
