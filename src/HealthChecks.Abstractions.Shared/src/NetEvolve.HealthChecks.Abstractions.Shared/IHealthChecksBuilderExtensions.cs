#if USE_CONFIGURABLE_HEALTHCHECK || USE_HEALTHCHECK
namespace NetEvolve.HealthChecks.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Linq;

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

                return registrations.Any(x => x.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase) && x.Factory(scope.ServiceProvider) is T);
            }

            return false;
        }
    }
}

#endif
