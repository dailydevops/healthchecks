namespace NetEvolve.HealthChecks.Keycloak;

using System;
using System.Collections.Concurrent;
using global::Keycloak.Net;
using Microsoft.Extensions.DependencyInjection;

internal sealed class KeycloakClientProvider
{
    private ConcurrentDictionary<string, KeycloakClient>? _keycloakClients;

    internal KeycloakClient GetClient(KeycloakOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == KeycloakClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<KeycloakClient>()
                : serviceProvider.GetRequiredKeyedService<KeycloakClient>(options.KeyedService);
        }

        _keycloakClients ??= new ConcurrentDictionary<string, KeycloakClient>(StringComparer.OrdinalIgnoreCase);

        return _keycloakClients.GetOrAdd(
            $"{options.BaseAddress}:{options.Username}-{options.Password}",
            _ => CreateClient(options)
        );
    }

    internal static KeycloakClient CreateClient(KeycloakOptions options)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual((int)options.Mode, (int)KeycloakClientCreationMode.Internal);
        ArgumentNullException.ThrowIfNullOrEmpty(options.BaseAddress);
        ArgumentNullException.ThrowIfNull(options.Username);
        ArgumentNullException.ThrowIfNull(options.Password);

        return new KeycloakClient(options.BaseAddress, options.Username, options.Password);
    }
}
