﻿namespace NetEvolve.HealthChecks.Redis;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using StackExchange.Redis;

internal sealed class RedisHealthCheck : ConfigurableHealthCheckBase<RedisOptions>
{
    private ConcurrentDictionary<string, IConnectionMultiplexer>? _connections;
    private readonly IServiceProvider _serviceProvider;

    public RedisHealthCheck(IServiceProvider serviceProvider, IOptionsMonitor<RedisOptions> optionsMonitor)
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        RedisOptions options,
        CancellationToken cancellationToken
    )
    {
        var connection = GetConnection(name, options, _serviceProvider);

        var result = await connection.GetDatabase().PingAsync().ConfigureAwait(false);
        var isDegraded = IsDegraded(result);

        return isDegraded
            ? HealthCheckResult.Degraded($"{name}: Degraded")
            : HealthCheckResult.Healthy($"{name}: Healthy");

        bool IsDegraded(TimeSpan elapsedTime) => elapsedTime.TotalMilliseconds >= options.Timeout;
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
