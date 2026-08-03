namespace NetEvolve.HealthChecks.Seq;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Seq.Api;

/// <summary>
/// Options for <see cref="SeqHealthCheck"/>
/// </summary>
public sealed record SeqOptions
{
    /// <summary>
    /// Gets or sets the mode used to create a client instance.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="SeqClientCreationMode.ServiceProvider"/>.
    /// </remarks>
    public SeqClientCreationMode Mode { get; set; } = SeqClientCreationMode.ServiceProvider;

    /// <summary>
    /// Gets or sets the key used to resolve the <see cref="SeqConnection"/> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <see cref="SeqConnection"/> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// <br/>
    /// When <see langword="null"/> or <see langword="empty"/>, the health check will resolve the <see cref="SeqConnection"/> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the base url of the Seq instance to check.
    /// </summary>
    /// <remarks>
    /// This option is only required when <see cref="Mode"/> is set to <see cref="SeqClientCreationMode.ServerUrl"/>.
    /// </remarks>
    public Uri? ServerUrl { get; set; }

    /// <summary>
    /// Gets or sets the api key used to authenticate with the Seq instance.
    /// </summary>
    /// <remarks>
    /// This option is optional and only used when <see cref="Mode"/> is set to <see cref="SeqClientCreationMode.ServerUrl"/>.
    /// </remarks>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the timeout to use when connecting and executing tasks against the service.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>100 milliseconds</c>.
    /// <br/>
    /// Values below <see cref="Timeout.Infinite"/> (-1) are invalid.
    /// </remarks>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the command to execute against the service.
    /// Returns <see langword="true"/> if successful, <see langword="false"/> otherwise.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public Func<SeqConnection, CancellationToken, Task<bool>> CommandAsync { get; set; } =
        SeqHealthCheck.DefaultCommandAsync;
}
