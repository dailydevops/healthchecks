namespace NetEvolve.HealthChecks.DuckDB;

using global::DuckDB.NET.Data;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(DuckDBConnection), typeof(DuckDBOptions), true)]
internal sealed partial class DuckDBHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
