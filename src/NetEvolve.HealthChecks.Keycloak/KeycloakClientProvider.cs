namespace NetEvolve.HealthChecks.Keycloak;

using System;
using System.Collections.Concurrent;
using global::Keycloak.Net;
using Microsoft.Extensions.DependencyInjection;

internal sealed class KeycloakClientProvider
{
    private ConcurrentDictionary<string, KeycloakClient>? _keycloakClients;

    internal KeycloakClient GetClient(string name, KeycloakOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == KeycloakClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<KeycloakClient>()
                : serviceProvider.GetRequiredKeyedService<KeycloakClient>(options.KeyedService);
        }

        _keycloakClients ??= new ConcurrentDictionary<string, KeycloakClient>(StringComparer.OrdinalIgnoreCase);

        return _keycloakClients.GetOrAdd(name, _ => CreateClient(options));
    }

    internal static KeycloakClient CreateClient(KeycloakOptions options)
    {
        ArgumentException.ThrowIfNullOrEmpty(options.BaseAddress);

        return options.Mode switch
        {
            KeycloakClientCreationMode.UsernameAndPassword => CreateClientWithUsernameAndPassword(options),
            KeycloakClientCreationMode.ClientSecret => CreateClientWithClientSecret(options),
            _ => throw new ArgumentOutOfRangeException(nameof(options), options.Mode, "The mode is not supported."),
        };
    }

    private static KeycloakClient CreateClientWithUsernameAndPassword(KeycloakOptions options)
    {
        ArgumentNullException.ThrowIfNull(options.Username);
        ArgumentNullException.ThrowIfNull(options.Password);

        return new KeycloakClient(options.BaseAddress!, options.Username, options.Password);
    }

    private static KeycloakClient CreateClientWithClientSecret(KeycloakOptions options)
    {
        ArgumentNullException.ThrowIfNull(options.ClientSecret);

        return new KeycloakClient(options.BaseAddress!, options.ClientSecret);
    }
}
