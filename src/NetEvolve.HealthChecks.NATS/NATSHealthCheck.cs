namespace NetEvolve.HealthChecks.NATS;

using System.Threading;
using System.Threading.Tasks;
using global::NATS.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

/// <summary>
/// Health check implementation for NATS, using the <c>NATS.Client</c> package.
/// </summary>
[ConfigurableHealthCheck(typeof(NatsOptions))]
internal sealed partial class NatsHealthCheck
{
    /// <inheritdoc />
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        NatsOptions options,
        CancellationToken cancellationToken
    )
    {
        var connection = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IConnection>()
            : _serviceProvider.GetRequiredKeyedService<IConnection>(options.KeyedService);

        var (isTimelyResponse, isConnected) = await Task.Run(
                () => connection.State == ConnState.CONNECTED,
                cancellationToken
            )
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!isConnected)
        {
            return HealthCheckUnhealthy(failureStatus, name, "NATS connection is not connected.");
        }

        return HealthCheckState(isTimelyResponse && isConnected, name);
    }
}
