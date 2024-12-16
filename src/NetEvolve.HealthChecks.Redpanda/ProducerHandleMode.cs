namespace NetEvolve.HealthChecks.Redpanda;

using System;
using Confluent.Kafka;

/// <summary>
/// Describes the mode used to handle the <see cref="IProducer{TKey,TValue}"/>.
/// </summary>
public enum ProducerHandleMode
{
    /// <summary>
    /// The default mode. The <see cref="IProducer{TKey,TValue}"/> is loading the preregistered instance from the <see cref="IServiceProvider"/>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// Create a new <see cref="IProducer{TKey,TValue}"/> instance.
    /// </summary>
    Create = 1,
}
