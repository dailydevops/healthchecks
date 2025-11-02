namespace NetEvolve.HealthChecks.Firebird;

using FirebirdSql.Data.FirebirdClient;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(FbConnection), typeof(FirebirdOptions), true)]
internal sealed partial class FirebirdHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM RDB$DATABASE;";
}
