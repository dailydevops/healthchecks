namespace NetEvolve.HealthChecks.Azure.ServiceBus;

public class ServiceBusSubscriptionOptions : ServiceBusOptionsBase
{
    /// <summary>
    /// Gets or sets a value indicating whether to enable peek mode, default is <c>false</c>.
    /// </summary>
    /// <remarks>
    /// To enable the peek mode, the executing user requires Listen claim to work.
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/azure/role-based-access-control/built-in-roles#azure-service-bus-data-sender"/>
    public bool EnablePeekMode { get; set; }
    public string SubscriptionName { get; set; }
    public string TopicName { get; set; }
}
