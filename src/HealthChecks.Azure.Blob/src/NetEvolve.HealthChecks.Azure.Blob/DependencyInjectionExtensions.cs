namespace NetEvolve.HealthChecks.Azure.Blob;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Arguments;
using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Extensions methods for <see cref="IHealthChecksBuilder"/> with custom Health Checks.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds a health check for the Azure Blob Storage, to check the availability of this service.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>. This is prefixed internally with `AzureBlobAvailability` if not already present.</param>
    /// <param name="name">The name of the <see cref="BlobContainerAvailableHealthCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddAzureBlobAvailability(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<BlobContainerAvailableOptions>? options = null,
        params string[] tags
    )
    {
        Argument.ThrowIfNull(builder);
        Argument.ThrowIfNullOrEmpty(name);
        Argument.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<AzureBlobCheckMarker>())
        {
            _ = builder.Services
                .AddSingleton<AzureBlobCheckMarker>()
                .AddSingleton<BlobContainerAvailableHealthCheck>()
                .ConfigureOptions<BlobContainerAvailableConfigure>();
        }

        var internalName = name.EnsureStartsWith(
            "AzureBlobAvailability",
            StringComparison.OrdinalIgnoreCase
        );

        if (builder.IsNameAlreadyUsed(internalName))
        {
            throw new ArgumentException($"Name `{name}` already in use.", nameof(name), null);
        }

        if (options is not null)
        {
            _ = builder.Services.Configure(internalName, options);
        }

        return builder.AddCheck<BlobContainerAvailableHealthCheck>(
            internalName,
            HealthStatus.Unhealthy,
            tags
        );
    }

    private sealed partial class AzureBlobCheckMarker { }
}
