namespace NetEvolve.HealthChecks.Azure.ServiceBus;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Messaging.ServiceBus;
using global::Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.DependencyInjection;

internal static class ClientCreation
{
    private static readonly ServiceBusClientOptions _clientOptions = new ServiceBusClientOptions();
    private static readonly ServiceBusAdministrationClientOptions _administrationClientOptions =
        new ServiceBusAdministrationClientOptions();

    private static readonly ConcurrentDictionary<string, ServiceBusClient> _serviceBusClients = new(
        StringComparer.OrdinalIgnoreCase
    );
    private static readonly ConcurrentDictionary<
        string,
        ServiceBusAdministrationClient
    > _serviceBusAdministrationClients = new(StringComparer.OrdinalIgnoreCase);

    static ClientCreation()
    {
        _clientOptions.RetryOptions.MaxRetries = 0;
        _administrationClientOptions.Retry.MaxRetries = 0;
    }

    internal static ServiceBusClient GetClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : ServiceBusOptionsBase
    {
        if (options.Mode == ClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<ServiceBusClient>();
        }

        return _serviceBusClients.GetOrAdd(name, _ => CreateClient(options, serviceProvider));
    }

    internal static ServiceBusAdministrationClient GetAdministrationClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : ServiceBusOptionsBase
    {
        if (options.Mode == ClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<ServiceBusAdministrationClient>();
        }

        return _serviceBusAdministrationClients.GetOrAdd(
            name,
            _ => CreateAdministrationClient(options, serviceProvider)
        );
    }

    private static ServiceBusClient CreateClient<TOptions>(TOptions options, IServiceProvider serviceProvider)
        where TOptions : ServiceBusOptionsBase =>
        options.Mode switch
        {
            ClientCreationMode.DefaultAzureCredentials => new ServiceBusClient(
                options.FullyQualifiedNamespace,
                serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential(),
                _clientOptions
            ),
            ClientCreationMode.ConnectionString => new ServiceBusClient(options.ConnectionString, _clientOptions),
            _ => throw new UnreachableException($"Invalid client creation mode `{options.Mode}`."),
        };

    private static ServiceBusAdministrationClient CreateAdministrationClient<TOptions>(
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : ServiceBusOptionsBase =>
        options.Mode switch
        {
            ClientCreationMode.DefaultAzureCredentials => new ServiceBusAdministrationClient(
                options.FullyQualifiedNamespace,
                serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential(),
                _administrationClientOptions
            ),
            ClientCreationMode.ConnectionString => new ServiceBusAdministrationClient(
                options.ConnectionString,
                _administrationClientOptions
            ),
            _ => throw new UnreachableException($"Invalid client creation mode `{options.Mode}`."),
        };
}
