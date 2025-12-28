namespace NetEvolve.HealthChecks.Apache.ActiveMq;

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using global::Apache.NMS;
using global::Apache.NMS.ActiveMQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(ActiveMqOptions))]
internal sealed partial class ActiveMqHealthCheck
{
    private readonly ConcurrentDictionary<string, ConnectionFactory> _factories = new(StringComparer.OrdinalIgnoreCase);

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        ActiveMqOptions options,
        CancellationToken cancellationToken
    )
    {
        using var client = await GetConnectionAsync(options, cancellationToken).ConfigureAwait(false);

        var isTimelyResponse = await client
            .StartAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!client.IsStarted)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Unable to start ActiveMQ connection.");
        }

        return HealthCheckState(isTimelyResponse, name);
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
