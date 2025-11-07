namespace NetEvolve.HealthChecks.Npgsql;

using global::Npgsql;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(NpgsqlConnection), typeof(NpgsqlOptions), true)]
internal sealed partial class NpgsqlHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
