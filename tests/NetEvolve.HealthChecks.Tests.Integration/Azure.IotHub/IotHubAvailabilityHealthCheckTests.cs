namespace NetEvolve.HealthChecks.Tests.Integration.Azure.IotHub;

using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.IotHub;

[TestGroup($"{nameof(Azure)}.{nameof(IotHub)}")]
[ClassDataSource<IoTHubMockContainer>(Shared = InstanceSharedType.Azure)]
public class IotHubAvailabilityHealthCheckTests : HealthCheckTestBase
{
    private readonly IoTHubMockContainer _container;

    public IotHubAvailabilityHealthCheckTests(IoTHubMockContainer container) => _container = container;

    [
        Test,
        Skip(
            "Azure IoT Hub SDK requires valid Azure hostnames and certificates. Mock servers cannot fully simulate the authentication flow."
        )
    ]
    public async Task AddAzureIotHubAvailability_UseOptions_ModeConnectionString_MockTest() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureIotHubAvailability(
                    "IotHubMockTest",
                    options =>
                    {
                        // Note: This will fail because Azure IoT Hub SDK validates hostnames
                        // and requires HTTPS with proper certificates
                        options.ConnectionString = _container.MockConnectionString;
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.Timeout = 5000;
                    }
                );
            },
            HealthStatus.Degraded // Expected to fail due to SDK limitations
        );

    [Test]
    public async Task AddAzureIotHubAvailability_UseOptions_InvalidConnectionString_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureIotHubAvailability(
                    "IotHubInvalidConnectionString",
                    options =>
                    {
                        options.ConnectionString = "invalid-connection-string";
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.Timeout = 1000;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddAzureIotHubAvailability_UseOptions_Timeout_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureIotHubAvailability(
                    "IotHubTimeout",
                    options =>
                    {
                        options.ConnectionString =
                            "HostName=nonexistent.azure-devices.net;SharedAccessKeyName=test;SharedAccessKey=dGVzdA==";
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.Timeout = 1; // Very short timeout to trigger degraded status
                    }
                );
            },
            HealthStatus.Degraded
        );
}
