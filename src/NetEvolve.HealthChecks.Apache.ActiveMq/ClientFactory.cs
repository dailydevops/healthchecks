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
        var factory = _factories.GetOrAdd(options.BrokerAddress, brokerAddress => new ConnectionFactory(brokerAddress));

        return options.Username is null && options.Password is null
            ? await factory.CreateConnectionAsync().ConfigureAwait(false)
            : await factory.CreateConnectionAsync(options.Username, options.Password).ConfigureAwait(false);
    }
}
