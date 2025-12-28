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
        HealthStatus failureStatus,
        CouchDbOptions options,
        CancellationToken cancellationToken
    )
    {
        var connectionInfo = new DbConnectionInfo(options.ConnectionString, options.DatabaseName);
        using var client = new MyCouchClient(connectionInfo);

        var (isTimelyResponse, result) = await options
            .CommandAsync.Invoke(client, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"CouchDB health check '{name}' failed.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(MyCouchClient client, CancellationToken cancellationToken)
    {
        _ = await client.Database.HeadAsync(cancellationToken).ConfigureAwait(false);

        return true;
    }
}
