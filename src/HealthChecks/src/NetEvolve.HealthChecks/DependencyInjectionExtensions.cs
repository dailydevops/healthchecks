namespace NetEvolve.HealthChecks;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Arguments;

/// <summary>
/// Extensions methods for <see cref="IHealthChecksBuilder"/> with custom Health Checks.
/// </summary>
public static class DependencyInjectionExtensions
{
    private static readonly string[] _defaultTagsReadiness = new[] { "self", "readiness" };
    private static readonly string[] _defaultTagsHealthy = new[] { "self" };

    /// <summary>
    /// Add a health check for the application readiness.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddApplicationReady(
        [NotNull] this IHealthChecksBuilder builder,
        params string[] tags
    )
    {
        Argument.ThrowIfNull(builder);
        Argument.ThrowIfNull(tags);

        if (builder.IsServiceTypeRegistered<ApplicationReadyCheckMarker>())
        {
            return builder;
        }

        _ = builder
            .Services.AddSingleton<ApplicationReadyCheckMarker>()
            .AddSingleton<ApplicationReadyCheck>();
        return builder.AddCheck<ApplicationReadyCheck>(
            "ApplicationReady",
            HealthStatus.Unhealthy,
            _defaultTagsReadiness.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Add a health check for the application, always <see cref="HealthStatus.Healthy"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddApplicationHealthy(
        [NotNull] this IHealthChecksBuilder builder,
        params string[] tags
    )
    {
        Argument.ThrowIfNull(builder);
        Argument.ThrowIfNull(tags);

        if (builder.IsServiceTypeRegistered<ApplicationHealthyCheckMarker>())
        {
            return builder;
        }

        _ = builder
            .Services.AddSingleton<ApplicationHealthyCheckMarker>()
            .AddSingleton<ApplicationHealthyCheck>();
        return builder.AddCheck<ApplicationHealthyCheck>(
            "ApplicationHealthy",
            HealthStatus.Unhealthy,
            _defaultTagsHealthy.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class ApplicationReadyCheckMarker { }

    private sealed partial class ApplicationHealthyCheckMarker { }
}
