namespace NetEvolve.HealthChecks.Seq;

using global::Seq.Api;

/// <summary>
/// Describes the mode used to create the <see cref="SeqConnection"/>.
/// </summary>
public enum SeqClientCreationMode
{
    /// <summary>
    /// The <see cref="SeqConnection"/> preregistered instance is retrieved from the <see cref="System.IServiceProvider"/>.
    /// </summary>
    /// <remarks>
    /// This is the default mode.
    /// </remarks>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="SeqConnection"/> instance is created using the <see cref="SeqOptions.ServerUrl"/>
    /// and the optional <see cref="SeqOptions.ApiKey"/>.
    /// </summary>
    ServerUrl = 1,
}
