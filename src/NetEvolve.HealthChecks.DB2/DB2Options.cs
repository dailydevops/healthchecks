namespace NetEvolve.HealthChecks.DB2;

using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Options for <see cref="DB2HealthCheck"/>
/// </summary>
public sealed record DB2Options : ISqlCheckOptions
{
    /// <inheritdoc cref="ISqlCheckOptions.ConnectionString"/>
    public string ConnectionString { get; set; } = default!;

    /// <inheritdoc cref="ISqlCheckOptions.Timeout"/>
    public int Timeout { get; set; } = 100;

    /// <inheritdoc cref="ISqlCheckOptions.Command"/>
    public string Command { get; internal set; } = DB2HealthCheck.DefaultCommand;
}
