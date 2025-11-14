namespace NetEvolve.HealthChecks.Mosquitto;

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SourceGenerator.Attributes;

/// <summary>
/// Extensions methods for <see cref="IHealthChecksBuilder"/> with Mosquitto Health Checks.
/// </summary>
[HealthCheckHelper]
public static partial class DependencyInjectionExtensions
{
    private static readonly string[] _defaultTags = ["mosquitto", "mqtt", "messaging"];

    /// <summary>
    /// Add a health check for Mosquitto.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the <see cref="MosquittoHealthCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddMosquitto(
        this IHealthChecksBuilder builder,
        string name,
        Action<MosquittoOptions>? options = null,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<MosquittoCheckMarker>())
        {
            _ = builder
                .Services.AddSingleton<MosquittoCheckMarker>()
                .AddSingleton<MosquittoHealthCheck>()
                .ConfigureOptions<MosquittoConfigure>();
        }

        builder.ThrowIfNameIsAlreadyUsed<MosquittoHealthCheck>(name);

        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }

        return builder.AddCheck<MosquittoHealthCheck>(
            name,
            HealthStatus.Unhealthy,
            _defaultTags.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class MosquittoCheckMarker;
}
