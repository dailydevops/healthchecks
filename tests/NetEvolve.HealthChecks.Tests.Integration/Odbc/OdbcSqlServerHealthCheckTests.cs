namespace NetEvolve.HealthChecks.Tests.Integration.Odbc;

using System.Data.Odbc;
using Microsoft.Data.SqlClient;
using NetEvolve.HealthChecks.Tests.Integration.SqlServer;

[ClassDataSource<SqlServerDatabase>(Shared = InstanceSharedType.SqlServer)]
[InheritsTests]
public class OdbcSqlServerHealthCheckTests : OdbcHealthCheckTestsBase
{
    private readonly SqlServerDatabase _database;
    private string? _connectionString;

    public OdbcSqlServerHealthCheckTests(SqlServerDatabase database) => _database = database;

    protected override string GetConnectionString()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            var sqlBuilder = new SqlConnectionStringBuilder(_database.ConnectionString);
            var odbcBuilder = new OdbcConnectionStringBuilder { Driver = "ODBC Driver 17 for SQL Server" };

            // Map sqlBuilder properties to odbcBuilder
            odbcBuilder["Server"] = sqlBuilder.DataSource;
            odbcBuilder["Database"] = sqlBuilder.InitialCatalog;
            odbcBuilder["UID"] = sqlBuilder.UserID;
            odbcBuilder["PWD"] = sqlBuilder.Password;

            _connectionString = odbcBuilder.ToString();
        }

        return _connectionString;
    }
}
