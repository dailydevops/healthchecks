namespace NetEvolve.HealthChecks.AWS.EC2;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

internal sealed class ElasticComputeCloudConfigure
    : IConfigureNamedOptions<ElasticComputeCloudOptions>,
        IValidateOptions<ElasticComputeCloudOptions>
{
    private readonly IConfiguration _configuration;

    public ElasticComputeCloudConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, ElasticComputeCloudOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AWSEC2:{name}", options);
    }

    public void Configure(ElasticComputeCloudOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, ElasticComputeCloudOptions options)
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

        if (options.Mode is CreationMode.BasicAuthentication)
        {
            if (string.IsNullOrWhiteSpace(options.AccessKey))
            {
                return ValidateOptionsResult.Fail("The access key cannot be null or whitespace.");
            }
            if (string.IsNullOrWhiteSpace(options.SecretKey))
            {
                return ValidateOptionsResult.Fail("The secret key cannot be null or whitespace.");
            }
        }

        return ValidateOptionsResult.Success;
    }
}
