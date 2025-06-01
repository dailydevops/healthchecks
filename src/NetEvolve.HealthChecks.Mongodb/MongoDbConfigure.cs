namespace NetEvolve.HealthChecks.MongoDb;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class MongoDbConfigure
    : IConfigureNamedOptions<MongoDbOptions>,
        IPostConfigureOptions<MongoDbOptions>,
        IValidateOptions<MongoDbOptions>
{
    private readonly IConfiguration _configuration;

    public MongoDbConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, MongoDbOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:MongoDb:{name}", options);
    }

    public void Configure(MongoDbOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, MongoDbOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (options.CommandAsync is null)
        {
            options.CommandAsync = MongoDbHealthCheck.DefaultCommandAsync;
        }
    }

    public ValidateOptionsResult Validate(string? name, MongoDbOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail("The connection string cannot be null or whitespace.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout cannot be less than infinite (-1).");
        }

        return Success;
    }
}
