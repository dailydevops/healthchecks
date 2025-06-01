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
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbHealthCheck"/> class.
    /// </summary>
    /// <param name="optionsMonitor">The <see cref="IOptionsMonitor{TOptions}"/> instance used to access named options.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to resolve dependencies.</param>
    public MongoDbHealthCheck(IOptionsMonitor<MongoDbOptions> optionsMonitor, IServiceProvider serviceProvider)
        : base(optionsMonitor)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
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
