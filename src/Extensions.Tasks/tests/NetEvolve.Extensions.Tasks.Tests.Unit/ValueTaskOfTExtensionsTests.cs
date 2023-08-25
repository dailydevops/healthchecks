namespace NetEvolve.Extensions.Tasks.Tests.Unit;

using NetEvolve.Extensions.XUnit;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

[UnitTest]
[ExcludeFromCodeCoverage]
public class ValueTaskOfTExtensionsTests
{
    [Fact]
    public async Task WithTimeoutAsync_IsValidTrue_Expected()
    {
        var timeoutInMilliseconds = 50;

        var (isValid, result) = await TestMethod()
            .WithTimeoutAsync(timeoutInMilliseconds)
            .ConfigureAwait(false);
        Assert.True(isValid);
        Assert.Equal(1, result);

        static async ValueTask<int> TestMethod()
        {
            await Task.Delay(20).ConfigureAwait(false);
            return 1;
        }
    }

    [Fact]
    public async Task WithTimeoutAsync_IsValidFalse_Expected()
    {
        var timeoutInMilliseconds = 20;

        var (isValid, result) = await TestMethod()
            .WithTimeoutAsync(timeoutInMilliseconds)
            .ConfigureAwait(false);
        Assert.False(isValid);
        Assert.Equal(1, result);

        static async ValueTask<int> TestMethod()
        {
            await Task.Delay(50).ConfigureAwait(false);
            return 1;
        }
    }
}
