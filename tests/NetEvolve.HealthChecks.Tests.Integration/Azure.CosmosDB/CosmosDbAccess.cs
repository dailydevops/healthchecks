namespace NetEvolve.HealthChecks.Tests.Integration.Azure.CosmosDB;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Azure.Cosmos;

public sealed class CosmosDbAccess : IAsyncInitializer, IAsyncDisposable
{
    private const ushort CosmosDbPort = 8081;

    private const string Hostname = "localhost";

    private const string DefaultAccountKey =
        "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    private readonly IContainer _container = new ContainerBuilder()
        .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:vnext-preview")
        .WithEnvironment("ENABLE_EXPLORER", "false")
        .WithPortBinding(CosmosDbPort, true)
        .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitForMe()))
        .Build();

    public string ConnectionString
    {
        get
        {
            var properties = new Dictionary<string, string>
            {
                {
                    "AccountEndpoint",
                    new UriBuilder(Uri.UriSchemeHttp, Hostname, _container.GetMappedPublicPort(CosmosDbPort)).ToString()
                },
                { "AccountKey", DefaultAccountKey },
            };
            return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
        }
    }

    public Uri AccountEndpoint => new UriBuilder(Uri.UriSchemeHttp, Hostname, _container.GetMappedPublicPort(CosmosDbPort)).Uri;

    public static string AccountKey => DefaultAccountKey;

    public CosmosClient CosmosClient
    {
        get
        {
            var clientOptions = new CosmosClientOptions();
            ClientConfiguration(clientOptions);

            return new CosmosClient(ConnectionString, clientOptions);
        }
    }

    public Action<CosmosClientOptions> ClientConfiguration =>
        options =>
        {
            options.ConnectionMode = ConnectionMode.Gateway;
            options.HttpClientFactory = () =>
                new(new UriRewriter(_container.Hostname, _container.GetMappedPublicPort(CosmosDbPort)));
        };

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        using var client = CosmosClient;
        _ = await client.CreateDatabaseIfNotExistsAsync("testdb").ConfigureAwait(false);
        var database = client.GetDatabase("testdb");
        _ = await database
            .CreateContainerIfNotExistsAsync(new ContainerProperties("testcontainer", "/id"))
            .ConfigureAwait(false);
    }

    private sealed class WaitForMe : IWaitUntil
    {
        public async Task<bool> UntilAsync(IContainer container)
        {
            using var rewriter = new UriRewriter(container.Hostname, container.GetMappedPublicPort(CosmosDbPort));
            using var client = new HttpClient(rewriter);
            try
            {
                using var httpResponse = await client.GetAsync(new Uri("http://localhost")).ConfigureAwait(false);

                if (httpResponse.IsSuccessStatusCode)
                {
                    await Task.Delay(1_000);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }

    private sealed class UriRewriter : DelegatingHandler
    {
        private readonly string _hostname;

        private readonly ushort _port;

        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test case")]
        public UriRewriter(string hostname, ushort port)
            : base(new HttpClientHandler())
        {
            _hostname = hostname;
            _port = port;
        }

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            request.RequestUri = new UriBuilder(
                Uri.UriSchemeHttp,
                _hostname,
                _port,
                request.RequestUri?.PathAndQuery
            ).Uri;
            return base.SendAsync(request, cancellationToken);
        }
    }
}
