namespace NetEvolve.HealthChecks.Redis;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SourceGenerator.Attributes;
using StackExchange.Redis;

[ConfigurableHealthCheck(typeof(RedisOptions))]
internal sealed partial class RedisHealthCheck
{
    private ConcurrentDictionary<string, IConnectionMultiplexer>? _connections;

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus _,
#pragma warning restore S1172 // Unused method parameters should be removed
        RedisOptions options,
        CancellationToken cancellationToken
    )
    {
        var connection = GetConnection(name, options, _serviceProvider);

        var result = await connection.GetDatabase().PingAsync().WaitAsync(cancellationToken).ConfigureAwait(false);

        return HealthCheckState(result.TotalMilliseconds <= options.Timeout, name);
    }

    private IConnectionMultiplexer GetConnection(string name, RedisOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == ConnectionHandleMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<IConnectionMultiplexer>();
        }

        _connections ??= new ConcurrentDictionary<string, IConnectionMultiplexer>(StringComparer.OrdinalIgnoreCase);

        return _connections.GetOrAdd(name, _ => ConnectionMultiplexer.Connect(options.ConnectionString!));
    }
}
