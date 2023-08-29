namespace NetEvolve.HealthChecks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public static class DependencyInjectionExtensions
{
    public static IHealthChecksBuilder AddApplicationReadinessCheck(
        [NotNull] this IHealthChecksBuilder builder,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(tags);

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

    public static IHealthChecksBuilder AddApplicationSelfCheck(
        [NotNull] this IHealthChecksBuilder builder,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(tags);

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
