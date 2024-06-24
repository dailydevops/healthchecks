namespace NetEvolve.HealthChecks.Tests.Integration;

using System.IO;
using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.SortPropertiesAlphabetically();
        VerifierSettings.SortJsonObjects();

        VerifierSettings.AutoVerify(includeBuildServer: false);

        Verifier.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) =>
            {
                var directory = Path.Combine(projectDirectory, "_snapshots");
                _ = Directory.CreateDirectory(directory);
                return new(directory, type.Name, method.Name);
            }
        );
    }
}
