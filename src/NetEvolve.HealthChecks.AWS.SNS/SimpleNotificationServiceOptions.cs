namespace NetEvolve.HealthChecks.AWS.SNS;

using Amazon;
using Amazon.Runtime;

public sealed class SimpleNotificationServiceOptions
{
    public string? AccessKey { get; set; }

    public CreationMode? Mode { get; set; }

    public string? SecretKey { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
    /// <inheritdoc cref="ClientConfig.ServiceURL"/>
    public string? ServiceUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    public string? TopicName { get; set; }

    public int Timeout { get; set; } = 100;
    public string? Subscription { get; set; }

    /// <inheritdoc cref="ClientConfig.RegionEndpoint"/>
    public RegionEndpoint? RegionEndpoint { get; set; }

    internal AWSCredentials? GetCredentials() =>
        Mode switch
        {
            CreationMode.BasicAuthentication => new BasicAWSCredentials(AccessKey, SecretKey),
            _ => null,
        };
}
