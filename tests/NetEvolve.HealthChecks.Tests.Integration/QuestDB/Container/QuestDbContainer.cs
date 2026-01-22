namespace NetEvolve.HealthChecks.Tests.Integration.QuestDB.Container;

using System.Globalization;
using DotNet.Testcontainers.Containers;
using global::Keycloak.Net.Models.Root;
using NetEvolve.Extensions.TUnit;

/// <inheritdoc cref="DockerContainer" />
public sealed class QuestDbContainer : DockerContainer, IDatabaseContainer
{
    private readonly QuestDbConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public QuestDbContainer(QuestDbConfiguration configuration)
        : base(configuration) => _configuration = configuration;

    /// <summary>
    /// Gets the PostgreSQL connection string for SQL queries.
    /// </summary>
    /// <returns>The PostgreSQL wire protocol connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>
        {
            { "Host", Hostname },
            { "Port", GetMappedPublicPort(QuestDbBuilder.QuestDbPgPort).ToString(CultureInfo.InvariantCulture) },
            { "Database", QuestDbBuilder.DefaultDatabase },
            { "Username", _configuration.Username },
            { "Password", _configuration.Password },
            { "Server Compatibility Mode", "NoTypeLoading" },
        };
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Gets the REST API base address.
    /// </summary>
    /// <returns>The REST API base address.</returns>
    public Uri GetRestApiAddress() =>
        new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(QuestDbBuilder.QuestDbHttpPort)).Uri;

    /// <summary>
    /// Gets the HTTP API port.
    /// </summary>
    /// <returns>The mapped HTTP API port.</returns>
    public int GetHttpApiPort() => GetMappedPublicPort(QuestDbBuilder.QuestDbHttpPort);

    /// <summary>
    /// Gets the Web Console URL.
    /// </summary>
    /// <returns>The Web Console URL.</returns>
    public Uri GetWebConsoleUrl() => GetRestApiAddress();

    /// <summary>
    /// Gets the connection string for the official QuestDB .NET client (net-questdb-client).
    /// </summary>
    /// <param name="useHttpTransport">If true, uses HTTP transport; otherwise uses TCP (default).</param>
    /// <returns>The QuestDB client connection string in format "protocol::addr=host:port;".</returns>
    /// <remarks>
    /// TCP example: tcp::addr=localhost:9009;
    /// HTTP example: http::addr=localhost:9000;
    /// </remarks>
    public string GetClientConnectionString(bool useHttpTransport = false)
    {
        if (useHttpTransport)
        {
            return $"http::addr={Hostname}:{GetMappedPublicPort(QuestDbBuilder.QuestDbHttpPort)};";
        }

        return $"tcp::addr={Hostname}:{GetMappedPublicPort(QuestDbBuilder.QuestDbInfluxLinePort)};";
    }

    /// <summary>
    /// Gets the InfluxDB Line Protocol (ILP) host.
    /// </summary>
    /// <returns>The ILP host.</returns>
    public string InfluxLineProtocolHost => Hostname;

    /// <summary>
    /// Gets the InfluxDB Line Protocol (ILP) port.
    /// </summary>
    /// <returns>The ILP port.</returns>
    public int GetInfluxLineProtocolPort() => GetMappedPublicPort(QuestDbBuilder.QuestDbInfluxLinePort);
}
