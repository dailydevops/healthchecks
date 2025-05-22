namespace NetEvolve.HealthChecks.ClickHouse;

using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Options for <see cref="ClickHouseCheck"/>
/// </summary>
public sealed record ClickHouseOptions : ISqlCheckOptions
{
    /// <inheritdoc cref="ISqlCheckOptions.ConnectionString"/>
    public string ConnectionString { get; set; } = default!;

    /// <inheritdoc cref="ISqlCheckOptions.Timeout"/>
    public int Timeout { get; set; } = 100;

    /// <inheritdoc cref="ISqlCheckOptions.Command"/>
    public string Command { get; internal set; } = ClickHouseCheck.DefaultCommand;
}
