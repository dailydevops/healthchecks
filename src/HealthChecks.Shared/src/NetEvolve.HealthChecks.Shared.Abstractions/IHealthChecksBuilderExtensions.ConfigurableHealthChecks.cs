#if USE_CONFIGURABLE_HEALTHCHECK || USE_SQL_HEALTHCHECK
namespace NetEvolve.HealthChecks;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

internal static partial class IHealthChecksBuilderExtensions
{
    public static bool IsNameAlreadyUsed<T>(this IHealthChecksBuilder builder, string name)
        where T : IHealthCheck
    {
        var serviceProvider = builder.Services.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var options = scope.ServiceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            return options?.Value?.Registrations
                    is ICollection<HealthCheckRegistration> registrations
                && registrations.Any(IsNameAlreadyUsedForServiceType);

            bool IsNameAlreadyUsedForServiceType(HealthCheckRegistration registration) =>
                                    registration.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                                    && registration.Factory(scope.ServiceProvider).GetType() == typeof(T);
        }
    }
}

#endif
