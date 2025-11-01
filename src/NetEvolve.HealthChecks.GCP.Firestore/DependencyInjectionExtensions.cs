namespace NetEvolve.HealthChecks.GCP.Firestore;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Extension methods for registering Firestore health checks.
/// </summary>
public static class DependencyInjectionExtensions
{
    private static readonly string[] _defaultTags = ["firestore", "gcp"];

    /// <summary>
    /// Adds a health check for Google Cloud Firestore.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the health check.</param>
    /// <param name="options">An optional action to configure the <see cref="FirestoreOptions"/>.</param>
    /// <param name="tags">An optional list of tags to associate with the health check.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <see langword="null"/> or empty.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tags"/> is <see langword="null"/>.</exception>
    public static IHealthChecksBuilder AddFirestore(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<FirestoreOptions>? options = null,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<FirestoreHealthCheckMarker>())
        {
            _ = builder
                .Services.AddSingleton<FirestoreHealthCheckMarker>()
                .AddSingleton<FirestoreHealthCheck>()
                .ConfigureOptions<FirestoreOptionsConfigure>();
        }

        builder.ThrowIfNameIsAlreadyUsed<FirestoreHealthCheck>(name);

        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }

        return builder.AddCheck<FirestoreHealthCheck>(
            name,
            HealthStatus.Unhealthy,
            _defaultTags.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class FirestoreHealthCheckMarker;
}
