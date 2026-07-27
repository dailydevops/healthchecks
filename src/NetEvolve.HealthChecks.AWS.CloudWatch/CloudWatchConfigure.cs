namespace NetEvolve.HealthChecks.AWS.CloudWatch;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

internal sealed class CloudWatchConfigure
    : IConfigureNamedOptions<CloudWatchOptions>,
        IValidateOptions<CloudWatchOptions>
{
    private readonly IConfiguration _configuration;

    public CloudWatchConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, CloudWatchOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AWSCloudWatch:{name}", options);
    }

    public void Configure(CloudWatchOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, CloudWatchOptions options)
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

        if (string.IsNullOrWhiteSpace(options.AlarmName))
        {
            return ValidateOptionsResult.Fail("The alarm name cannot be null or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(options.ServiceUrl))
        {
            return ValidateOptionsResult.Fail("The service URL cannot be null or whitespace.");
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
