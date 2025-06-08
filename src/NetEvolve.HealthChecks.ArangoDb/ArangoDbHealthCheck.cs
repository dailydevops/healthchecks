namespace NetEvolve.HealthChecks.ArangoDb;

using System;
using System.Threading.Tasks;
using ArangoDBNetStandard;
using ArangoDBNetStandard.CursorApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ArangoDbHealthCheck : ConfigurableHealthCheckBase<ArangoDbOptions>
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArangoDbHealthCheck"/> class.
    /// </summary>
    /// <param name="optionsMonitor">The <see cref="IOptionsMonitor{TOptions}"/> instance used to access named options.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to resolve dependencies.</param>
    public ArangoDbHealthCheck(IOptionsMonitor<ArangoDbOptions> optionsMonitor, IServiceProvider serviceProvider)
        : base(optionsMonitor)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        ArangoDbOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientProvider = _serviceProvider.GetRequiredService<ArangoDbClientProvider>();
        var client = clientProvider.GetClient(options, _serviceProvider);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isNotTimedOut, isResultValid) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isNotTimedOut && isResultValid, name);
    }

    internal static async Task<bool> DefaultCommandAsync(ArangoDBClient client, CancellationToken cancellationToken)
    {
        var cursor = await client
            .Cursor.PostCursorAsync<int>(new PostCursorBody { Query = "RETURN 1" }, null, cancellationToken)
            .ConfigureAwait(false);

        return cursor is not null && cursor.Result?.Any() == true;
    }
}
