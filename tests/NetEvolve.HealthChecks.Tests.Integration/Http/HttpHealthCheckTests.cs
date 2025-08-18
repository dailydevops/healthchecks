namespace NetEvolve.HealthChecks.Tests.Integration.Http;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Http;

[TestGroup(nameof(Http))]
public class HttpHealthCheckTests : HealthCheckTestBase
{
    [Test]
    public async Task AddHttp_UseOptions_WithInvalidUri_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddHttp(
                    "TestInvalidUri",
                    options =>
                    {
                        options.Uri = "https://invalid-domain-that-does-not-exist-12345.com";
                        options.Timeout = 1000;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddHttp_UseConfiguration_WithInvalidUri_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddHttp("TestInvalidUri"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Http:TestInvalidUri:Uri", "https://invalid-domain-that-does-not-exist-12345.com" },
                    { "HealthChecks:Http:TestInvalidUri:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}