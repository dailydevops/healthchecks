namespace NetEvolve.HealthChecks.ArangoDb;

using ArangoDBNetStandard;

/// <summary>
/// Options for <see cref="ArangoDbHealthCheck"/>
/// </summary>
public sealed record ArangoDbOptions
{
    /// <summary>
    /// Gets or sets the mode used to create/retrieve a client instance.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="ArangoDbClientCreationMode.ServiceProvider"/>.
    /// </remarks>
    public ArangoDbClientCreationMode Mode { get; set; } = ArangoDbClientCreationMode.ServiceProvider;

    /// <summary>
    /// Gets or sets the key used to resolve the <see cref="ArangoDBClient"/> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <see cref="ArangoDBClient"/> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// <br/>
    /// When null or empty, the health check will resolve the <see cref="ArangoDBClient"/> using <c>IServiceProvider.GetRequiredService</c>.
    /// <br/>
    /// This option is only used when <see cref="Mode"/> is set to <see cref="ArangoDbClientCreationMode.ServiceProvider"/>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the address to the ArangoDb instance to check.
    /// </summary>
    /// <remarks>
    /// This option is only required when <see cref="Mode"/> is set to <see cref="ArangoDbClientCreationMode.Internal"/>.
    /// </remarks>
    public string? TransportAddress { get; set; }

    /// <summary>
    /// Gets or sets the username for authenticating with the testing client.
    /// </summary>
    /// <remarks>
    /// This option is only used when <see cref="Mode"/> is set to <see cref="ArangoDbClientCreationMode.Internal"/>
    /// and required when <see cref="Password"/> is set.
    /// </remarks>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for authenticating with the testing client.
    /// </summary>
    /// <remarks>
    /// This option is only used when <see cref="Mode"/> is set to <see cref="ArangoDbClientCreationMode.Internal"/>
    /// and required when <see cref="Username"/> is set.
    /// </remarks>
    public string? Password { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>100 milliseconds</c>.
    /// <br/>
    /// Values below <see cref="Timeout.Infinite"/> (-1) are invalid.
    /// </remarks>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// The command to execute against the database.
    /// Returns <see langword="true"/> if successful, <see langword="false"/> otherwise.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public Func<ArangoDBClient, CancellationToken, Task<bool>> CommandAsync { get; internal set; } =
        ArangoDbHealthCheck.DefaultCommandAsync;
}
