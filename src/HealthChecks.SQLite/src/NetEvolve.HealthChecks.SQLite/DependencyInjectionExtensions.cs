﻿namespace NetEvolve.HealthChecks.SQLite;

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
    /// Add a health check for the SQL Server database.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the <see cref="SQLiteCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddSQLite(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<SQLiteOptions>? options = null,
        params string[] tags
    )
    {
        Argument.ThrowIfNull(builder);
        Argument.ThrowIfNullOrEmpty(name);
        Argument.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<SQLiteCheckMarker>())
        {
            _ = builder.Services
                .AddSingleton<SQLiteCheckMarker>()
                .AddSingleton<SQLiteCheck>()
                .ConfigureOptions<SQLiteConfigure>();
        }

        var internalName = name.EnsureStartsWith("SQLite", StringComparison.OrdinalIgnoreCase);

        if (builder.IsNameAlreadyUsed(internalName))
        {
            throw new ArgumentException($"Name `{name}` already in use.", nameof(name), null);
        }

        if (options is not null)
        {
            _ = builder.Services.Configure(internalName, options);
        }

        return builder.AddCheck<SQLiteCheck>(
            internalName,
            HealthStatus.Unhealthy,
            new[] { "sqlserver", "database" }.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class SQLiteCheckMarker { }
}