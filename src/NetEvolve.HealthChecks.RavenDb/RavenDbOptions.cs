namespace NetEvolve.HealthChecks.RavenDb;

using Raven.Client.Documents;

/// <summary>
/// Options for <see cref="RavenDbHealthCheck"/>
/// </summary>
public sealed record RavenDbOptions
{
    /// <summary>
    /// Gets or sets the key used to resolve the <c>IConnection</c> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <c>IConnection</c> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <c>IConnection</c> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// The command to execute against the database.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public Func<IDocumentStore, CancellationToken, Task<bool>> CommandAsync { get; internal set; } =
        RavenDbHealthCheck.DefaultCommandAsync;
}
