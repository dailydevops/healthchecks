namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Files;

using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Files;

[TestGroup($"{nameof(Azure)}.{nameof(Files)}")]
[ClassDataSource<AzuriteAccess>(Shared = InstanceSharedType.Azure)]
public class FileShareAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly AzuriteAccess _container;

    public FileShareAvailableHealthCheckTests(AzuriteAccess container) => _container = container;

    [Test]
    public async Task AddFileShareAvailability_UseOptions_ModeConnectionString_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddFileShareAvailability(
                    "FileConnectionStringHealthy",
                    options =>
                    {
                        options.ShareName = "test-share";
                        options.Mode = FileClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddFileShareAvailability_UseOptions_ModeConnectionString_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddFileShareAvailability(
                    "FileConnectionStringDegraded",
                    options =>
                    {
                        options.ShareName = "test-share";
                        options.Mode = FileClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );
}