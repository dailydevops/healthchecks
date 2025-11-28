namespace NetEvolve.HealthChecks.Azure.Kusto;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Kusto.Data;
using global::Kusto.Data.Net.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(KustoAvailableOptions))]
internal sealed partial class KustoAvailableHealthCheck
{
    private static async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        KustoAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var connectionString = GetConnectionString(options);
        var kcsb = new KustoConnectionStringBuilder(connectionString);
        using var queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb);

        var databaseName = string.IsNullOrWhiteSpace(options.DatabaseName) ? "NetDefaultDB" : options.DatabaseName;

        var queryTask = queryProvider.ExecuteQueryAsync(databaseName, ".show databases", null, cancellationToken);

        var (isTimelyResponse, result) = await queryTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (result is null)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Kusto query returned no result.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    private static string GetConnectionString(KustoAvailableOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return options.ConnectionString;
        }

        if (options.ClusterUri is not null)
        {
            return options.ClusterUri.ToString();
        }

        throw new InvalidOperationException("Either ConnectionString or ClusterUri must be provided.");
    }
}
