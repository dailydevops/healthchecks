namespace NetEvolve.HealthChecks.Azure.ServiceBus;

/// <summary>
/// Represents configuration options for Azure Service Bus subscription health checks.
/// </summary>
public sealed record ServiceBusSubscriptionOptions : ServiceBusOptionsBase
{
    /// <summary>
    /// Gets or sets a value indicating whether to enable peek mode. Default is <c>false</c>.
    /// </summary>
    /// <remarks>
    /// To enable the peek mode, the executing user requires the Listen claim to work.
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/azure/role-based-access-control/built-in-roles#azure-service-bus-data-sender"/>
    public bool EnablePeekMode { get; set; }

    /// <summary>
    /// Gets or sets the name of the subscription to check for existence.
    /// </summary>
    public string SubscriptionName { get; set; }

    /// <summary>
    /// Gets or sets the name of the topic associated with the subscription.
    /// </summary>
    public string TopicName { get; set; }
}
