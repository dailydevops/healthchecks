namespace NetEvolve.HealthChecks.Azure.Kusto;

using System;
using System.Threading;
using System.Threading.Tasks;
using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class KustoHealthCheck : ConfigurableHealthCheckBase<KustoOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public KustoHealthCheck(IServiceProvider serviceProvider, IOptionsMonitor<KustoOptions> optionsMonitor)
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        KustoOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var kustoClient = clientCreation.GetKustoClient(name, options, _serviceProvider);

        var clientRequestProperties = new ClientRequestProperties { ClientRequestId = Guid.NewGuid().ToString() };

        var (isQuerySuccessful, _) = await ExecuteQueryWithTimeoutAsync(
            kustoClient,
            clientRequestProperties,
            options,
            cancellationToken
        );

        if (!isQuerySuccessful)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Failed to execute query against Kusto cluster");
        }

        // If a specific database name is provided, check if it exists
        if (!string.IsNullOrWhiteSpace(options.DatabaseName))
        {
            var (isDatabaseFound, _) = await CheckDatabaseExistsWithTimeoutAsync(
                kustoClient,
                options.DatabaseName,
                clientRequestProperties,
                options,
                cancellationToken
            );

            if (!isDatabaseFound)
            {
                return HealthCheckUnhealthy(failureStatus, name, $"Database '{options.DatabaseName}' not found");
            }
        }

        return HealthCheckState(true, name);
    }

    private static async ValueTask<(bool Success, Exception? Exception)> ExecuteQueryWithTimeoutAsync(
        ICslQueryProvider queryProvider,
        ClientRequestProperties clientRequestProperties,
        KustoOptions options,
        CancellationToken cancellationToken
    )
    {
        try
        {
            using var reader = await queryProvider
                .ExecuteQueryAsync(
                    databaseName: null,
                    query: ".show cluster",
                    clientRequestProperties,
                    cancellationToken
                )
                .WithTimeoutAsync(options.Timeout, cancellationToken)
                .ConfigureAwait(false);

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex);
        }
    }

    private static async ValueTask<(bool Found, Exception? Exception)> CheckDatabaseExistsWithTimeoutAsync(
        ICslQueryProvider queryProvider,
        string databaseName,
        ClientRequestProperties clientRequestProperties,
        KustoOptions options,
        CancellationToken cancellationToken
    )
    {
        try
        {
            using var reader = await queryProvider
                .ExecuteQueryAsync(
                    databaseName: null,
                    query: ".show databases",
                    clientRequestProperties,
                    cancellationToken
                )
                .WithTimeoutAsync(options.Timeout, cancellationToken)
                .ConfigureAwait(false);

            while (reader.Read())
            {
                var currentDatabaseName = reader.GetString(0);
                if (currentDatabaseName.Equals(databaseName, StringComparison.OrdinalIgnoreCase))
                {
                    return (true, null);
                }
            }

            return (false, null);
        }
        catch (Exception ex)
        {
            return (false, ex);
        }
    }
}
