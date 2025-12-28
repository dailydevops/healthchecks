namespace NetEvolve.HealthChecks.MongoDb;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(MongoDbOptions))]
internal sealed partial class MongoDbHealthCheck
{
    private static readonly BsonDocument _defaultCommand = new BsonDocument("ping", 1);

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        MongoDbOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<MongoClient>()
            : _serviceProvider.GetRequiredKeyedService<MongoClient>(options.KeyedService);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, result) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "The command did not return a valid result.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(MongoClient client, CancellationToken cancellationToken)
    {
        var database = client.GetDatabase("admin");

        _ = await database
            .RunCommandAsync<BsonDocument>(_defaultCommand, null, cancellationToken)
            .ConfigureAwait(false);

        return true;
    }
}
