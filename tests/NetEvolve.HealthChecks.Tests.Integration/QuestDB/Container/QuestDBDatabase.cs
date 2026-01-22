namespace NetEvolve.HealthChecks.Tests.Integration.QuestDB.Container;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using TUnit.Core.Interfaces;

public sealed class QuestDBDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly QuestDbContainer _database = new QuestDbBuilder(
        /*dockerimage*/"questdb/questdb:9.3.1"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public Uri StatusUri => new Uri(_database.GetRestApiAddress(), "/status");

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
