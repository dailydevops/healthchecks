namespace NetEvolve.HealthChecks.Azure.Cosmos;

using System;
using System.Collections.Concurrent;
using global::Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

internal static class ClientCreation
{
    private static ConcurrentDictionary<string, CosmosClient>? _cosmosClients;

    internal static CosmosClient GetCosmosClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, ICosmosOptions
    {
        if (options.Mode == CosmosClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<CosmosClient>();
        }

        if (_cosmosClients is null)
        {
            _cosmosClients = new ConcurrentDictionary<string, CosmosClient>(StringComparer.OrdinalIgnoreCase);
        }

        return _cosmosClients.GetOrAdd(name, _ => CreateCosmosClient(options, serviceProvider));
    }

    internal static CosmosClient CreateCosmosClient<TOptions>(TOptions options, IServiceProvider serviceProvider)
        where TOptions : class, ICosmosOptions
    {
        CosmosClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new CosmosClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

        switch (options.Mode)
        {
            case CosmosClientCreationMode.DefaultAzureCredentials:
                var tokenCredential =
                    serviceProvider.GetService<global::Azure.Core.TokenCredential>() ?? new DefaultAzureCredential();
                return new CosmosClient(options.EndpointUri?.ToString(), tokenCredential, clientOptions);

            case CosmosClientCreationMode.ConnectionString:
                return new CosmosClient(options.ConnectionString, clientOptions);

            default:
                if (options.EndpointUri is null)
                {
                    throw new InvalidOperationException("EndpointUri cannot be null.");
                }
                return new CosmosClient(options.EndpointUri.ToString(), options.PrimaryKey, clientOptions);
        }
    }
}
