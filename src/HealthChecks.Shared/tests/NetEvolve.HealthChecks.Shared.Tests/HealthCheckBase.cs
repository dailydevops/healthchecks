namespace NetEvolve.HealthChecks.Tests.Shared;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using VerifyXunit;

public abstract class HealthCheckTestBase
{
    private const string HealthCheckPath = "/health";

    protected static async ValueTask RunAndVerify(Action<IHealthChecksBuilder> healthChecks)
    {
        var builder = new WebHostBuilder().ConfigureServices(services =>
        {
            var healthChecksBuilder = services.AddHealthChecks();
            healthChecks?.Invoke(healthChecksBuilder);
        }).Configure(app =>
        {
            _ = app.UseHealthChecks(HealthCheckPath);
        });

        using (var server = new TestServer(builder))
        {
            var client = server.CreateClient();
            var response = await client.GetAsync(new Uri(HealthCheckPath, UriKind.Relative)).ConfigureAwait(false);

            var state = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var statusCode = (int)response.StatusCode;

            _ = await Verifier.Verify(new { state, statusCode }).UseDirectory("_snapshot");
        }
    }
}
