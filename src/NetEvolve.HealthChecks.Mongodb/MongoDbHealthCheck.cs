namespace NetEvolve.HealthChecks.MongoDb;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class MongoDbHealthCheck : ConfigurableHealthCheckBase<MongoDbOptions>
{
    public MongoDbHealthCheck(IOptionsMonitor<MongoDbOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        MongoDbOptions options,
        CancellationToken cancellationToken
    )
    {
        using var client = new MongoClient(options.ConnectionString); // Ensure IDisposable is properly handled
        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isHealthy, result) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isHealthy && result is not null, name);
    }

    internal static async Task<BsonDocument> DefaultCommandAsync(
        MongoClient client,
        CancellationToken cancellationToken
    )
    {
        var database = client.GetDatabase("admin");
        var command = new BsonDocument("ping", 1);

        return await database.RunCommandAsync<BsonDocument>(command, null, cancellationToken).ConfigureAwait(false);
    }
}
