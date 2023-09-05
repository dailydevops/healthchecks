namespace NetEvolve.HealthChecks.MySql;

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
    /// Add a health check for the MySql database.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the <see cref="MySqlCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddMySql(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<MySqlOptions>? options = null,
        params string[] tags
    )
    {
        Argument.ThrowIfNull(builder);
        Argument.ThrowIfNullOrEmpty(name);
        Argument.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<MySqlCheckMarker>())
        {
            _ = builder.Services
                .AddSingleton<MySqlCheckMarker>()
                .AddSingleton<MySqlCheck>()
                .ConfigureOptions<MySqlConfigure>();
        }

        var internalName = name.EnsureStartsWith("MySql", StringComparison.OrdinalIgnoreCase);

        if (builder.IsNameAlreadyUsed(internalName))
        {
            throw new ArgumentException($"Name `{name}` already in use.", nameof(name), null);
        }

        if (options is not null)
        {
            _ = builder.Services.Configure(internalName, options);
        }

        return builder.AddCheck<MySqlCheck>(
            internalName,
            HealthStatus.Unhealthy,
            new[] { "mysql", "database" }.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class MySqlCheckMarker { }
}
