namespace NetEvolve.HealthChecks.Tests.Integration.Internals;

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
            (_, projectDirectory, type, method) =>
            {
                var directory = Path.Combine(projectDirectory, "_snapshots");
                var createdDirectory = Directory.CreateDirectory(directory);
                return new(createdDirectory.FullName, type.Name, method.Name);
            }
        );
    }
}
