namespace NetEvolve.HealthChecks.Redpanda;

using Confluent.Kafka;

/// <summary>
/// Options for <see cref="RedpandaCheck"/>.
/// </summary>
public class RedpandaOptions
{
    /// <summary>
    /// The name of the topic to produce messages to.
    /// </summary>
    public string Topic { get; set; } = default!;

    /// <summary>
    /// Gets or sets the mode to access / create the producer.
    /// </summary>
    public ProducerHandleMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the configuration for the producer.
    /// </summary>
    public ProducerConfig Configuration { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;
}
