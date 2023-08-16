namespace NetEvolve.Extensions.Tasks.Tests.Unit;

using System;
using System.Threading.Tasks;
using Xunit;

public class TaskOfTExtensionsTests
{
    [Fact]
    public async Task WithTimeoutAsync_ParamTodoNull_ArgumentNullException()
    {
        Task<bool> task = null!;

        _ = await Assert
            .ThrowsAsync<ArgumentNullException>(
                "todo",
                async () => await task!.WithTimeoutAsync(100).ConfigureAwait(false)
            ).ConfigureAwait(false);
    }

    [Fact]
    public async Task WithTimeoutAsync_IsValidTrue_Expected()
    {
        var timeoutInMilliseconds = 50;

        var (isValid, result) = await TestMethod().WithTimeoutAsync(timeoutInMilliseconds).ConfigureAwait(false);
        Assert.True(isValid);
        Assert.Equal(1, result);

        static async Task<int> TestMethod()
        {
            await Task.Delay(20).ConfigureAwait(false);
            return 1;
        }
    }

    [Fact]
    public async Task WithTimeoutAsync_IsValidFalse_Expected()
    {
        var timeoutInMilliseconds = 20;

        var (isValid, result) = await TestMethod().WithTimeoutAsync(timeoutInMilliseconds).ConfigureAwait(false);
        Assert.False(isValid);
        Assert.Equal(1, result);

        static async Task<int> TestMethod()
        {
            await Task.Delay(50).ConfigureAwait(false);
            return 1;
        }
    }
}
