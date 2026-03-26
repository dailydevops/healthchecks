namespace NetEvolve.HealthChecks.Apache.RocketMQ;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Configuration options for the <see cref="RocketMQHealthCheck"/>.
/// </summary>
[SuppressMessage(
    "Minor Code Smell",
    "S2325:Methods and properties that don't access instance data should be static",
    Justification = "False positive."
)]
public sealed record RocketMQOptions
{
    /// <summary>
    /// Gets or sets the gRPC endpoint of the RocketMQ name server (e.g., "127.0.0.1:8081").
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the topic used for the health check message.
    /// </summary>
    public string? Topic { get; set; }

    /// <summary>
    /// Gets or sets the access key for authenticating with the RocketMQ broker.
    /// Returns <c>null</c> if the value is null, empty, or whitespace.
    /// </summary>
    public string? AccessKey
    {
        get
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                return null;
            }

            return field;
        }
        set;
    }

    /// <summary>
    /// Gets or sets the access secret for authenticating with the RocketMQ broker.
    /// Returns <c>null</c> if the value is null, empty, or whitespace.
    /// </summary>
    public string? AccessSecret
    {
        get
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                return null;
            }

            return field;
        }
        set;
    }

    /// <summary>
    /// Gets or sets whether to enable SSL/TLS for the broker connection. Default is <c>true</c>.
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    /// <remarks>
    /// The minimum value is -1 (Timeout.Infinite). Default value is 100 milliseconds.
    /// </remarks>
    public int Timeout { get; set; } = 100;
}
