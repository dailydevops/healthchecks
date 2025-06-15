namespace NetEvolve.HealthChecks.AWS.SQS;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;

internal sealed class SimpleQueueServiceConfigure
    : IConfigureNamedOptions<SimpleQueueServiceOptions>,
        IValidateOptions<SimpleQueueServiceOptions>
{
    private readonly IConfiguration _configuration;

    public SimpleQueueServiceConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, SimpleQueueServiceOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AWSSQS:{name}", options);
    }

    public void Configure(SimpleQueueServiceOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, SimpleQueueServiceOptions options)
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

        if (string.IsNullOrWhiteSpace(options.QueueName))
        {
            return ValidateOptionsResult.Fail("The queue name cannot be null or whitespace.");
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
