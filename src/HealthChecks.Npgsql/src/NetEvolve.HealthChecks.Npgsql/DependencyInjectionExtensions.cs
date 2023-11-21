namespace NetEvolve.HealthChecks.Npgsql;

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
    private static readonly string[] _defaultTags = new[] { "postgresql", "database" };

    /// <summary>
    /// Add a health check for the MySql database.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the <see cref="NpgsqlCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddPostgreSql(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<NpgsqlOptions>? options = null,
        params string[] tags
    )
    {
        Argument.ThrowIfNull(builder);
        Argument.ThrowIfNullOrEmpty(name);
        Argument.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<NpgsqlCheckMarker>())
        {
            _ = builder
                .Services
                .AddSingleton<NpgsqlCheckMarker>()
                .AddSingleton<NpgsqlCheck>()
                .ConfigureOptions<NpgsqlConfigure>();
        }

        if (builder.IsNameAlreadyUsed<NpgsqlCheck>(name))
        {
            throw new ArgumentException($"Name `{name}` already in use.", nameof(name), null);
        }

        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }

        return builder.AddCheck<NpgsqlCheck>(
            name,
            HealthStatus.Unhealthy,
            _defaultTags.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class NpgsqlCheckMarker { }
}
