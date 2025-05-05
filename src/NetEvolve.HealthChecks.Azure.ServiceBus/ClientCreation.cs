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
    private static ConcurrentDictionary<string, ServiceBusClient>? _serviceBusClients;
    private static ConcurrentDictionary<string, ServiceBusAdministrationClient>? _serviceBusAdministrationClients;

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

        if (_serviceBusClients is null)
        {
            _serviceBusClients = new ConcurrentDictionary<string, ServiceBusClient>(StringComparer.OrdinalIgnoreCase);
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

        if (_serviceBusAdministrationClients is null)
        {
            _serviceBusAdministrationClients = new ConcurrentDictionary<string, ServiceBusAdministrationClient>(
                StringComparer.OrdinalIgnoreCase
            );
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
            ClientCreationMode.ServiceProvider => serviceProvider.GetRequiredService<ServiceBusClient>(),
            ClientCreationMode.DefaultAzureCredentials => new ServiceBusClient(
                options.FullyQualifiedNamespace,
                serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential()
            ),
            ClientCreationMode.ConnectionString => new ServiceBusClient(options.ConnectionString),
            _ => throw new UnreachableException($"Invalid client creation mode `{options.Mode}`."),
        };

    private static ServiceBusAdministrationClient CreateAdministrationClient<TOptions>(
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : ServiceBusOptionsBase =>
        options.Mode switch
        {
            ClientCreationMode.ServiceProvider => serviceProvider.GetRequiredService<ServiceBusAdministrationClient>(),
            ClientCreationMode.DefaultAzureCredentials => new ServiceBusAdministrationClient(
                options.FullyQualifiedNamespace,
                serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential()
            ),
            ClientCreationMode.ConnectionString => new ServiceBusAdministrationClient(options.ConnectionString),
            _ => throw new UnreachableException($"Invalid client creation mode `{options.Mode}`."),
        };
}
