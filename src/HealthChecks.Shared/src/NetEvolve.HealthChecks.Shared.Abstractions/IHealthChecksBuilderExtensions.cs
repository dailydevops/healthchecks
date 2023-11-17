namespace NetEvolve.HealthChecks;

using System.Linq;
using Microsoft.Extensions.DependencyInjection;

internal static partial class IHealthChecksBuilderExtensions
{
    public static bool IsServiceTypeRegistered<T>(this IHealthChecksBuilder builder)
        where T : class
        => builder.Services.Any(x => x.ServiceType == typeof(T));
}
