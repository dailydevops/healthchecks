namespace NetEvolve.HealthChecks.Ollama;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OllamaSharp;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(OllamaOptions))]
internal sealed partial class OllamaHealthCheck : IDisposable
{
    private ConcurrentDictionary<string, OllamaApiClient>? _clients;
    private bool _disposedValue;

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus _,
#pragma warning restore S1172 // Unused method parameters should be removed
        OllamaOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = GetClient(name, options);

        var stopwatch = Stopwatch.StartNew();
        var isRunning = await client.IsRunningAsync(cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();

        return HealthCheckState(isRunning && stopwatch.ElapsedMilliseconds <= options.Timeout, name);
    }

    private OllamaApiClient GetClient(string name, OllamaOptions options)
    {
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
                foreach (var client in _clients.Values)
                {
                    client.Dispose();
                }
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
