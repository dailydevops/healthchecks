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
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        MongoDbOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<MongoClient>()
            : _serviceProvider.GetRequiredKeyedService<MongoClient>(options.KeyedService);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, _) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<BsonDocument> DefaultCommandAsync(
        MongoClient client,
        CancellationToken cancellationToken
    )
    {
        var database = client.GetDatabase("admin");

        return await database.RunCommandAsync<BsonDocument>(_defaultCommand, null, cancellationToken).ConfigureAwait(false);
    }
}
