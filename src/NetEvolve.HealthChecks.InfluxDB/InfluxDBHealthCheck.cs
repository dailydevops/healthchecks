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
        HealthStatus failureStatus,
        InfluxDBOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IInfluxDBClient>()
            : _serviceProvider.GetRequiredKeyedService<IInfluxDBClient>(options.KeyedService);

        var pingTask = options.PingAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, result) = await pingTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Command execution failed.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultPingAsync(IInfluxDBClient client, CancellationToken _) =>
        await client.PingAsync().ConfigureAwait(false);
}
