namespace NetEvolve.HealthChecks.Dapr;

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
    /// Adds a health check for the Dapr sidecar.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddDapr(
        [NotNull] this IHealthChecksBuilder builder,
        Action<DaprOptions>? options = null,
        params string[] tags
    )
    {
        Argument.ThrowIfNull(builder);
        Argument.ThrowIfNull(tags);

        if (builder.IsServiceTypeRegistered<DaprMarker>())
        {
            return builder;
        }

        _ = builder.Services
            .AddSingleton<DaprMarker>()
            .AddSingleton<DaprHealthCheck>()
            .ConfigureOptions<DaprConfigure>();

        const string internalName = "DaprSidecar";

        if (options is not null)
        {
            _ = builder.Services.Configure(internalName, options);
        }

        return builder.AddCheck<DaprHealthCheck>(
            internalName,
            HealthStatus.Unhealthy,
            new[] { "dapr" }.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class DaprMarker { }
}
