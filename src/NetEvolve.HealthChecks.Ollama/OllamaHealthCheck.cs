namespace NetEvolve.HealthChecks.Ollama;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using OllamaSharp;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(OllamaOptions))]
internal sealed partial class OllamaHealthCheck : IDisposable
{
    private ConcurrentDictionary<string, OllamaApiClient>? _clients;
    private bool _disposedValue;

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        OllamaOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = GetClient(name, options);

        var (isTimelyResponse, isRunning) = await client
            .IsRunningAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!isRunning)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Ollama service is not running.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    private OllamaApiClient GetClient(string name, OllamaOptions options)
    {
        if (options.ClientMode == ClientMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? _serviceProvider.GetRequiredService<OllamaApiClient>()
                : _serviceProvider.GetRequiredKeyedService<OllamaApiClient>(options.KeyedService);
        }

        _clients ??= new ConcurrentDictionary<string, OllamaApiClient>(StringComparer.OrdinalIgnoreCase);

        return _clients.GetOrAdd(name, _ => new OllamaApiClient(options.Uri!));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
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
                _ = Parallel.ForEach(_clients.Values, client => client.Dispose());
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
