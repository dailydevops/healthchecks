namespace NetEvolve.HealthChecks.MongoDb;

using MongoDB.Bson;
using MongoDB.Driver;

/// <summary>
/// Options for <see cref="MongoDbHealthCheck"/>
/// </summary>
public sealed record MongoDbOptions
{
    /// <summary>
    /// The connection string for the database to check.
    /// </summary>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// The command to execute against the database.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public Func<MongoClient, CancellationToken, Task<BsonDocument>> CommandAsync { get; internal set; } =
        MongoDbHealthCheck.DefaultCommandAsync;
}
