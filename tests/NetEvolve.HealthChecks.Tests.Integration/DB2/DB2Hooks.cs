namespace NetEvolve.HealthChecks.Tests.Integration.DB2;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public static class DB2Hooks
{
    [Before(Assembly)]
    public static Task BeforeTestDiscovery()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var assemblyLocation = typeof(DB2HealthCheckTests).Assembly.Location;

            const EnvironmentVariableTarget target = EnvironmentVariableTarget.Process;
            var buildOutput = Path.GetDirectoryName(assemblyLocation);

            var clidriverDirectory = Path.Combine(buildOutput, "clidriver");
            var binDirectory = Path.Combine(clidriverDirectory, "bin");
            var libDirectory = Path.Combine(clidriverDirectory, "lib");
            var iccDirectory = Path.Combine(libDirectory, "icc");

            var envPATH = Environment.GetEnvironmentVariable("PATH", target);
            Environment.SetEnvironmentVariable("PATH", $"{envPATH};{binDirectory}", target);

            Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", $"{libDirectory};{iccDirectory}", target);

            Environment.SetEnvironmentVariable("DB2_CLI_DRIVER_INSTALL_PATH", clidriverDirectory, target);
        }

        return Task.CompletedTask;
    }
}
