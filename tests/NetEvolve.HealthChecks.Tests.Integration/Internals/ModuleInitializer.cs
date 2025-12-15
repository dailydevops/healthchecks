namespace NetEvolve.HealthChecks.Tests.Integration.Internals;

using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using VerifyTests;
using VerifyTUnit;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        // Set all tests to use the same culture
        // This is necessary to ensure consistent results across different environments
        var cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        VerifierSettings.SortPropertiesAlphabetically();
        VerifierSettings.SortJsonObjects();

        VerifierSettings.AutoVerify(includeBuildServer: false, throwException: true);

        Verifier.DerivePathInfo(
            (_, projectDirectory, type, method) =>
            {
                var directory = Path.Combine(projectDirectory, "_snapshots");
                var createdDirectory = Directory.CreateDirectory(directory);
                return new(createdDirectory.FullName, CleanTypeName(type), CleanMethodName(method.Name));
            }
        );

        // Removes Azure.ServiceBus TrackingId from error message
        VerifierSettings.AddScrubber(RemoveTrackingId);
    }

    private static string CleanTypeName(Type type) =>
        type.Name.Replace("Tests", string.Empty, StringComparison.OrdinalIgnoreCase);

    private static string CleanMethodName(string methodName) =>
        methodName
            .Replace("Async", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("_", "_", StringComparison.OrdinalIgnoreCase);

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
