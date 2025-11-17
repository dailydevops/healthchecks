namespace NetEvolve.HealthChecks.Neo4j;

using System.Threading.Tasks;
using global::Neo4j.Driver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(Neo4jOptions))]
#pragma warning disable S101 // Types should be named in PascalCase
internal sealed partial class Neo4jHealthCheck
#pragma warning restore S101 // Types should be named in PascalCase
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        Neo4jOptions options,
        CancellationToken cancellationToken
    )
    {
        var driver = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IDriver>()
            : _serviceProvider.GetRequiredKeyedService<IDriver>(options.KeyedService);

        var commandTask = options.CommandAsync.Invoke(driver, cancellationToken);

        var (isTimelyResponse, result) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "The command did not return a valid result.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(IDriver driver, CancellationToken cancellationToken)
    {
        var session = driver.AsyncSession();

        await using (session.ConfigureAwait(false))
        {
            _ = await session.RunAsync("RETURN 1").ConfigureAwait(false);

            return true;
        }
    }
}
