namespace NetEvolve.HealthChecks.Milvus;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class MilvusOptionsConfigure : IConfigureNamedOptions<MilvusOptions>, IValidateOptions<MilvusOptions>
{
    private readonly IConfiguration _configuration;

    public MilvusOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, MilvusOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Milvus:{name}", options);
    }

    public void Configure(MilvusOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, MilvusOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        return Success;
    }
}
