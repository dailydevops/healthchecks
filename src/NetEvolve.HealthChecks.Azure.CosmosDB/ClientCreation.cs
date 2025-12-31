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

    internal CosmosClient GetCosmosClient(
        string name,
        CosmosDbAvailableOptions options,
        IServiceProvider serviceProvider
    )
    {
        if (options.Mode == CosmosDbClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<CosmosClient>();
        }

        _cosmosClients ??= new ConcurrentDictionary<string, CosmosClient>(StringComparer.OrdinalIgnoreCase);

        return _cosmosClients.GetOrAdd(name, _ => CreateCosmosClient(options, serviceProvider));
    }

    internal static CosmosClient CreateCosmosClient(
        CosmosDbAvailableOptions options,
        IServiceProvider serviceProvider
    )
    {
#pragma warning disable IDE0010 // Add missing cases
        switch (options.Mode)
        {
            case CosmosDbClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new CosmosClient(options.AccountEndpoint!.ToString(), tokenCredential);
            case CosmosDbClientCreationMode.ConnectionString:
                return new CosmosClient(options.ConnectionString);
            case CosmosDbClientCreationMode.AccountKey:
                return new CosmosClient(options.AccountEndpoint!.ToString(), options.AccountKey);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
#pragma warning restore IDE0010 // Add missing cases
    }
}
