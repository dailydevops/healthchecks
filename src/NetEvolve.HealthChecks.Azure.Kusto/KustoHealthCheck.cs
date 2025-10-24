namespace NetEvolve.HealthChecks.Azure.Kusto;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using global::Azure.Core;
using global::Azure.Identity;
using global::Kusto.Data;
using global::Kusto.Data.Common;
using global::Kusto.Data.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class KustoHealthCheck : ConfigurableHealthCheckBase<KustoOptions>
{
    private ConcurrentDictionary<string, ICslQueryProvider>? _kustoClients;
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
        var kustoClient = GetKustoClient(name, options, _serviceProvider);

        var clientRequestProperties = new ClientRequestProperties { ClientRequestId = Guid.NewGuid().ToString() };

        var (isTimelyResponse, _) = await kustoClient
            .ExecuteQueryAsync(databaseName: null, query: ".show cluster", clientRequestProperties, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(options.DatabaseName))
        {
            return HealthCheckState(isTimelyResponse, name);
        }

        (isTimelyResponse, var reader) = await kustoClient
            .ExecuteQueryAsync(databaseName: null, query: ".show databases", clientRequestProperties, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        try
        {
            while (reader.Read())
            {
                var currentDatabaseName = reader.GetString(0);
                if (currentDatabaseName.Equals(options.DatabaseName, StringComparison.OrdinalIgnoreCase))
                {
                    return HealthCheckState(isTimelyResponse, name);
                }
            }

            return HealthCheckUnhealthy(failureStatus, name, $"Database '{options.DatabaseName}' not found");
        }
        finally
        {
            reader?.Dispose();
        }
    }

    internal ICslQueryProvider GetKustoClient<TOptions>(string name, TOptions options, IServiceProvider serviceProvider)
        where TOptions : class, IKustoOptions
    {
        if (options.Mode == KustoClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<ICslQueryProvider>();
        }

        if (_kustoClients is null)
        {
            _kustoClients = new ConcurrentDictionary<string, ICslQueryProvider>(StringComparer.OrdinalIgnoreCase);
        }

        return _kustoClients.GetOrAdd(name, _ => CreateKustoClient(options, serviceProvider));
    }

    internal static ICslQueryProvider CreateKustoClient<TOptions>(TOptions options, IServiceProvider serviceProvider)
        where TOptions : class, IKustoOptions
    {
        KustoConnectionStringBuilder connectionStringBuilder;

#pragma warning disable IDE0010 // Add missing cases
        switch (options.Mode)
        {
            case KustoClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                connectionStringBuilder = new KustoConnectionStringBuilder(
                    options.ClusterUri!.ToString()
                ).WithAadAzureTokenCredentialsAuthentication(tokenCredential);
                break;
            case KustoClientCreationMode.ConnectionString:
                connectionStringBuilder = new KustoConnectionStringBuilder(options.ConnectionString);
                break;
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
#pragma warning restore IDE0010 // Add missing cases

        if (options.ConfigureConnectionStringBuilder is not null)
        {
            options.ConfigureConnectionStringBuilder(connectionStringBuilder);
        }

        return KustoClientFactory.CreateCslQueryProvider(connectionStringBuilder);
    }
}
