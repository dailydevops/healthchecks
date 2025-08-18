namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using global::Azure;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Search.Documents;
using global::Azure.Search.Documents.Indexes;
using Microsoft.Extensions.DependencyInjection;

internal class ClientCreation
{
    private ConcurrentDictionary<string, SearchIndexClient>? _searchIndexClients;

    internal SearchIndexClient GetSearchIndexClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, ISearchOptions
    {
        _searchIndexClients ??= new ConcurrentDictionary<string, SearchIndexClient>();
        return _searchIndexClients.GetOrAdd(name, _ => CreateSearchIndexClient(options, serviceProvider));
    }

    internal static SearchIndexClient CreateSearchIndexClient<TOptions>(
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, ISearchOptions
    {
        SearchClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new SearchClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

#pragma warning disable IDE0010 // Add missing cases
        switch (options.Mode)
        {
            case SearchClientCreationMode.ServiceProvider:
                return serviceProvider.GetRequiredService<SearchIndexClient>();
            case SearchClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new SearchIndexClient(options.ServiceUri!, tokenCredential, clientOptions);
            case SearchClientCreationMode.ConnectionString:
                // For Azure Search, connection string typically contains endpoint and key
                // Parse the connection string to extract endpoint and key
                var connectionParts = options.ConnectionString!.Split(';');
                var endpoint = connectionParts.FirstOrDefault(p => p.StartsWith("Endpoint=", StringComparison.OrdinalIgnoreCase))?.Substring(9);
                var key = connectionParts.FirstOrDefault(p => p.StartsWith("ApiKey=", StringComparison.OrdinalIgnoreCase))?.Substring(7);
                
                if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(key))
                {
                    throw new InvalidOperationException("Connection string must contain both 'Endpoint' and 'ApiKey' parameters.");
                }
                
                var connectionKeyCredential = new AzureKeyCredential(key);
                return new SearchIndexClient(new Uri(endpoint), connectionKeyCredential, clientOptions);
            case SearchClientCreationMode.ApiKey:
                var keyCredential = new AzureKeyCredential(options.ApiKey!);
                return new SearchIndexClient(options.ServiceUri!, keyCredential, clientOptions);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
#pragma warning restore IDE0010 // Add missing cases
    }
}