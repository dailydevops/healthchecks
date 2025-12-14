namespace NetEvolve.HealthChecks.Meilisearch;

using System.Collections.Concurrent;
using System.Threading.Tasks;
using global::Meilisearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(MeilisearchOptions))]
internal sealed partial class MeilisearchHealthCheck : IDisposable
{
    private bool _disposedValue;
    private ConcurrentDictionary<string, MeilisearchClient>? _meilisearchClients;

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        MeilisearchOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = GetClient(name, options, _serviceProvider);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, isResultValid) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!isResultValid)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Received an invalid response from Meilisearch.");
        }

        return HealthCheckState(isTimelyResponse && isResultValid, name);
    }

    internal static async Task<bool> DefaultCommandAsync(
        MeilisearchClient client,
        CancellationToken cancellationToken
    ) => await client.IsHealthyAsync(cancellationToken).ConfigureAwait(false);

    private MeilisearchClient GetClient(string name, MeilisearchOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == MeilisearchClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<MeilisearchClient>()
                : serviceProvider.GetRequiredKeyedService<MeilisearchClient>(options.KeyedService);
        }

        _meilisearchClients ??= new ConcurrentDictionary<string, MeilisearchClient>(StringComparer.OrdinalIgnoreCase);

        return _meilisearchClients.GetOrAdd(name, _ => CreateClient(options));
    }

    private static MeilisearchClient CreateClient(MeilisearchOptions options) =>
        new MeilisearchClient(options.Host, options.ApiKey);

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing && _meilisearchClients is not null)
            {
                _meilisearchClients.Clear();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
