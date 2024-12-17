namespace NetEvolve.HealthChecks.Apache.ActiveMq;

public class ActiveMqOptions
{
    public string BrokerAddress { get; set; }
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

    public int Timeout { get; set; } = 100;
}
