namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using global::Azure;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Search.Documents;
using global::Azure.Search.Documents.Indexes;
using Microsoft.Extensions.DependencyInjection;

internal class ClientCreation
{
    private ConcurrentDictionary<string, SearchIndexClient>? _searchIndexClients;
    private ConcurrentDictionary<string, SearchClient>? _searchClients;

    internal SearchIndexClient GetSearchIndexClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, ISearchOptions
    {
        if (options.Mode == SearchIndexClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<SearchIndexClient>()
                : serviceProvider.GetRequiredKeyedService<SearchIndexClient>(options.KeyedService);
        }

        if (_searchIndexClients is null)
        {
            _searchIndexClients = new ConcurrentDictionary<string, SearchIndexClient>(StringComparer.OrdinalIgnoreCase);
        }

        return _searchIndexClients.GetOrAdd(name, _ => CreateSearchIndexClient(options, serviceProvider));
    }

    internal SearchClient GetSearchClient<TOptions>(string name, TOptions options, IServiceProvider serviceProvider)
        where TOptions : class, ISearchOptions
    {
        if (options.Mode == SearchIndexClientCreationMode.ServiceProvider)
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
            case SearchIndexClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new SearchIndexClient(options.ServiceUri, tokenCredential, clientOptions);
            case SearchIndexClientCreationMode.AzureKeyCredential:
                var azureKeyCredential = new AzureKeyCredential(options.ApiKey!);
                return new SearchIndexClient(options.ServiceUri, azureKeyCredential, clientOptions);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
#pragma warning restore IDE0010 // Add missing cases
    }

    internal static SearchClient CreateSearchClient<TOptions>(TOptions options, IServiceProvider serviceProvider)
        where TOptions : class, ISearchOptions
    {
        if (options is not SearchIndexAvailableOptions indexOptions)
        {
            throw new InvalidOperationException("SearchClient can only be created with SearchIndexAvailableOptions.");
        }

        SearchClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new SearchClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

#pragma warning disable IDE0010 // Add missing cases
        switch (options.Mode)
        {
            case SearchIndexClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new SearchClient(options.ServiceUri, indexOptions.IndexName, tokenCredential, clientOptions);
            case SearchIndexClientCreationMode.AzureKeyCredential:
                var azureKeyCredential = new AzureKeyCredential(options.ApiKey!);
                return new SearchClient(options.ServiceUri, indexOptions.IndexName, azureKeyCredential, clientOptions);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
#pragma warning restore IDE0010 // Add missing cases
    }
}
