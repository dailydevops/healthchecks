namespace NetEvolve.HealthChecks.Tests.Integration;

using System.Runtime.CompilerServices;
using VerifyTests;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.SortPropertiesAlphabetically();
        VerifierSettings.SortJsonObjects();
    }
}
