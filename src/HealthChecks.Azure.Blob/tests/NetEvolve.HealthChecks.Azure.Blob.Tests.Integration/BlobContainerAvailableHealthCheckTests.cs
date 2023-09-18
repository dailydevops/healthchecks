namespace NetEvolve.HealthChecks.Azure.Blob.Tests.Integration;

using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Tests;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
[SetCulture]
public class BlobContainerAvailableHealthCheckHttpTests
    : HealthCheckTestBase,
        IClassFixture<AzuriteContainerAccess>
{
    private const string AccountName = "devstoreaccount1";

    private const string SharedAccessToken =
        "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

    private readonly AzuriteContainerAccess _container;

    public BlobContainerAvailableHealthCheckHttpTests(AzuriteContainerAccess container) =>
        _container = container;

    [Fact]
    public async Task AddAzureBlobAvailability_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddAzureBlobAvailability(
                "TestContainerHealthy",
                options =>
                {
                    options.ConnectionString = _container.ConnectionString;
                    options.Mode = ClientCreationMode.ConnectionString;
                }
            );
        });

    [Fact]
    public async Task AddAzureBlobAvailability_UseOptions_ModeSharedKey_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddAzureBlobAvailability(
                "TestContainerHealthy",
                options =>
                {
                    options.AccountName = AccountName;
                    options.Mode = ClientCreationMode.SharedKey;
                    options.ServiceUri = _container.BlobEndpoint;
                    options.SharedAccessToken = SharedAccessToken;
                }
            );
        });
}
