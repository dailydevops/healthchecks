namespace NetEvolve.HealthChecks.Npgsql.Devart;

using global::Devart.Data.PostgreSql;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(PgSqlConnection), typeof(NpgsqlDevartOptions), false)]
internal sealed partial class NpgsqlDevartHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
