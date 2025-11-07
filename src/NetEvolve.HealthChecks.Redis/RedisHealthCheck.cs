namespace NetEvolve.HealthChecks.Redis;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SourceGenerator.Attributes;
using StackExchange.Redis;

[ConfigurableHealthCheck(typeof(RedisOptions))]
internal sealed partial class RedisHealthCheck : IDisposable
{
    private ConcurrentDictionary<string, IConnectionMultiplexer>? _connections;
    private bool _disposedValue;

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

    [SuppressMessage(
        "Blocker Code Smell",
        "S2953:Methods named \"Dispose\" should implement \"IDisposable.Dispose\"",
        Justification = "As designed."
    )]
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing && _connections is not null)
            {
                _ = Parallel.ForEach(_connections.Values, connection => connection.Dispose());
                _connections.Clear();
            }
            _disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
