namespace NetEvolve.HealthChecks.ArangoDb;

using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using ArangoDBNetStandard;
using ArangoDBNetStandard.CursorApi.Models;
using ArangoDBNetStandard.Transport.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(ArangoDbOptions))]
internal sealed partial class ArangoDbHealthCheck
{
    private ConcurrentDictionary<string, ArangoDBClient>? _arangoDbClients;

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        ArangoDbOptions options,
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
            return HealthCheckUnhealthy(failureStatus, name, "The command did not return a valid result.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(ArangoDBClient client, CancellationToken cancellationToken)
    {
        var cursor = await client
            .Cursor.PostCursorAsync<int>(new PostCursorBody { Query = "RETURN 1" }, null, cancellationToken)
            .ConfigureAwait(false);

        return cursor?.Result?.Any() == true;
    }

    private ArangoDBClient GetClient(string name, ArangoDbOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == ArangoDbClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<ArangoDBClient>()
                : serviceProvider.GetRequiredKeyedService<ArangoDBClient>(options.KeyedService);
        }

        _arangoDbClients ??= new ConcurrentDictionary<string, ArangoDBClient>(StringComparer.OrdinalIgnoreCase);

        return _arangoDbClients.GetOrAdd(name, _ => CreateClient(options));
    }

    private static ArangoDBClient CreateClient(ArangoDbOptions options)
    {
        var addressUri = new Uri(options.TransportAddress!);

        var transport =
            !string.IsNullOrWhiteSpace(options.Password) && !string.IsNullOrWhiteSpace(options.Username)
                ? HttpApiTransport.UsingBasicAuth(addressUri, options.Username, options.Password)
                : HttpApiTransport.UsingNoAuth(addressUri);

        return new ArangoDBClient(transport);
    }
}
