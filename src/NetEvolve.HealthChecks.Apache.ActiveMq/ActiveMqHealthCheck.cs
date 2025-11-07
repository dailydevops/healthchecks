namespace NetEvolve.HealthChecks.Apache.ActiveMq;

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using global::Apache.NMS;
using global::Apache.NMS.ActiveMQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(ActiveMqOptions))]
internal sealed partial class ActiveMqHealthCheck
{
    private readonly ConcurrentDictionary<string, ConnectionFactory> _factories = new(StringComparer.OrdinalIgnoreCase);

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        ActiveMqOptions options,
        CancellationToken cancellationToken
    )
    {
        using var client = await GetConnectionAsync(options, cancellationToken).ConfigureAwait(false);

        var isValid = await client
            .StartAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(client.IsStarted && isValid, name);
    }

    private async ValueTask<IConnection> GetConnectionAsync(
        ActiveMqOptions options,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.BrokerAddress);

        var factory = _factories.GetOrAdd(options.BrokerAddress, brokerAddress => new ConnectionFactory(brokerAddress));

        var createConnectionTask =
            options.Username is null && options.Password is null
                ? factory.CreateConnectionAsync()
                : factory.CreateConnectionAsync(options.Username, options.Password);

        return await createConnectionTask.WaitAsync(cancellationToken).ConfigureAwait(false);
    }
}
