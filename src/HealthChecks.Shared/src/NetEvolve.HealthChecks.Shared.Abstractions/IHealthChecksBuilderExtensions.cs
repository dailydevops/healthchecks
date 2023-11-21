namespace NetEvolve.HealthChecks;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
internal static partial class IHealthChecksBuilderExtensions
{
    public static bool IsServiceTypeRegistered<T>(this IHealthChecksBuilder builder)
        where T : class
        => builder.Services.Any(x => x.ServiceType == typeof(T));
}
