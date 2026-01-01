namespace NetEvolve.HealthChecks.Apache.Pulsar;

using System.Threading;
using System.Threading.Tasks;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

/// <summary>
/// Health check implementation for Pulsar, using the <c>DotPulsar</c> package.
/// </summary>
[ConfigurableHealthCheck(typeof(PulsarOptions))]
internal sealed partial class PulsarHealthCheck
{
    /// <inheritdoc />
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        PulsarOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IPulsarClient>()
            : _serviceProvider.GetRequiredKeyedService<IPulsarClient>(options.KeyedService);

        // Create a temporary reader to test connectivity
        var (isTimelyResponse, result) = await Task.Run(
                async () =>
                {
                    IAsyncDisposable? reader = null;
                    try
                    {
                        reader = client
                            .NewReader(Schema.String)
                            .Topic("persistent://public/default/healthcheck")
                            .StartMessageId(MessageId.Earliest)
                            .Create();

                        return true;
                    }
                    finally
                    {
                        if (reader is not null)
                        {
                            await reader.DisposeAsync().ConfigureAwait(false);
                        }
                    }
                },
                cancellationToken
            )
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Failed to create a reader for the health check topic.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
