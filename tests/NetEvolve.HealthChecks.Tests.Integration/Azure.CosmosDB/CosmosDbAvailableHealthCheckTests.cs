namespace NetEvolve.HealthChecks.Tests.Integration.Azure.CosmosDB;

using System.Threading.Tasks;
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
    public async Task AddCosmosDbAvailability_UseOptions_ModeConnectionString_Healthy() =>
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
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddCosmosDbAvailability_UseOptions_ModeConnectionString_WithDatabaseId_Healthy() =>
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
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddCosmosDbAvailability_UseOptions_ModeConnectionString_Degraded() =>
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
            HealthStatus.Degraded
        );
}
