namespace NetEvolve.HealthChecks.Apache.ActiveMq;

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using global::Apache.NMS;
using global::Apache.NMS.ActiveMQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ActiveMqHealthCheck : ConfigurableHealthCheckBase<ActiveMqOptions>
{
    private readonly ConcurrentDictionary<string, ConnectionFactory> _factories = new(StringComparer.OrdinalIgnoreCase);

    public ActiveMqHealthCheck(IOptionsMonitor<ActiveMqOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
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
