namespace NetEvolve.HealthChecks.Neo4j;

using System.Threading.Tasks;
using global::Neo4j.Driver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(Neo4jOptions))]
internal sealed partial class Neo4jHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        Neo4jOptions options,
        CancellationToken cancellationToken
    )
    {
        var driver = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IDriver>()
            : _serviceProvider.GetRequiredKeyedService<IDriver>(options.KeyedService);

        var commandTask = options.CommandAsync.Invoke(driver, cancellationToken);

        var (isTimelyResponse, _) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<IResultCursor> DefaultCommandAsync(IDriver driver, CancellationToken cancellationToken)
    {
        var session = driver.AsyncSession();

        await using (session.ConfigureAwait(false))
        {
            return await session.RunAsync("RETURN 1", cancellationToken).ConfigureAwait(false);
        }
    }
}
