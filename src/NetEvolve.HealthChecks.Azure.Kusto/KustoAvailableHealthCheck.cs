namespace NetEvolve.HealthChecks.Azure.Kusto;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Kusto.Data;
using global::Kusto.Data.Common;
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
        ICslQueryProvider? queryProvider = null;
        try
        {
            var connectionString = GetConnectionString(options);
            var kcsb = new KustoConnectionStringBuilder(connectionString);
            queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb);

            var query = ".show databases";
            var queryTask = queryProvider.ExecuteQueryAsync(
                options.DatabaseName ?? "NetDefaultDB",
                query,
                null,
                cancellationToken
            );

            var (isValid, result) = await queryTask
                .WithTimeoutAsync(options.Timeout, cancellationToken)
                .ConfigureAwait(false);

            return HealthCheckState(isValid && result is not null, name);
        }
        finally
        {
            queryProvider?.Dispose();
        }
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
