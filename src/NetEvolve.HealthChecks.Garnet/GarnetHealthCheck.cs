namespace NetEvolve.HealthChecks.Garnet;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using global::Garnet.client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(GarnetOptions))]
internal sealed partial class GarnetHealthCheck : IDisposable
{
    private ConcurrentDictionary<string, GarnetClient>? _clients;
    private bool _disposedValue;

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        GarnetOptions options,
        CancellationToken cancellationToken
    )
    {
        var connection = GetConnection(name, options, _serviceProvider);

        if (!connection.IsConnected)
        {
            await connection.ConnectAsync(cancellationToken).ConfigureAwait(false);
        }

        var (isTimelyResponse, result) = await connection
            .PingAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (result is null || !result.Equals("pong", StringComparison.OrdinalIgnoreCase))
        {
            return HealthCheckUnhealthy(
                failureStatus,
                name,
                "The ping operation did not return the expected response."
            );
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    private GarnetClient GetConnection(string name, GarnetOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == ConnectionHandleMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<GarnetClient>();
        }

        _clients ??= new ConcurrentDictionary<string, GarnetClient>(StringComparer.OrdinalIgnoreCase);

        return _clients.GetOrAdd(
            name,
            _ =>
            {
                EndPoint endpoint = IPAddress.TryParse(options.Hostname!, out var ipAddress)
                    ? new IPEndPoint(ipAddress, options.Port)
                    : new DnsEndPoint(options.Hostname!, options.Port);

                return new GarnetClient(endpoint);
            }
        );
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
            if (disposing && _clients is not null)
            {
                _ = Parallel.ForEach(_clients.Values, clients => clients.Dispose());
                _clients.Clear();
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
