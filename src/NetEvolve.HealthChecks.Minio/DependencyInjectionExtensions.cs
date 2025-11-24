namespace NetEvolve.HealthChecks.Minio;

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SourceGenerator.Attributes;

/// <summary>
/// Extensions methods for <see cref="IHealthChecksBuilder"/> with custom Health Checks.
/// </summary>
[HealthCheckHelper]
public static partial class DependencyInjectionExtensions
{
    private static readonly string[] _defaultTags = ["minio", "s3", "storage"];

    /// <summary>
    /// Add a health check for Minio.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the <see cref="MinioHealthCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddMinio(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<MinioOptions>? options = null,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<MinioCheckMarker>())
        {
            _ = builder
                .Services.AddSingleton<MinioCheckMarker>()
                .AddSingleton<MinioHealthCheck>()
                .ConfigureOptions<MinioConfigure>();
        }

        builder.ThrowIfNameIsAlreadyUsed<MinioHealthCheck>(name);

        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }

        return builder.AddCheck<MinioHealthCheck>(
            name,
            HealthStatus.Unhealthy,
            _defaultTags.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class MinioCheckMarker;
}
