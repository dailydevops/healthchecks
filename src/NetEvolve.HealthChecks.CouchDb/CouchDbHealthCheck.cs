namespace NetEvolve.HealthChecks.CouchDb;

using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyCouch;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(CouchDbOptions))]
internal sealed partial class CouchDbHealthCheck
{
    private static async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        CouchDbOptions options,
        CancellationToken cancellationToken
    )
    {
        var connectionInfo = new DbConnectionInfo(options.ConnectionString, options.DatabaseName);
        using var client = new MyCouchClient(connectionInfo);

        var isTimelyResponse = await options
            .CommandAsync.Invoke(client, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task DefaultCommandAsync(MyCouchClient client, CancellationToken cancellationToken) =>
        _ = await client.Database.HeadAsync(cancellationToken).ConfigureAwait(false);
}
