namespace NetEvolve.HealthChecks.CockroachDb;

using global::Npgsql;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(NpgsqlConnection), typeof(CockroachDbOptions), true)]
internal sealed partial class CockroachDbHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
