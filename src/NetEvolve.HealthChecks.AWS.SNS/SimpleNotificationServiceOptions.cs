﻿namespace NetEvolve.HealthChecks.AWS.SNS;

using Amazon;
using Amazon.Runtime;

/// <summary>
/// Represents configuration options for the AWS SNS health check.
/// </summary>
public sealed record SimpleNotificationServiceOptions
{
    /// <summary>
    /// Gets or sets the AWS access key used for authentication.
    /// </summary>
    public string? AccessKey { get; set; }

    /// <summary>
    /// Gets or sets the creation mode for the SNS client.
    /// </summary>
    public CreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the AWS secret key used for authentication.
    /// </summary>
    public string? SecretKey { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
    /// <inheritdoc cref="ClientConfig.ServiceURL"/>
    public string? ServiceUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    /// <summary>
    /// Gets or sets the name of the SNS topic to check.
    /// </summary>
    public string? TopicName { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for the health check operation. Default is 100 ms.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the subscription identifier for the SNS topic.
    /// </summary>
    public string? Subscription { get; set; }

    /// <inheritdoc cref="ClientConfig.RegionEndpoint"/>
    public RegionEndpoint? RegionEndpoint { get; set; }

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
