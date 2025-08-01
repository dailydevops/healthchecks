﻿namespace NetEvolve.HealthChecks.SqlServer.Devart;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using global::Devart.Data.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Arguments;
using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Extensions methods for <see cref="IHealthChecksBuilder"/> with custom Health Checks.
/// </summary>
public static class DependencyInjectionExtensions
{
    private static readonly string[] _defaultTags = ["sqlserver", "database", "devart"];

    /// <summary>
    /// Add a health check for the SQL Server database, based on Devart's <see cref="SqlConnection"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the <see cref="SqlServerDevartHealthCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddSqlServerDevart(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<SqlServerDevartOptions>? options = null,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        Argument.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<SqlServerDevartCheckMarker>())
        {
            _ = builder
                .Services.AddSingleton<SqlServerDevartCheckMarker>()
                .AddSingleton<SqlServerDevartHealthCheck>()
                .ConfigureOptions<SqlServerDevartConfigure>();
        }

        builder.ThrowIfNameIsAlreadyUsed<SqlServerDevartHealthCheck>(name);

        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }

        return builder.AddCheck<SqlServerDevartHealthCheck>(
            name,
            HealthStatus.Unhealthy,
            _defaultTags.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class SqlServerDevartCheckMarker;
}
