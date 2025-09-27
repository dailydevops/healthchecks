namespace NetEvolve.HealthChecks.Azure.IotHub;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using global::Azure.Core;
using global::Azure.Identity;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.DependencyInjection;

internal sealed class IotHubClientFactory
{
    private readonly ConcurrentDictionary<string, RegistryManager> _registryManagers = new(
        StringComparer.OrdinalIgnoreCase
    );
    private readonly ConcurrentDictionary<string, ServiceClient> _serviceClients = new(
        StringComparer.OrdinalIgnoreCase
    );

    internal RegistryManager GetRegistryManager<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : IotHubOptionsBase
    {
        if (options.Mode == ClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<RegistryManager>();
        }

        return _registryManagers.GetOrAdd(name, _ => CreateRegistryManager(options, serviceProvider));
    }

    internal ServiceClient GetServiceClient<TOptions>(string name, TOptions options, IServiceProvider serviceProvider)
        where TOptions : IotHubOptionsBase
    {
        if (options.Mode == ClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<ServiceClient>();
        }

        return _serviceClients.GetOrAdd(name, _ => CreateServiceClient(options, serviceProvider));
    }

    private static RegistryManager CreateRegistryManager<TOptions>(TOptions options, IServiceProvider serviceProvider)
        where TOptions : IotHubOptionsBase =>
        options.Mode switch
        {
            ClientCreationMode.DefaultAzureCredentials => RegistryManager.Create(
                options.FullyQualifiedHostname!,
                serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential()
            ),
            ClientCreationMode.ConnectionString => RegistryManager.CreateFromConnectionString(
                options.ConnectionString!
            ),
            _ => throw new UnreachableException($"Invalid client creation mode `{options.Mode}`."),
        };

    private static ServiceClient CreateServiceClient<TOptions>(TOptions options, IServiceProvider serviceProvider)
        where TOptions : IotHubOptionsBase =>
        options.Mode switch
        {
            ClientCreationMode.DefaultAzureCredentials => ServiceClient.Create(
                options.FullyQualifiedHostname!,
                serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential()
            ),
            ClientCreationMode.ConnectionString => ServiceClient.CreateFromConnectionString(options.ConnectionString!),
            _ => throw new UnreachableException($"Invalid client creation mode `{options.Mode}`."),
        };
}
