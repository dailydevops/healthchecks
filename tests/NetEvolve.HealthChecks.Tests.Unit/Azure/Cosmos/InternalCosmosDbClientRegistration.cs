namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Cosmos;

using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection;

internal static class InternalCosmosDbClientRegistration
{
    public static IServiceCollection AddCosmosDbClient(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
        {
            var clientBuilder = new CosmosClientBuilder("test");
            return clientBuilder.WithConnectionModeDirect().Build();
        });

        return services;
    }
}
