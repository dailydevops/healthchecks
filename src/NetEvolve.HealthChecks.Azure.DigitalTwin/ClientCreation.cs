namespace NetEvolve.HealthChecks.Azure.DigitalTwin;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using global::Azure.Core;
using global::Azure.DigitalTwins.Core;
using global::Azure.Identity;
using Microsoft.Extensions.DependencyInjection;

internal class ClientCreation
{
    private ConcurrentDictionary<string, DigitalTwinsClient>? _digitalTwinsClients;

    internal DigitalTwinsClient GetDigitalTwinsClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, IDigitalTwinOptions
    {
        if (options.Mode == DigitalTwinClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<DigitalTwinsClient>();
        }

        if (_digitalTwinsClients is null)
        {
            _digitalTwinsClients = new ConcurrentDictionary<string, DigitalTwinsClient>(
                StringComparer.OrdinalIgnoreCase
            );
        }

        return _digitalTwinsClients.GetOrAdd(name, _ => CreateDigitalTwinsClient(options, serviceProvider));
    }

    internal static DigitalTwinsClient CreateDigitalTwinsClient<TOptions>(
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, IDigitalTwinOptions
    {
        DigitalTwinsClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new DigitalTwinsClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

        switch (options.Mode)
        {
            case DigitalTwinClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new DigitalTwinsClient(options.ServiceUri, tokenCredential, clientOptions);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
    }
}