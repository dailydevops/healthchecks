namespace NetEvolve.HealthChecks.GCP.Firestore;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class FirestoreHealthCheck : ConfigurableHealthCheckBase<FirestoreOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public FirestoreHealthCheck(IOptionsMonitor<FirestoreOptions> optionsMonitor, IServiceProvider serviceProvider)
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
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
        var (isValid, _) = await testDoc
            .GetSnapshotAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
