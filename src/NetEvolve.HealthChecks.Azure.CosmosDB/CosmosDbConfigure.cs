namespace NetEvolve.HealthChecks.Azure.CosmosDB;

using Microsoft.Extensions.Configuration;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class CosmosDbConfigure
    : ConfigureOptionsBase<CosmosDbOptions, CosmosDbConfigure>
{
    public CosmosDbConfigure(IConfiguration configuration)
        : base(configuration) { }

    protected override string ConfigurationKey => "HealthChecks:CosmosDb";

    protected override void Configure(CosmosDbOptions options, IConfiguration sectionConfig)
    {
        var connectionString = sectionConfig.GetConnectionString("ConnectionString");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            options.ConnectionString = connectionString;
        }

        var serviceEndpoint = sectionConfig["ServiceEndpoint"];
        if (!string.IsNullOrWhiteSpace(serviceEndpoint))
        {
            options.ServiceEndpoint = serviceEndpoint;
        }

        var accountKey = sectionConfig["AccountKey"];
        if (!string.IsNullOrWhiteSpace(accountKey))
        {
            options.AccountKey = accountKey;
        }

        var databaseName = sectionConfig["DatabaseName"];
        if (!string.IsNullOrWhiteSpace(databaseName))
        {
            options.DatabaseName = databaseName;
        }

        var containerName = sectionConfig["ContainerName"];
        if (!string.IsNullOrWhiteSpace(containerName))
        {
            options.ContainerName = containerName;
        }

        var timeout = sectionConfig["Timeout"];
        if (!string.IsNullOrWhiteSpace(timeout) && int.TryParse(timeout, out var timeoutValue))
        {
            options.Timeout = timeoutValue;
        }

        var mode = sectionConfig["Mode"];
        if (!string.IsNullOrWhiteSpace(mode) && Enum.TryParse<CosmosDbClientCreationMode>(mode, true, out var modeValue))
        {
            options.Mode = modeValue;
        }
    }
}