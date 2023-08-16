namespace NetEvolve.Extensions.Tasks;

using System;
using System.Threading;
using System.Threading.Tasks;

public static class TaskOfTExtensions
{
    public static async Task<(bool isValid, T result)> WithTimeoutAsync<T>(
        this Task<T> todo,
        int timeoutInMilliseconds,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(todo);

        var winner = await Task.WhenAny(todo, Task.Delay(timeoutInMilliseconds, cancellationToken))
            .ConfigureAwait(false);
        var result = await todo.ConfigureAwait(false);
        return (winner == todo, result);
    }
}
