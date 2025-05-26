namespace NetEvolve.HealthChecks.Azure.ServiceBus;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal class ServiceBusSubscriptionOptionsConfigure
    : IConfigureNamedOptions<ServiceBusSubscriptionOptions>,
        IValidateOptions<ServiceBusSubscriptionOptions>
{
    private readonly IConfiguration _configuration;

    public ServiceBusSubscriptionOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, ServiceBusSubscriptionOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureServiceBusSubscription:{name}", options);
    }

    public void Configure(ServiceBusSubscriptionOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, ServiceBusSubscriptionOptions options)
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

        if (string.IsNullOrWhiteSpace(options.SubscriptionName))
        {
            return Fail("The subscription name cannot be null or whitespace.");
        }

        return Success;
    }
}
