namespace NetEvolve.HealthChecks.Npgsql;

using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Options for <see cref="NpgsqlCheck"/>
/// </summary>
public sealed record NpgsqlOptions : ISqlCheckOptions
{
    /// <inheritdoc cref="ISqlCheckOptions.ConnectionString"/>
    public string ConnectionString { get; set; } = default!;

    /// <inheritdoc cref="ISqlCheckOptions.Timeout"/>
    public int Timeout { get; set; } = 100;

    /// <inheritdoc cref="ISqlCheckOptions.Command"/>
    public string Command { get; internal set; } = NpgsqlCheck.DefaultCommand;
}
