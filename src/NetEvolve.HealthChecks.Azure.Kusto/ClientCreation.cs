namespace NetEvolve.HealthChecks.Azure.Kusto;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Azure.Core;
using Azure.Identity;
using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using Microsoft.Extensions.DependencyInjection;

internal class ClientCreation
{
    private ConcurrentDictionary<string, ICslQueryProvider>? _kustoClients;

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
                ).WithAadUserPromptAuthentication();
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
