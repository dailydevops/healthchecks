namespace NetEvolve.HealthChecks.InfluxDB;

using System.Threading.Tasks;
using global::InfluxDB.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(InfluxDBOptions))]
internal sealed partial class InfluxDBHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        InfluxDBOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IInfluxDBClient>()
            : _serviceProvider.GetRequiredKeyedService<IInfluxDBClient>(options.KeyedService);

        var pingTask = options.PingAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, _) = await pingTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultPingAsync(IInfluxDBClient client, CancellationToken cancellationToken)
    {
        _ = cancellationToken; // InfluxDB client doesn't support cancellation token in PingAsync
        var pingResult = await client.PingAsync().ConfigureAwait(false);

        return pingResult;
    }
}
