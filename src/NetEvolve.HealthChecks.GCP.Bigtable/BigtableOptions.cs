namespace NetEvolve.HealthChecks.GCP.Bigtable;

/// <summary>
/// Options for <see cref="BigtableHealthCheck"/>
/// </summary>
public sealed record BigtableOptions
{
    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when executing tasks against the Bigtable database.
    /// </summary>
    /// <value>
    /// The timeout in milliseconds. Default value is 100 milliseconds.
    /// </value>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the keyed service name for retrieving the <see cref="Google.Cloud.Bigtable.Admin.V2.BigtableTableAdminClient"/> instance.
    /// </summary>
    /// <value>
    /// The keyed service name, or <c>null</c> if using the default service registration.
    /// </value>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the Google Cloud Project ID.
    /// </summary>
    /// <value>
    /// The project ID. If not specified, attempts to read from environment variables
    /// (BIGTABLE_PROJECT_ID, GCP_PROJECT, GOOGLE_CLOUD_PROJECT) or defaults to "test-project".
    /// </value>
    public string? ProjectName { get; set; }

    /// <summary>
    /// Gets or sets the Bigtable Instance ID.
    /// </summary>
    /// <value>
    /// The instance ID. If not specified, uses "_" as a placeholder for connectivity checks.
    /// </value>
    public string? InstanceId { get; set; }
}
