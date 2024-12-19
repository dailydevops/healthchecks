namespace NetEvolve.HealthChecks.Tests.Architecture;

using System;
using System.Threading;
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;

internal static class HealthCheckArchitecture
{
    // TIP: load your architecture once at the start to maximize performance of your tests
    private static readonly Lazy<Architecture> _instance = new Lazy<Architecture>(
        LoadArchitecture,
        LazyThreadSafetyMode.PublicationOnly
    );

    public static Architecture Instance => _instance.Value;

    private static Architecture LoadArchitecture()
    {
        var assemblies = System
            .Reflection.Assembly.GetExecutingAssembly()!
            .GetReferencedAssemblies()
            .Where(a =>
                a.Name?.StartsWith("NetEvolve.HealthChecks", StringComparison.OrdinalIgnoreCase)
                == true
            )
            .Select(System.Reflection.Assembly.Load)
            .ToArray();

        return new ArchLoader()
            .LoadAssembliesRecursively(
                assemblies,
                x =>
                    x.Name.Name.StartsWith(
                        "NetEvolve.HealthChecks",
                        StringComparison.OrdinalIgnoreCase
                    )
                        ? FilterResult.LoadAndContinue
                        : FilterResult.SkipAndContinue
            )
            .Build();
    }
}
