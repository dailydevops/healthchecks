namespace NetEvolve.HealthChecks.GCP.Firestore;

using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(FirestoreOptions))]
internal sealed partial class FirestoreHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        FirestoreOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<FirestoreDb>()
            : _serviceProvider.GetRequiredKeyedService<FirestoreDb>(options.KeyedService);

        // Use a simple operation to check if the database is accessible
        // Creating a document reference is a local operation that validates the client is configured
        var testCollection = client.Collection("healthcheck");
        var testDoc = testCollection.Document("test");

        // Attempt a lightweight operation with timeout
        var (isTimelyResponse, snapshot) = await testDoc
            .GetSnapshotAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (snapshot is null)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Document not found.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
