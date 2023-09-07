namespace NetEvolve.HealthChecks.SQLite.Tests.Integration;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

[ExcludeFromCodeCoverage]
public sealed class SQLiteDatabase : IAsyncLifetime
{
    public string ConnectionString => "Data Source=:memory:";

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => Task.CompletedTask;
}
