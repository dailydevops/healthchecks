namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using global::Azure;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Search.Documents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(SearchAvailableOptions))]
internal sealed partial class SearchAvailableHealthCheck
{
    private ConcurrentDictionary<string, SearchClient>? _searchClients;

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        SearchAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var searchClient = GetSearchClient(name, options, _serviceProvider);

        var (isTimelyResponse, response) = await searchClient
            .GetDocumentCountAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!response.HasValue)
        {
            return HealthCheckUnhealthy(
                failureStatus,
                name,
                $"Azure Search index '{options.IndexName}' is not available."
            );
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    private SearchClient GetSearchClient(string name, SearchAvailableOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == ClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<SearchClient>()
                : serviceProvider.GetRequiredKeyedService<SearchClient>(options.KeyedService);
        }

        if (_searchClients is null)
        {
            _searchClients = new ConcurrentDictionary<string, SearchClient>(StringComparer.OrdinalIgnoreCase);
        }

        return _searchClients.GetOrAdd(name, _ => CreateSearchClient(options, serviceProvider));
    }

    private static SearchClient CreateSearchClient(SearchAvailableOptions options, IServiceProvider serviceProvider)
    {
        SearchClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new SearchClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

        switch (options.Mode)
        {
            case ClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new SearchClient(options.ServiceUri, options.IndexName, tokenCredential, clientOptions);
            case ClientCreationMode.AzureKeyCredential:
                var azureKeyCredential = new AzureKeyCredential(options.ApiKey!);
                return new SearchClient(options.ServiceUri, options.IndexName, azureKeyCredential, clientOptions);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
    }
}
