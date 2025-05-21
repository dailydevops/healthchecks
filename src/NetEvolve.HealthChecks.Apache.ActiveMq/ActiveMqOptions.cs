namespace NetEvolve.HealthChecks.Apache.ActiveMq;

/// <summary>
/// Represents configuration options for the Apache ActiveMQ health check.
/// </summary>
public sealed record ActiveMqOptions
{
    /// <summary>
    /// Gets or sets the address of the ActiveMQ broker.
    /// </summary>
    public string? BrokerAddress { get; set; }

    /// <summary>
    /// Gets or sets the username for authenticating with the ActiveMQ broker.
    /// Returns <c>null</c> if the value is null, empty, or whitespace.
    /// </summary>
    public string? Username
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
    /// Gets or sets the password for authenticating with the ActiveMQ broker.
    /// Returns <c>null</c> if the value is null, empty, or whitespace.
    /// </summary>
    public string? Password
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
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;
}
