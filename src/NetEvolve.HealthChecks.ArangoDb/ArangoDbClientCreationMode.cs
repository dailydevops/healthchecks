namespace NetEvolve.HealthChecks.ArangoDb;

using System;
using ArangoDBNetStandard;

/// <summary>
/// Describes the mode used to create the <see cref="ArangoDBClient"/>.
/// </summary>
public enum ArangoDbClientCreationMode
{
    /// <summary>
    /// The <see cref="ArangoDBClient"/> preregistered instance is retrieved from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <remarks>
    /// This is the default mode.
    /// </remarks>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="ArangoDBClient"/> instance is created using the <see cref="ArangoDbOptions.TransportAddress"/>,
    /// the <see cref="ArangoDbOptions.Username"/> and the <see cref="ArangoDbOptions.Password"/>.
    /// </summary>
    Internal = 1,
}
