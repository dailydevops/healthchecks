namespace NetEvolve.HealthChecks.OpenSearch;

using global::OpenSearch.Client;

/// <summary>
/// Options for <see cref="OpenSearchHealthCheck"/>
/// </summary>
public sealed record OpenSearchOptions
{
    /// <summary>
    /// Gets or sets the mode used to create/retrieve a client instance.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="OpenSearchClientCreationMode.ServiceProvider"/>.
    /// </remarks>
    public OpenSearchClientCreationMode Mode { get; set; } = OpenSearchClientCreationMode.ServiceProvider;

    /// <summary>
    /// Gets or sets the key used to resolve the <see cref="OpenSearchClient"/> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <see cref="OpenSearchClient"/> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// <br/>
    /// When null or empty, the health check will resolve the <see cref="OpenSearchClient"/> using <c>IServiceProvider.GetRequiredService</c>.
    /// <br/>
    /// This option is only used when <see cref="Mode"/> is set to <see cref="OpenSearchClientCreationMode.ServiceProvider"/>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the connection strings to the OpenSearch instance(s) to check.
    /// <br/>
    /// If only one connection string is given, instanciates a new <see cref="OpenSearchClient"/> with a node connection,
    /// otherwise with a cluster connection.
    /// </summary>
    /// <remarks>
    /// This option is only required when <see cref="Mode"/> is set to <see cref="OpenSearchClientCreationMode.UsernameAndPassword"/>.
    /// </remarks>
    public IList<string> ConnectionStrings { get; } = [];

    /// <summary>
    /// Gets or sets the username for authenticating with the testing client.
    /// </summary>
    /// <remarks>
    /// This option is only used when <see cref="Mode"/> is set to <see cref="OpenSearchClientCreationMode.UsernameAndPassword"/>
    /// and <see cref="Password"/> is set.
    /// </remarks>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for authenticating with the testing client.
    /// </summary>
    /// <remarks>
    /// This option is only used when <see cref="Mode"/> is set to <see cref="OpenSearchClientCreationMode.UsernameAndPassword"/>
    /// and <see cref="Username"/> is set.
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
    public Func<OpenSearchClient, CancellationToken, Task<bool>> CommandAsync { get; internal set; } =
        OpenSearchHealthCheck.DefaultCommandAsync;
}
