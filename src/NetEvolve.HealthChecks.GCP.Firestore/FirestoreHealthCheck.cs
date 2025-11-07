namespace NetEvolve.HealthChecks.GCP.Firestore;

using System.Threading;
using System.Threading.Tasks;
using global::Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(FirestoreOptions))]
internal sealed partial class FirestoreHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
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
        var (isTimelyResponse, _) = await testDoc
            .GetSnapshotAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }
}
