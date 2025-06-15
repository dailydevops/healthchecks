namespace NetEvolve.HealthChecks.Azure.ServiceBus;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal class ServiceBusTopicOptionsConfigure
    : IConfigureNamedOptions<ServiceBusTopicOptions>,
        IValidateOptions<ServiceBusTopicOptions>
{
    private readonly IConfiguration _configuration;

    public ServiceBusTopicOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, ServiceBusTopicOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureServiceBusTopic:{name}", options);
    }

    public void Configure(ServiceBusTopicOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, ServiceBusTopicOptions options)
    {
        var result = ServiceBusOptionsBase.InternalValidate(name, options);

        if (result is not null)
        {
            return result;
        }

        if (string.IsNullOrWhiteSpace(options.TopicName))
        {
            return Fail("The topic name cannot be null or whitespace.");
        }

        return Success;
    }
}
