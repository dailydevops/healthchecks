namespace NetEvolve.HealthChecks.AWS.SNS;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

internal sealed class SimpleNotificationServiceConfigure
    : IConfigureNamedOptions<SimpleNotificationServiceOptions>,
        IValidateOptions<SimpleNotificationServiceOptions>
{
    private readonly IConfiguration _configuration;

    public SimpleNotificationServiceConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, SimpleNotificationServiceOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AWSSNS:{name}", options);
    }

    public void Configure(SimpleNotificationServiceOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, SimpleNotificationServiceOptions options)
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
            return ValidateOptionsResult.Fail("The timeout cannot be less than infinite (-1).");
        }

        if (string.IsNullOrWhiteSpace(options.TopicName))
        {
            return ValidateOptionsResult.Fail("The topic name cannot be null or whitespace.");
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
