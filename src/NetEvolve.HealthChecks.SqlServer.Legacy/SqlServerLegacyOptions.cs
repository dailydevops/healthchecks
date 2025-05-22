namespace NetEvolve.HealthChecks.SqlServer.Legacy;

using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Options for <see cref="SqlServerLegacyCheck"/>
/// </summary>
public sealed record SqlServerLegacyOptions : ISqlCheckOptions
{
    /// <inheritdoc cref="ISqlCheckOptions.ConnectionString"/>
    public string ConnectionString { get; set; } = default!;

    /// <inheritdoc cref="ISqlCheckOptions.Timeout"/>
    public int Timeout { get; set; } = 100;

    /// <inheritdoc cref="ISqlCheckOptions.Command"/>
    public string Command { get; internal set; } = SqlServerLegacyCheck.DefaultCommand;
}
