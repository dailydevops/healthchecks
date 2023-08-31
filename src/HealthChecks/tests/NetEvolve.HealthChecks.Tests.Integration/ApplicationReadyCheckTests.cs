﻿namespace NetEvolve.HealthChecks.Tests.Integration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks;
using NetEvolve.HealthChecks.Tests;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
public class ApplicationReadyCheckTests : HealthCheckTestBase
{
    [Fact]
    public async Task AddApplicationReady_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
            {
                _ = healthChecks.AddApplicationReady();
            })
            .ConfigureAwait(false);

    [Fact]
    public async Task AddApplicationReady_WithCustomName_ShouldReturnHealthy() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddApplicationReady();
                },
                serverConfiguration: server =>
                {
                    var lifetime = server.Services.GetRequiredService<IHostApplicationLifetime>();
                    lifetime.StopApplication();
                }
            )
            .ConfigureAwait(false);
}
