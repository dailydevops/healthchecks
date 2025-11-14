namespace NetEvolve.HealthChecks.Azure.Tables;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using global::Azure;
using global::Azure.Core;
using global::Azure.Data.Tables;
using global::Azure.Identity;
using Microsoft.Extensions.DependencyInjection;

internal class ClientCreation
{
    private ConcurrentDictionary<string, TableServiceClient>? _tableServiceClients;

    internal TableServiceClient GetTableServiceClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, ITableOptions
    {
        if (options.Mode == TableClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<TableServiceClient>()
                : serviceProvider.GetRequiredKeyedService<TableServiceClient>(options.KeyedService);
        }

        if (_tableServiceClients is null)
        {
            _tableServiceClients = new ConcurrentDictionary<string, TableServiceClient>(
                StringComparer.OrdinalIgnoreCase
            );
        }

        return _tableServiceClients.GetOrAdd(name, _ => CreateTableServiceClient(options, serviceProvider));
    }

    internal static TableServiceClient CreateTableServiceClient<TOptions>(
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, ITableOptions
    {
        TableClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new TableClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

#pragma warning disable IDE0010 // Add missing cases
        switch (options.Mode)
        {
            case TableClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new TableServiceClient(options.ServiceUri, tokenCredential, clientOptions);
            case TableClientCreationMode.ConnectionString:
                return new TableServiceClient(options.ConnectionString, clientOptions);
            case TableClientCreationMode.SharedKey:
                var sharedKeyCredential = new TableSharedKeyCredential(options.AccountName, options.AccountKey);
                return new TableServiceClient(options.ServiceUri, sharedKeyCredential, clientOptions);
            case TableClientCreationMode.AzureSasCredential:
                var tableUriBuilder = new TableUriBuilder(options.ServiceUri) { Sas = null };
                var azureSasCredential = new AzureSasCredential(options.ServiceUri!.Query);

                return new TableServiceClient(tableUriBuilder.ToUri(), azureSasCredential, clientOptions);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
#pragma warning restore IDE0010 // Add missing cases
    }
}
