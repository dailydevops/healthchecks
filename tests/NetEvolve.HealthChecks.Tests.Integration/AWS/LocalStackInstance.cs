namespace NetEvolve.HealthChecks.Tests.Integration.AWS;

using System.Threading.Tasks;
using Testcontainers.LocalStack;
using TestContainer = Testcontainers.LocalStack.LocalStackContainer;

public sealed class LocalStackInstance : IAsyncLifetime
{
    /// <summary>Access Key</summary>
    /// <see href="https://docs.localstack.cloud/references/credentials/#access-key-id" />
    internal const string AccessKey = "LSIAQAAAAAAVNCBMPNSG";
    internal const string SecretKey = "test";

    private readonly TestContainer _container = new LocalStackBuilder()
        .WithEnvironment("AWS_ACCESS_KEY_ID", AccessKey)
        .WithEnvironment("AWS_SECRET_ACCESS_KEY", SecretKey)
        .Build();

    internal string ConnectionString => _container.GetConnectionString();

    public async Task DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
