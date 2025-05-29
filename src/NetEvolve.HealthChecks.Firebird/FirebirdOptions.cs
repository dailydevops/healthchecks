namespace NetEvolve.HealthChecks.Firebird;

using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Options for <see cref="FirebirdHealthCheck"/>
/// </summary>
public sealed record FirebirdOptions : ISqlCheckOptions
{
    /// <inheritdoc cref="ISqlCheckOptions.ConnectionString"/>
    public string ConnectionString { get; set; } = default!;

    /// <inheritdoc cref="ISqlCheckOptions.Timeout"/>
    public int Timeout { get; set; } = 100;

    /// <inheritdoc cref="ISqlCheckOptions.Command"/>
    public string Command { get; internal set; } = FirebirdHealthCheck.DefaultCommand;
}
