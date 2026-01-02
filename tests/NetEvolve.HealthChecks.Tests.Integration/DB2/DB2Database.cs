namespace NetEvolve.HealthChecks.Tests.Integration.DB2;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Db2;

public sealed class DB2Database : IAsyncInitializer, IAsyncDisposable
{
    private readonly Db2Container _database = new Db2Builder(
        /*dockerimage*/"icr.io/db2_community/db2:12.1.0.0"
    )
        .WithLogger(NullLogger.Instance)
        .WithAcceptLicenseAgreement(true)
        .Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
