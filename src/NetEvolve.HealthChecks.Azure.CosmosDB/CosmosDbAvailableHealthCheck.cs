namespace NetEvolve.HealthChecks.Azure.CosmosDB;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using global::Azure.Core;
using global::Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(CosmosDbAvailableOptions))]
internal sealed partial class CosmosDbAvailableHealthCheck : IDisposable
{
    private ConcurrentDictionary<string, CosmosClient>? _cosmosClients;
    private bool _disposedValue;

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CosmosDbAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var cosmosClient = GetCosmosClient(name, options, _serviceProvider);

        var readTask = ReadAccountPropertiesAsync(cosmosClient, options, cancellationToken);

        var (isValid, result) = await readTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "CosmosDB is not available.");
        }

        return HealthCheckState(isValid, name);
    }

    private static async Task<bool> ReadAccountPropertiesAsync(
        Microsoft.Azure.Cosmos.CosmosClient client,
        CosmosDbAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        _ = await client.ReadAccountAsync().ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(options.DatabaseId))
        {
            try
            {
                var database = client.GetDatabase(options.DatabaseId);
                var response = await database.ReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        return true;
    }

    private CosmosClient GetCosmosClient(
        string name,
        CosmosDbAvailableOptions options,
        IServiceProvider serviceProvider
    )
    {
        if (options.Mode == CosmosDbClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<CosmosClient>()
                : serviceProvider.GetRequiredKeyedService<CosmosClient>(options.KeyedService);
        }

        _cosmosClients ??= new ConcurrentDictionary<string, CosmosClient>(StringComparer.OrdinalIgnoreCase);

        return _cosmosClients.GetOrAdd(name, _ => CreateCosmosClient(options, serviceProvider));
    }

    internal static CosmosClient CreateCosmosClient(CosmosDbAvailableOptions options, IServiceProvider serviceProvider)
    {
        var clientOptions = new CosmosClientOptions();
        options.ClientConfiguration?.Invoke(clientOptions);

#pragma warning disable IDE0010 // Add missing cases
        switch (options.Mode)
        {
            case CosmosDbClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new CosmosClient(options.AccountEndpoint!.ToString(), tokenCredential, clientOptions);
            case CosmosDbClientCreationMode.ConnectionString:
                return new CosmosClient(options.ConnectionString, clientOptions);
            case CosmosDbClientCreationMode.AccountKey:
                return new CosmosClient(options.AccountEndpoint!.ToString(), options.AccountKey, clientOptions);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
#pragma warning restore IDE0010 // Add missing cases
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing && _cosmosClients is not null)
            {
                _ = Parallel.ForEach(_cosmosClients.Values, client => client.Dispose());
                _cosmosClients.Clear();
                _cosmosClients = null;
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
