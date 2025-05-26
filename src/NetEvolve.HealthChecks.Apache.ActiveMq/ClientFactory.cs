namespace NetEvolve.HealthChecks.Apache.ActiveMq;

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using global::Apache.NMS;
using global::Apache.NMS.ActiveMQ;

internal static class ClientFactory
{
    private static readonly ConcurrentDictionary<string, ConnectionFactory> _factories = new(
        StringComparer.OrdinalIgnoreCase
    );

    public static async ValueTask<IConnection> GetConnectionAsync(
        ActiveMqOptions options,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.BrokerAddress);

        var factory = _factories.GetOrAdd(options.BrokerAddress, brokerAddress => new ConnectionFactory(brokerAddress));

        return options.Username is null && options.Password is null
            ? await factory.CreateConnectionAsync().WaitAsync(cancellationToken).ConfigureAwait(false)
            : await factory
                .CreateConnectionAsync(options.Username, options.Password)
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
    }
}
