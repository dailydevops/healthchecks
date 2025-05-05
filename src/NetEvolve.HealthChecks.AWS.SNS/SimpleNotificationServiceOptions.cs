namespace NetEvolve.HealthChecks.AWS.SNS;

using Amazon;
using Amazon.Runtime;

public sealed class SimpleNotificationServiceOptions
{
    public string? AccessKey { get; set; }

    public CreationMode CreationMode { get; set; }

    public string? SecretKey { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
    public string? ServiceUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    public string? TopicName { get; set; }

    public int Timeout { get; set; } = 100;

    internal AWSCredentials? GetCredentials() =>
        CreationMode switch
        {
            CreationMode.BasicAuthentication => new BasicAWSCredentials(AccessKey, SecretKey),
            _ => null,
        };

    internal RegionEndpoint? GetRegionEndpoint() => CreationMode == CreationMode ? null : null;
}
