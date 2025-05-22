namespace NetEvolve.HealthChecks.Oracle;

using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Options for <see cref="OracleCheck"/>
/// </summary>
public sealed record OracleOptions : ISqlCheckOptions
{
    /// <inheritdoc cref="ISqlCheckOptions.ConnectionString"/>
    public string ConnectionString { get; set; } = default!;

    /// <inheritdoc cref="ISqlCheckOptions.Timeout"/>
    public int Timeout { get; set; } = 100;

    /// <inheritdoc cref="ISqlCheckOptions.Command"/>
    public string Command { get; internal set; } = OracleCheck.DefaultCommand;
}
