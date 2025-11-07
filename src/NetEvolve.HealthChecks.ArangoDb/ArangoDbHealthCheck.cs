namespace NetEvolve.HealthChecks.ArangoDb;

using System.Linq;
using System.Threading.Tasks;
using ArangoDBNetStandard;
using ArangoDBNetStandard.CursorApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(ArangoDbOptions))]
internal sealed partial class ArangoDbHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        ArangoDbOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientProvider = _serviceProvider.GetRequiredService<ArangoDbClientProvider>();
        var client = clientProvider.GetClient(name, options, _serviceProvider);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, isResultValid) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse && isResultValid, name);
    }

    internal static async Task<bool> DefaultCommandAsync(ArangoDBClient client, CancellationToken cancellationToken)
    {
        var cursor = await client
            .Cursor.PostCursorAsync<int>(new PostCursorBody { Query = "RETURN 1" }, null, cancellationToken)
            .ConfigureAwait(false);

        return cursor?.Result?.Any() == true;
    }
}
