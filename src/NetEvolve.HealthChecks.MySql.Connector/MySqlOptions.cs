namespace NetEvolve.HealthChecks.MySql.Connector;

using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Options for <see cref="MySqlHealthCheck"/>
/// </summary>
public sealed record MySqlOptions : ISqlCheckOptions
{
    /// <inheritdoc cref="ISqlCheckOptions.ConnectionString"/>
    public string ConnectionString { get; set; } = default!;

    /// <inheritdoc cref="ISqlCheckOptions.Timeout"/>
    public int Timeout { get; set; } = 100;

    /// <inheritdoc cref="ISqlCheckOptions.Command"/>
    public string Command { get; internal set; } = MySqlHealthCheck.DefaultCommand;
}
