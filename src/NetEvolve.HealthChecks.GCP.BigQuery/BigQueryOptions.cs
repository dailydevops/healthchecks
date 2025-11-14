namespace NetEvolve.HealthChecks.GCP.BigQuery;

/// <summary>
/// Options for <see cref="BigQueryHealthCheck"/>
/// </summary>
public sealed record BigQueryOptions
{
    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when executing tasks against the BigQuery service.
    /// </summary>
    /// <value>
    /// The timeout in milliseconds. Default value is 100 milliseconds.
    /// </value>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the keyed service name for retrieving the <see cref="Google.Cloud.BigQuery.V2.BigQueryClient"/> instance.
    /// </summary>
    /// <value>
    /// The keyed service name, or <c>null</c> if using the default service registration.
    /// </value>
    public string? KeyedService { get; set; }
}
