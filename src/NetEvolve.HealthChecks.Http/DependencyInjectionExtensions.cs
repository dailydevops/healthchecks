namespace NetEvolve.HealthChecks.Http;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SourceGenerator.Attributes;

/// <summary>
/// Extensions methods for <see cref="IHealthChecksBuilder"/> with custom Health Checks.
/// </summary>
[HealthCheckHelper]
public static partial class DependencyInjectionExtensions
{
    private static readonly string[] _tags = ["http", "endpoint"];

    /// <summary>
    /// Adds a health check for HTTP endpoints.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the health check. The name is used to identify the configuration object.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddHttp(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<HttpOptions>? options = null,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<HttpMarker>())
        {
            _ = builder
                .Services.AddSingleton<HttpMarker>()
                .AddSingleton<HttpHealthCheck>()
                .ConfigureOptions<HttpConfigure>();

            // Ensure HttpClient is available
            _ = builder.Services.AddHttpClient();
        }

        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }

        return builder.AddCheck<HttpHealthCheck>(
            name,
            HealthStatus.Unhealthy,
            _tags.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class HttpMarker;
}
