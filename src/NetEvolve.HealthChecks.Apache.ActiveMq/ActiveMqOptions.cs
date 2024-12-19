namespace NetEvolve.HealthChecks.Apache.ActiveMq;

public class ActiveMqOptions
{
    public string? BrokerAddress { get; set; }
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
