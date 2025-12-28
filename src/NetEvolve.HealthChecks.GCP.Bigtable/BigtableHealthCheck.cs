namespace NetEvolve.HealthChecks.GCP.Bigtable;

using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(BigtableOptions))]
internal sealed partial class BigtableHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        BigtableOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<BigtableTableAdminClient>()
            : _serviceProvider.GetRequiredKeyedService<BigtableTableAdminClient>(options.KeyedService);

        // Use project name from options, environment, or default placeholder
        var projectId = GetProjectId(options);

        // Use a placeholder instance name that works with emulators
        // ListTables is a lightweight operation that verifies the client can connect to the service
        var instanceName = new InstanceName(projectId, options.InstanceId ?? "_");

        var listTablesTask = Task.Run(
            async () =>
            {
                var result = client.ListTablesAsync(instanceName);
                return await result.ReadPageAsync(1, cancellationToken).ConfigureAwait(false);
            },
            cancellationToken
        );

        var (isTimelyResponse, _) = await listTablesTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }

    private static string GetProjectId(BigtableOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.ProjectName))
        {
            return options.ProjectName;
        }

        if (Environment.GetEnvironmentVariable("BIGTABLE_PROJECT_ID") is { Length: > 0 } envProjectId)
        {
            return envProjectId;
        }

        if (Environment.GetEnvironmentVariable("GCP_PROJECT") is { Length: > 0 } gcpProjectId)
        {
            return gcpProjectId;
        }

        if (Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT") is { Length: > 0 } googleCloudProjectId)
        {
            return googleCloudProjectId;
        }

        return "test-project";
    }
}
