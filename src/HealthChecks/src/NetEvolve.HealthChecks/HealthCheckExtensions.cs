namespace NetEvolve.HealthChecks;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddSelfCheck(
        [NotNull] this IHealthChecksBuilder builder,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(tags);

        if (builder.Services.Any(x => x.ServiceType == typeof(SelfCheckMarker)))
        {
            return builder;
        }

        _ = builder.Services.AddSingleton<SelfCheckMarker>().AddSingleton<SelfHealthCheck>();
        return builder.AddCheck<SelfHealthCheck>(
            "SelfCheck",
            HealthStatus.Unhealthy,
            new[] { "self" }.Union(tags)
        );
    }

    private sealed class SelfCheckMarker { }
}
