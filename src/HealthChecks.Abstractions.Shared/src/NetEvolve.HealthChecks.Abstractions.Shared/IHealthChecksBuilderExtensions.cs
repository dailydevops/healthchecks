﻿#if USE_CONFIGURABLE_HEALTHCHECK
namespace NetEvolve.HealthChecks.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

internal static partial class IHealthChecksBuilderExtensions
{
    public static bool IsNameAlreadyUsed<T>(this IHealthChecksBuilder builder, string name) where T : IHealthCheck
    {
        var serviceProvider = builder.Services.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var options = scope.ServiceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            if (options?.Value is not null)
            {
                var registrations = options.Value.Registrations;

                foreach (var registration in registrations)
                {
                    if (registration.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    {
                        var healthCheck = registration.Factory(scope.ServiceProvider);

                        if (healthCheck is T)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}

#endif
