namespace NetEvolve.HealthChecks.GCP.Firestore;

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Arguments;

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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
    public static IHealthChecksBuilder AddFirestore(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<FirestoreOptions>? options = null,
        params string[] tags
    )
    {
        Argument.ThrowIfNull(builder);
        Argument.ThrowIfNullOrWhiteSpace(name);

        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }

        return builder
            .AddCheck<FirestoreHealthCheck>(
                name,
                failureStatus: HealthStatus.Unhealthy,
                tags: [.. _defaultTags, .. tags]
            )
            .ConfigureOptionsService<FirestoreOptions, FirestoreOptionsConfigure>();
    }
}
