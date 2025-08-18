namespace NetEvolve.HealthChecks.Tests.Integration.Azure.DigitalTwin;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.DigitalTwin;

[TestGroup($"{nameof(Azure)}.{nameof(DigitalTwin)}")]
public class DigitalTwinServiceAvailableHealthCheckTests : HealthCheckTestBase
{
    [Test]
    public async Task AddDigitalTwinServiceAvailability_WithDefaultAzureCredentialsMode_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDigitalTwinServiceAvailability(
                    "DigitalTwinService",
                    options =>
                    {
                        options.Mode = DigitalTwinClientCreationMode.DefaultAzureCredentials;
                        options.ServiceUri = new Uri("https://test.api.wcus.digitaltwins.azure.net");
                        options.Timeout = 100; // Short timeout for test
                    }
                );
            },
            HealthStatus.Unhealthy // Expected to be unhealthy since we can't connect to a real service
        );

    [Test]
    public async Task AddDigitalTwinServiceAvailability_WithServiceProviderMode_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDigitalTwinServiceAvailability(
                    "DigitalTwinServiceProvider",
                    options =>
                    {
                        options.Mode = DigitalTwinClientCreationMode.ServiceProvider;
                        options.Timeout = 100;
                    }
                );
            },
            HealthStatus.Unhealthy // Expected to be unhealthy since no service is registered
        );
}