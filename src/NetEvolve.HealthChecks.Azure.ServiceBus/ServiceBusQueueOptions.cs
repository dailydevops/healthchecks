namespace NetEvolve.HealthChecks.Azure.ServiceBus;

/// <summary>
/// Represents configuration options for Azure Service Bus queue health checks.
/// </summary>
public class ServiceBusQueueOptions : ServiceBusOptionsBase
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
    /// Gets or sets the name of the queue to check for existence.
    /// </summary>
    public string? QueueName { get; set; }
}
