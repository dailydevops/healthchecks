namespace NetEvolve.HealthChecks.Tests.Integration.Ollama;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Ollama;

public sealed class OllamaContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly Testcontainers.Ollama.OllamaContainer _container = new OllamaBuilder(
        /*dockerimage*/"ollama/ollama:0.6.6"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public string BaseAddress => _container.GetBaseAddress();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
