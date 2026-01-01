namespace NetEvolve.HealthChecks.RabbitMQ;

using System.Threading;
using System.Threading.Tasks;
using global::RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

/// <summary>
/// Health check implementation for RabbitMQ, using the <c>RabbitMQ.Client</c> package.
/// </summary>
[ConfigurableHealthCheck(typeof(RabbitMQOptions))]
internal sealed partial class RabbitMQHealthCheck
{
    /// <inheritdoc />
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        RabbitMQOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IConnection>()
            : _serviceProvider.GetRequiredKeyedService<IConnection>(options.KeyedService);

        var (isTimelyResponse, channel) = await client
            .CreateChannelAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (channel?.IsOpen != true)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Failed to create a channel to RabbitMQ.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
