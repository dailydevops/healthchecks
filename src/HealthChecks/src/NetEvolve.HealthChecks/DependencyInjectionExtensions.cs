namespace NetEvolve.HealthChecks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Arguments;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// Extensions methods for <see cref="IHealthChecksBuilder"/> with custom Health Checks.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Add a health check for the application readiness.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddApplicationReadinessCheck(
        [NotNull] this IHealthChecksBuilder builder,
        params string[] tags
    )
    {
        Argument.ThrowIfNull(builder);
        Argument.ThrowIfNull(tags);

        if (builder.Services.Any(x => x.ServiceType == typeof(ApplicationReadinessCheckMarker)))
        {
            return builder;
        }

        _ = builder.Services
            .AddSingleton<ApplicationReadinessCheckMarker>()
            .AddSingleton<ApplicationReadinessCheck>();
        return builder.AddCheck<ApplicationReadinessCheck>(
            "ApplicationReadiness",
            HealthStatus.Unhealthy,
            new[] { "self", "readiness" }.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Add a health check for the application, always <see cref="HealthStatus.Healthy"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddApplicationSelfCheck(
        [NotNull] this IHealthChecksBuilder builder,
        params string[] tags
    )
    {
        Argument.ThrowIfNull(builder);
        Argument.ThrowIfNull(tags);

        if (builder.Services.Any(x => x.ServiceType == typeof(ApplicationSelfCheckMarker)))
        {
            return builder;
        }

        _ = builder.Services
            .AddSingleton<ApplicationSelfCheckMarker>()
            .AddSingleton<ApplicationSelfCheck>();
        return builder.AddCheck<ApplicationSelfCheck>(
            "ApplicationSelf",
            HealthStatus.Unhealthy,
            new[] { "self" }.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed class ApplicationReadinessCheckMarker { }

    private sealed class ApplicationSelfCheckMarker { }
}
