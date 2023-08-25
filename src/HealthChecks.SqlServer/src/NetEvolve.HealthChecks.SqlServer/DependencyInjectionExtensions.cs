namespace NetEvolve.HealthChecks.SqlServer;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public static class DependencyInjectionExtensions
{
    public static IHealthChecksBuilder AddSqlServer(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<SqlServerCheckOptions>? options = null,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(tags);

        if (!builder.Services.Any(x => x.ServiceType == typeof(SqlServerCheckMarker)))
        {
            _ = builder.Services
                .AddSingleton<SqlServerCheckMarker>()
                .AddSingleton<SqlServerCheck>();
        }

        var internalName = $"SqlServer{name}";

        builder.Services.Configure(internalName, options ?? (x => { }));

        return builder.AddCheck<SqlServerCheck>(
            internalName,
            HealthStatus.Unhealthy,
            new[] { "self", "readiness" }.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed class SqlServerCheckMarker { }
}
