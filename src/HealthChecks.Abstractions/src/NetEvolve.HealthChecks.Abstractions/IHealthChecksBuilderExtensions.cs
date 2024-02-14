namespace NetEvolve.HealthChecks;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

[ExcludeFromCodeCoverage]
public static partial class IHealthChecksBuilderExtensions
{
    public static bool IsServiceTypeRegistered<T>(this IHealthChecksBuilder builder)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.Services.Any(x => x.ServiceType == typeof(T));
    }

    public static bool IsNameAlreadyUsed<T>(this IHealthChecksBuilder builder, string name)
        where T : IHealthCheck
    {
        ArgumentNullException.ThrowIfNull(builder);

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
