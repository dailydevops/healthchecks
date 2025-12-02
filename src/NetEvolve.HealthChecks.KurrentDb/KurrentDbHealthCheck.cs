namespace NetEvolve.HealthChecks.KurrentDb;

using System.Threading.Tasks;
using KurrentDB.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(KurrentDbOptions))]
internal sealed partial class KurrentDbHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        KurrentDbOptions options,
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

        if (!result)
        {
            return HealthCheckUnhealthy(
                failureStatus,
                name,
                "The KurrentDB health check command returned a failed result."
            );
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(EventStoreClient client, CancellationToken cancellationToken)
    {
        try
        {
            var result = client.ReadAllAsync(
                Direction.Backwards,
                Position.End,
                maxCount: 1,
                cancellationToken: cancellationToken
            );

            // Refactored to avoid single-iteration loop: check if any message exists
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var enumerator = result.Messages.WithCancellation(cancellationToken).GetAsyncEnumerator();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            return await enumerator.MoveNextAsync();
        }
        catch
        {
            return false;
        }
    }
}
