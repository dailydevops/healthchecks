namespace NetEvolve.HealthChecks.CouchDb;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyCouch;
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
        var dbUri = new Uri(options.ConnectionString!);
        var connectionInfo = new DbConnectionInfo(
            dbUri.GetLeftPart(UriPartial.Authority),
            dbUri.AbsolutePath.Trim('/')
        );
        using var client = new MyCouchClient(connectionInfo);

        var sw = Stopwatch.StartNew();
        await options.CommandAsync.Invoke(client, cancellationToken).ConfigureAwait(false);
        sw.Stop();

        var isTimelyResponse = options.Timeout >= sw.Elapsed.TotalMilliseconds;

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task DefaultCommandAsync(MyCouchClient client, CancellationToken cancellationToken) =>
        _ = await client.Database.HeadAsync(cancellationToken).ConfigureAwait(false);
}
