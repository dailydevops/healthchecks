namespace NetEvolve.HealthChecks.SQLite;

using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Options for <see cref="SQLiteHealthCheck"/>
/// </summary>
public sealed record SQLiteOptions : ISqlCheckOptions
{
    /// <inheritdoc cref="ISqlCheckOptions.ConnectionString"/>
    public string ConnectionString { get; set; } = default!;

    /// <inheritdoc cref="ISqlCheckOptions.Timeout"/>
    public int Timeout { get; set; } = 100;

    /// <inheritdoc cref="ISqlCheckOptions.Command"/>
    public string Command { get; internal set; } = SQLiteHealthCheck.DefaultCommand;
}
