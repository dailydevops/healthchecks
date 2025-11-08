namespace NetEvolve.HealthChecks.OpenSearch;

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using global::OpenSearch.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(OpenSearchOptions))]
internal sealed partial class OpenSearchHealthCheck
{
    private ConcurrentDictionary<string, OpenSearchClient>? _clients;

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        OpenSearchOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = GetClient(name, options, _serviceProvider);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, isResultValid) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return !isResultValid
            ? HealthCheckUnhealthy(failureStatus, name, "Invalid command result.")
            : HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(OpenSearchClient client, CancellationToken cancellationToken)
    {
        _ = await client.PingAsync(ct: cancellationToken).ConfigureAwait(false);

        return true;
    }

    private OpenSearchClient GetClient(string name, OpenSearchOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == OpenSearchClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<OpenSearchClient>()
                : serviceProvider.GetRequiredKeyedService<OpenSearchClient>(options.KeyedService);
        }

        _clients ??= new ConcurrentDictionary<string, OpenSearchClient>(StringComparer.OrdinalIgnoreCase);

        return _clients.GetOrAdd(name, _ => CreateClient(options));
    }

    private static OpenSearchClient CreateClient(OpenSearchOptions options)
    {
        var uri = new Uri(options.ConnectionStrings[0]);

#pragma warning disable CA2000 // Dispose objects before losing scope
        var settings = new ConnectionSettings(uri);
#pragma warning restore CA2000 // Dispose objects before losing scope

        _ = settings.ServerCertificateValidationCallback((o, certificate, chain, errors) => true);

        if (options.Username is not null && options.Password is not null)
        {
            _ = settings.BasicAuthentication(options.Username, options.Password);
        }

        return new OpenSearchClient(settings);
    }
}
