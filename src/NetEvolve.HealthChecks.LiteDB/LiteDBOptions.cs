namespace NetEvolve.HealthChecks.LiteDB;

/// <summary>
/// Options for <see cref="LiteDBHealthCheck"/>
/// </summary>
public record LiteDBOptions
{
    /// <summary>
    /// Gets or sets the connection string to the LiteDB database.
    /// </summary>
    /// <value>
    /// The connection string used to establish a connection to the LiteDB database.
    /// </value>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// Gets or sets the name of the collection to check for existence in the LiteDB database.
    /// </summary>
    /// <value>
    /// The name of the collection to verify.
    /// </value>
    public string CollectionName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when executing operations against the database.
    /// </summary>
    /// <value>
    /// The timeout in milliseconds. Default value is 100 milliseconds.
    /// </value>
    public int Timeout { get; set; } = 100;
}
