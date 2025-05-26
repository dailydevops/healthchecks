namespace NetEvolve.HealthChecks.Tests.Unit.Apache.ActiveMq;

using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Apache.ActiveMq;
using Xunit;

[TestGroup($"{nameof(Apache)}.{nameof(ActiveMq)}")]
public sealed class ClientFactoryChecks
{
    [Fact]
    public async Task GetConnectionAsync_WhenOptionsNull_ThrowArgumentNullException()
    {
        // Arrange

        // Act
        static async Task Act() => _ = await ClientFactory.GetConnectionAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("options", Act);
    }

    [Fact]
    public async Task GetConnectionAsync_WhenOptionsBrokerAddressNull_ThrowArgumentException()
    {
        // Arrange
        var options = new ActiveMqOptions { BrokerAddress = null };

        // Act
        async Task Act() => _ = await ClientFactory.GetConnectionAsync(options, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("options.BrokerAddress", Act);
    }

    [Fact]
    public async Task GetConnectionAsync_WhenOptionsBrokerAddressEmpty_ThrowArgumentException()
    {
        // Arrange
        var options = new ActiveMqOptions { BrokerAddress = "" };

        // Act
        async Task Act() => _ = await ClientFactory.GetConnectionAsync(options, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentException>("options.BrokerAddress", Act);
    }
}
