namespace NetEvolve.HealthChecks.Azure.ServiceBus;

/// <summary>
/// Represents configuration options for Azure Service Bus topic health checks.
/// </summary>
public class ServiceBusTopicOptions : ServiceBusOptionsBase
{
    /// <summary>
    /// Gets or sets the name of the topic to check for existence.
    /// </summary>
    public string TopicName { get; set; }
}
