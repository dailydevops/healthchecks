namespace NetEvolve.HealthChecks.Elasticsearch;

using System.Collections.ObjectModel;
using Elastic.Clients.Elasticsearch;

/// <summary>
/// Options for <see cref="ElasticsearchHealthCheck"/>
/// </summary>
public sealed record ElasticsearchOptions
{
    /// <summary>
    /// Gets or sets the mode used to create/retrieve a client instance.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="ElasticsearchClientCreationMode.ServiceProvider"/>.
    /// </remarks>
    public ElasticsearchClientCreationMode Mode { get; set; } = ElasticsearchClientCreationMode.ServiceProvider;

    /// <summary>
    /// Gets or sets the key used to resolve the <see cref="ElasticsearchClient"/> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <see cref="ElasticsearchClient"/> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// <br/>
    /// When null or empty, the health check will resolve the <see cref="ElasticsearchClient"/> using <c>IServiceProvider.GetRequiredService</c>.
    /// <br/>
    /// This option is only used when <see cref="Mode"/> is set to <see cref="ElasticsearchClientCreationMode.ServiceProvider"/>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the connection strings to the Elasticsearch instance(s) to check.
    /// <br/>
    /// If only one connection string is given, instanciates a new <see cref="ElasticsearchClient"/> with a node connection,
    /// otherwise with a cluster connection.
    /// </summary>
    /// <remarks>
    /// This option is only required when <see cref="Mode"/> is set to <see cref="ElasticsearchClientCreationMode.UsernameAndPassword"/>.
    /// </remarks>
    public IList<string> ConnectionStrings { get; } = [];

    /// <summary>
    /// Gets or sets the username for authenticating with the testing client.
    /// </summary>
    /// <remarks>
    /// This option is only used when <see cref="Mode"/> is set to <see cref="ElasticsearchClientCreationMode.UsernameAndPassword"/>
    /// and <see cref="Password"/> is set.
    /// </remarks>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for authenticating with the testing client.
    /// </summary>
    /// <remarks>
    /// This option is only used when <see cref="Mode"/> is set to <see cref="ElasticsearchClientCreationMode.UsernameAndPassword"/>
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
    public Func<ElasticsearchClient, CancellationToken, Task<bool>> CommandAsync { get; internal set; } =
        ElasticsearchHealthCheck.DefaultCommandAsync;
}
