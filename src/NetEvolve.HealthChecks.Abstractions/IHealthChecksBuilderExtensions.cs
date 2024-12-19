namespace NetEvolve.HealthChecks.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

/// <summary>
/// Extension methods for <see cref="IHealthChecksBuilder"/>.
/// </summary>
public static partial class IHealthChecksBuilderExtensions
{
    /// <summary>
    /// Determines whether the specified service type is registered.
    /// </summary>
    /// <typeparam name="T">Type of service</typeparam>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/> instance.</param>
    /// <exception cref="ArgumentNullException">Throws a <see cref="ArgumentNullException"/>, when <paramref name="builder"/> is null.</exception>
    public static bool IsServiceTypeRegistered<T>(this IHealthChecksBuilder builder)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.Services.Any(x => x.ServiceType == typeof(T));
    }

    /// <summary>
    /// Determines whether the specified name is already registered for the service type.
    /// </summary>
    /// <typeparam name="T">Type of service</typeparam>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/> instance.</param>
    /// <param name="name">Configuration name.</param>
    /// <exception cref="ArgumentNullException">Throws a <see cref="ArgumentNullException"/>, when <paramref name="builder"/> is null.</exception>
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
