namespace NetEvolve.HealthChecks.Tests.Integration.DB2;

using System.Threading.Tasks;
using Testcontainers.Db2;

public sealed class DB2Database : IAsyncLifetime
{
    private readonly Db2Container _database = new Db2Builder().WithAcceptLicenseAgreement(true).Build();

    public string ConnectionString => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
