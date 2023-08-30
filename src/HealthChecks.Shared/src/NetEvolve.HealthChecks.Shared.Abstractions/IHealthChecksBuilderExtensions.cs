namespace NetEvolve.HealthChecks;

using Microsoft.Extensions.DependencyInjection;
using System.Linq;

internal static partial class IHealthChecksBuilderExtensions
{
    public static bool IsServiceTypeRegistered<T>(this IHealthChecksBuilder builder)
        where T : class
        => builder.Services.Any(x => x.ServiceType == typeof(T));
}
