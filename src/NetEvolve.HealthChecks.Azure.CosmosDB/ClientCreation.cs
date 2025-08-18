namespace NetEvolve.HealthChecks.Azure.CosmosDB;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

internal class ClientCreation
{
    private ConcurrentDictionary<string, CosmosClient>? _cosmosClients;

    internal CosmosClient GetCosmosClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, ICosmosDbOptions
    {
        _cosmosClients ??= new ConcurrentDictionary<string, CosmosClient>();

        return _cosmosClients.GetOrAdd(name, _ => CreateCosmosClient(options, serviceProvider));
    }

    internal static CosmosClient CreateCosmosClient<TOptions>(
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, ICosmosDbOptions
    {
        CosmosClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new CosmosClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

        var mode = options.Mode ?? CosmosDbClientCreationMode.ConnectionString;

#pragma warning disable IDE0010 // Add missing cases
        switch (mode)
        {
            case CosmosDbClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new CosmosClient(options.ServiceEndpoint, tokenCredential, clientOptions);
            case CosmosDbClientCreationMode.ConnectionString:
                return new CosmosClient(options.ConnectionString, clientOptions);
            case CosmosDbClientCreationMode.AccountKey:
                return new CosmosClient(options.ServiceEndpoint, options.AccountKey, clientOptions);
            case CosmosDbClientCreationMode.ServicePrincipal:
                var servicePrincipalCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new CosmosClient(options.ServiceEndpoint, servicePrincipalCredential, clientOptions);
            default:
                throw new UnreachableException($"Invalid client creation mode `{mode}`.");
        }
#pragma warning restore IDE0010 // Add missing cases
    }
}