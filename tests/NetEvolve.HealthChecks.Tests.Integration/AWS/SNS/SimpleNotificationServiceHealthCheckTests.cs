namespace NetEvolve.HealthChecks.Tests.Integration.AWS.SNS;

using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.AWS.SNS;
using NodaTime;

[TestGroup($"{nameof(AWS)}.{nameof(SNS)}")]
public class SimpleNotificationServiceHealthCheckTests : HealthCheckTestBase, IClassFixture<LocalStackInstance>
{
    private readonly LocalStackInstance _instance;

    public SimpleNotificationServiceHealthCheckTests(LocalStackInstance instance)
    {
        _instance = instance;
    }

    [Fact]
    public async Task AddSimpleNotificationService_UseOptionsCreate_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddSimpleNotificationService(
                "TestContainerHealthy",
                options =>
                {
                    options.AccessKey = LocalStackInstance.AccessKey;
                    options.SecretKey = LocalStackInstance.SecretKey;
                    options.ServiceUrl = _instance.ConnectionString;
                }
            );
        });
}
