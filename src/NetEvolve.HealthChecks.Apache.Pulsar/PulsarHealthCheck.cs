namespace NetEvolve.HealthChecks.Apache.Pulsar;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
#pragma warning disable S1172, RCS1163, IDE0060 // Unused method parameters should be removed
        HealthStatus _,
        PulsarOptions options,
        CancellationToken cancellationToken
#pragma warning restore S1172, RCS1163, IDE0060 // Unused method parameters should be removed
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IPulsarClient>()
            : _serviceProvider.GetRequiredKeyedService<IPulsarClient>(options.KeyedService);

        // Create a temporary reader to test connectivity
        var sw = Stopwatch.StartNew();
        var reader = client
            .NewReader(Schema.String)
            .Topic("persistent://public/default/healthcheck")
            .StartMessageId(MessageId.Earliest)
            .Create();

        var isTimelyResponse = options.Timeout >= sw.Elapsed.TotalMilliseconds;

        // If we can create the reader, the client is connected
        await reader.DisposeAsync().ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }
}
