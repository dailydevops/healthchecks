namespace NetEvolve.HealthChecks.EventStoreDb;

using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(EventStoreDbOptions))]
internal sealed partial class EventStoreDbHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus _,
#pragma warning restore S1172 // Unused method parameters should be removed
        EventStoreDbOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<EventStoreClient>()
            : _serviceProvider.GetRequiredKeyedService<EventStoreClient>(options.KeyedService);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, result) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse && result, name);
    }

    internal static async Task<bool> DefaultCommandAsync(EventStoreClient client, CancellationToken cancellationToken)
    {
        try
        {
            var readStream = client.ReadStreamAsync(
                Direction.Backwards,
                "$all",
                StreamPosition.End,
                maxCount: 1,
                resolveLinkTos: false,
                cancellationToken: cancellationToken
            );

            await foreach (var _ in readStream.ConfigureAwait(false))
            {
                return true;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
