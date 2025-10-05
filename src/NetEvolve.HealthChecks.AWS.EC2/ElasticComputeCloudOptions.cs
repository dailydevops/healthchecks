namespace NetEvolve.HealthChecks.AWS.EC2;

using Amazon;
using Amazon.Runtime;

/// <summary>
/// Represents configuration options for the AWS EC2 health check.
/// </summary>
public sealed record ElasticComputeCloudOptions
{
    /// <summary>
    /// Gets or sets the AWS access key used for authentication.
    /// </summary>
    public string? AccessKey { get; set; }

    /// <summary>
    /// Gets or sets the name of the EC2 instance to check.
    /// </summary>
    public string? KeyName { get; set; }

    /// <summary>
    /// Gets or sets the creation mode for the EC2 client.
    /// </summary>
    public CreationMode? Mode { get; set; }

    /// <inheritdoc cref="ClientConfig.RegionEndpoint"/>
    public RegionEndpoint? RegionEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the AWS secret key used for authentication.
    /// </summary>
    public string? SecretKey { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
    /// <inheritdoc cref="ClientConfig.ServiceURL"/>
    public string? ServiceUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    /// <summary>
    /// Gets or sets the timeout in milliseconds for the health check operation. Default is 100 ms.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets the AWS credentials based on the configured mode.
    /// </summary>
    /// <returns>The AWS credentials or <c>null</c> if not configured.</returns>
    internal AWSCredentials? GetCredentials() =>
        Mode switch
        {
            CreationMode.BasicAuthentication => new BasicAWSCredentials(AccessKey, SecretKey),
            _ => null,
        };
}
