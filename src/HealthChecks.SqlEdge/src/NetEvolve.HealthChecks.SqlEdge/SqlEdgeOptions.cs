namespace NetEvolve.HealthChecks.SqlEdge;

using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Options for <see cref="SqlEdgeCheck"/>
/// </summary>
public sealed class SqlEdgeOptions : ISqlCheckOptions
{
    /// <inheritdoc cref="ISqlCheckOptions.ConnectionString"/>
    public string ConnectionString { get; set; } = default!;

    /// <inheritdoc cref="ISqlCheckOptions.Timeout"/>
    public int Timeout { get; set; } = 100;

    /// <inheritdoc cref="ISqlCheckOptions.Command"/>
    public string Command { get; internal set; } = SqlEdgeCheck.DefaultCommand;
}
