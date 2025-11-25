namespace NetEvolve.HealthChecks.Minio;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

internal sealed class MinioConfigure : IConfigureNamedOptions<MinioOptions>, IValidateOptions<MinioOptions>
{
    private readonly IConfiguration _configuration;

    public MinioConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, MinioOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Minio:{name}", options);
    }

    public void Configure(MinioOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, MinioOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ValidateOptionsResult.Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return ValidateOptionsResult.Fail("The option cannot be null.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return ValidateOptionsResult.Fail(
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout."
            );
        }

        if (string.IsNullOrWhiteSpace(options.BucketName))
        {
            return ValidateOptionsResult.Fail("The bucket name cannot be null or whitespace.");
        }

        return ValidateOptionsResult.Success;
    }
}
