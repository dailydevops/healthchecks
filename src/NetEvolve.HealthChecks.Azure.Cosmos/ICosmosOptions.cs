namespace NetEvolve.HealthChecks.Azure.Cosmos;

using System;
using Microsoft.Azure.Cosmos;

internal interface ICosmosOptions
{
    /// <summary>
    /// Gets the connection string.
    /// </summary>
    string? ConnectionString { get; }

    /// <summary>
    /// Gets the endpoint URI.
    /// </summary>
    Uri? EndpointUri { get; }

    /// <summary>
    /// Gets the primary key.
    /// </summary>
    string? PrimaryKey { get; }

    /// <summary>
    /// Gets the mode to create the client.
    /// </summary>
    CosmosClientCreationMode Mode { get; }

    /// <summary>
    /// Gets the timeout in milliseconds.
    /// </summary>
    int Timeout { get; }

    /// <summary>
    /// Gets the lambda to configure the <see cref="CosmosClientOptions"/>.
    /// </summary>
    Action<CosmosClientOptions>? ConfigureClientOptions { get; }
}
