namespace NetEvolve.HealthChecks.GCP.Firestore;

/// <summary>
/// Options for <see cref="FirestoreHealthCheck"/>
/// </summary>
public sealed record FirestoreOptions
{
    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when executing tasks against the Firestore database.
    /// </summary>
    /// <value>
    /// The timeout in milliseconds. Default value is 100 milliseconds.
    /// </value>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the keyed service name for retrieving the <see cref="Google.Cloud.Firestore.FirestoreDb"/> instance.
    /// </summary>
    /// <value>
    /// The keyed service name, or <c>null</c> if using the default service registration.
    /// </value>
    public string? KeyedService { get; set; }
}
