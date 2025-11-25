namespace NetEvolve.HealthChecks.Minio;

using global::Minio;

/// <summary>
/// Options for <see cref="MinioHealthCheck"/>
/// </summary>
public sealed record MinioOptions
{
    /// <summary>
    /// Gets or sets the key used to resolve the <c>IMinioClient</c> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <c>IMinioClient</c> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <c>IMinioClient</c> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the name of the bucket.
    /// </summary>
    public string? BucketName { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for the health check operation. Default is 100 ms.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// The command to execute against the Minio service.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public Func<IMinioClient, string, CancellationToken, Task<bool>> CommandAsync { get; internal set; } =
        MinioHealthCheck.DefaultCommandAsync;
}
