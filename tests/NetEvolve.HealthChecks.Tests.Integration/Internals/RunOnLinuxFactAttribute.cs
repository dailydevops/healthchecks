namespace NetEvolve.HealthChecks.Tests.Integration.Internals;

using System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class RunOnLinuxFactAttribute : FactAttribute
{
    public RunOnLinuxFactAttribute()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Skip = "Execution skipped: The operating system is not Linux.";
        }
    }
}
