namespace NetEvolve.HealthChecks.SQLite;

using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Options for <see cref="SQLiteCheck"/>
/// </summary>
public sealed class SQLiteOptions : ISqlCheckOptions
{
    /// <inheritdoc cref="ISqlCheckOptions.ConnectionString"/>
    public string ConnectionString { get; set; } = default!;

    /// <inheritdoc cref="ISqlCheckOptions.Timeout"/>
    public int Timeout { get; set; } = 100;

    /// <inheritdoc cref="ISqlCheckOptions.Command"/>
    public string Command { get; internal set; } = SQLiteCheck.DefaultCommand;
}
