namespace NetEvolve.HealthChecks.Redis;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;
using StackExchange.Redis;

[ConfigurableHealthCheck(typeof(RedisOptions))]
internal sealed partial class RedisHealthCheck : IDisposable
{
    private ConcurrentDictionary<string, Task<IConnectionMultiplexer>>? _connections;
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
        var connection = await GetConnectionAsync(name, options, _serviceProvider).ConfigureAwait(false);

        var (isTimelyResponse, _) = await connection
            .GetDatabase()
            .PingAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }

    private Task<IConnectionMultiplexer> GetConnectionAsync(
        string name,
        RedisOptions options,
        IServiceProvider serviceProvider
    )
    {
        if (options.Mode == ConnectionHandleMode.ServiceProvider)
        {
            return Task.FromResult(serviceProvider.GetRequiredService<IConnectionMultiplexer>());
        }

        _connections ??= new ConcurrentDictionary<string, Task<IConnectionMultiplexer>>(
            StringComparer.OrdinalIgnoreCase
        );

        var connectionTask = _connections.GetOrAdd(name, _ => CreateConnectionAsync(options));

        // A failed connection attempt must not be cached, otherwise every subsequent health
        // check would immediately fail with the same exception, even after the server
        // becomes reachable again. Evict the entry as soon as it faults, so the next call
        // retries. The conditional remove targets this exact task instance, to avoid
        // accidentally removing a newer, still-valid entry created by a racing caller.
        if (connectionTask.IsFaulted)
        {
            RemoveFaultedConnection(name, connectionTask);
        }
        else if (!connectionTask.IsCompleted)
        {
            _ = connectionTask.ContinueWith(
                task => RemoveFaultedConnection(name, task),
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            );
        }

        return connectionTask;
    }

    private static async Task<IConnectionMultiplexer> CreateConnectionAsync(RedisOptions options) =>
        await ConnectionMultiplexer.ConnectAsync(options.ConnectionString!).ConfigureAwait(false);

    private void RemoveFaultedConnection(string name, Task<IConnectionMultiplexer> connectionTask) =>
        _ = ((ICollection<KeyValuePair<string, Task<IConnectionMultiplexer>>>?)_connections)?.Remove(
            new KeyValuePair<string, Task<IConnectionMultiplexer>>(name, connectionTask)
        );

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
                _ = Parallel.ForEach(
                    _connections.Values,
                    connectionTask =>
                    {
                        if (connectionTask.Status == TaskStatus.RanToCompletion)
                        {
#pragma warning disable VSTHRD002 // Task is already completed, so accessing Result cannot block.
                            connectionTask.Result.Dispose();
#pragma warning restore VSTHRD002
                        }
                    }
                );
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
