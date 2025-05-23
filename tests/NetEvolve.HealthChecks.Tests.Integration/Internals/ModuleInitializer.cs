namespace NetEvolve.HealthChecks.Tests.Integration.Internals;

using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
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

        // Removes Azure.ServiceBus TrackingId from error message
        VerifierSettings.AddScrubber(RemoveTrackingId);
    }

    private static void RemoveTrackingId(StringBuilder builder)
    {
        const string trackingId = "  TrackingId:";

        if (builder.Length <= trackingId.Length)
        {
            return;
        }

        var value = builder.ToString();
        var trackingIdIndex = value.IndexOf(trackingId, 0, StringComparison.OrdinalIgnoreCase);
        if (trackingIdIndex == -1)
        {
            return;
        }

        _ = builder.Remove(trackingIdIndex, value.Length - trackingIdIndex);
    }
}
