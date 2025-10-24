namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Extensions methods for <see cref="IHealthChecksBuilder"/> with custom Health Checks.
/// </summary>
public static class DependencyInjectionExtensions
{
    private static readonly string[] _defaultTags = ["azure", "keyvault", "secrets"];

    /// <summary>
    /// Adds a health check for Azure Key Vault, to check the availability of the service.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the <see cref="KeyVaultSecretAvailableHealthCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddKeyVaultSecretAvailability(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<KeyVaultSecretAvailableOptions>? options = null,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<AzureKeyVaultCheckMarker>())
        {
            _ = builder
                .Services.AddSingleton<AzureKeyVaultCheckMarker>()
                .AddSingleton<KeyVaultSecretAvailableHealthCheck>()
                .ConfigureOptions<KeyVaultSecretAvailableConfigure>();

            builder.Services.TryAddSingleton<ClientCreation>();
        }

        builder.ThrowIfNameIsAlreadyUsed<KeyVaultSecretAvailableHealthCheck>(name);

        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }

        return builder.AddCheck<KeyVaultSecretAvailableHealthCheck>(
            name,
            HealthStatus.Unhealthy,
            _defaultTags.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class AzureKeyVaultCheckMarker;
}
