namespace NetEvolve.HealthChecks.Tests.Integration.SQLite;

using System.Threading.Tasks;
using Xunit;

public sealed class SQLiteDatabase : IAsyncLifetime
{
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    public string ConnectionString => "Data Source=:memory:";
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => Task.CompletedTask;
}
