namespace NetEvolve.HealthChecks.Elasticsearch;

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ElasticsearchHealthCheck : ConfigurableHealthCheckBase<ElasticsearchOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private ConcurrentDictionary<string, ElasticsearchClient>? _clients;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchHealthCheck"/> class.
    /// </summary>
    /// <param name="optionsMonitor">The <see cref="IOptionsMonitor{TOptions}"/> instance used to access named options.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to resolve dependencies.</param>
    public ElasticsearchHealthCheck(
        IOptionsMonitor<ElasticsearchOptions> optionsMonitor,
        IServiceProvider serviceProvider
    )
        : base(optionsMonitor)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus failureStatus,
        ElasticsearchOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = GetClient(name, options, _serviceProvider);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, isResultValid) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return !isResultValid
            ? HealthCheckUnhealthy(failureStatus, name, "Invalid command result.")
            : HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(
        ElasticsearchClient client,
        CancellationToken cancellationToken
    )
    {
        _ = await client.PingAsync(cancellationToken).ConfigureAwait(false);

        return true;
    }

    private ElasticsearchClient GetClient(string name, ElasticsearchOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == ElasticsearchClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<ElasticsearchClient>()
                : serviceProvider.GetRequiredKeyedService<ElasticsearchClient>(options.KeyedService);
        }

        _clients ??= new ConcurrentDictionary<string, ElasticsearchClient>(StringComparer.OrdinalIgnoreCase);

        return _clients.GetOrAdd(name, _ => CreateClient(options));
    }

    private static ElasticsearchClient CreateClient(ElasticsearchOptions options)
    {
        var nodes = options.ConnectionStrings.Select(connectionString => new Uri(connectionString)).ToArray();

#pragma warning disable CA2000 // Dispose objects before losing scope
        var settings =
            options.ConnectionStrings.Count == 1
                ? new ElasticsearchClientSettings(nodes[0])
                : new ElasticsearchClientSettings(new StaticNodePool(nodes));
#pragma warning restore CA2000 // Dispose objects before losing scope

        _ = settings.ServerCertificateValidationCallback(CertificateValidations.AllowAll);

        if (options.Username is not null && options.Password is not null)
        {
            _ = settings.Authentication(new BasicAuthentication(options.Username, options.Password));
        }

        return new ElasticsearchClient(settings);
    }
}
