namespace NetEvolve.HealthChecks.Apache.Solr;

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SolrNet;
using SolrNet.Impl;
using SourceGenerator.Attributes;

/// <summary>
/// Executes a ping-based health check against Apache Solr instances.
/// </summary>
[ConfigurableHealthCheck(typeof(SolrOptions))]
internal sealed partial class SolrHealthCheck
{
    private ConcurrentDictionary<string, ISolrBasicReadOnlyOperations<string>>? _clients;

    /// <summary>
    /// Executes the health check for the configured Solr instance.
    /// </summary>
    /// <param name="name">The health check registration name.</param>
    /// <param name="failureStatus">The status to report when the check is unhealthy.</param>
    /// <param name="options">The Solr configuration options.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="HealthCheckResult"/> indicating the Solr availability.</returns>
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        SolrOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = GetClient(name, options, serviceProvider);

        var (isTimelyResponse, response) = await client
            .PingAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (response.Status != 0)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"Solr ping returned non-zero status: {response.Status}.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    /// <summary>
    /// Resolves or creates an <see cref="ISolrBasicReadOnlyOperations{T}"/> client based on the configured creation mode.
    /// </summary>
    /// <param name="name">The health check registration name.</param>
    /// <param name="options">The Solr configuration options.</param>
    /// <param name="serviceProvider">The service provider used to resolve services.</param>
    /// <returns>A ready-to-use Solr client.</returns>
    private ISolrBasicReadOnlyOperations<string> GetClient(
        string name,
        SolrOptions options,
        IServiceProvider serviceProvider
    )
    {
        if (options.CreationMode == ClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<ISolrBasicReadOnlyOperations<string>>();
        }

        _clients ??= new ConcurrentDictionary<string, ISolrBasicReadOnlyOperations<string>>(
            StringComparer.OrdinalIgnoreCase
        );

        return _clients.GetOrAdd(name, _ => CreateClient(options));
    }

    /// <summary>
    /// Creates a new Solr client for direct connections using the configured base URL.
    /// </summary>
    /// <param name="options">The Solr configuration options.</param>
    /// <returns>A new <see cref="SolrBasicServer{T}"/> instance.</returns>
    private static SolrBasicServer<string> CreateClient(SolrOptions options)
    {
        var connection = new SolrConnection(options.BaseUrl);

        return new SolrBasicServer<string>(
            connection,
            queryExecuter: null!,
            documentSerializer: null!,
            schemaParser: null!,
            headerParser: null!,
            querySerializer: null!,
            dihStatusParser: null!,
            extractResponseParser: null!
        );
    }
}
