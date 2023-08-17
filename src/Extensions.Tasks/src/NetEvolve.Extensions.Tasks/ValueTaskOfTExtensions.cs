namespace NetEvolve.Extensions.Tasks;

using System.Threading;
using System.Threading.Tasks;

public static class ValueTaskOfTExtensions
{
    public static async ValueTask<(bool isValid, T result)> WithTimeoutAsync<T>(
        this ValueTask<T> todo,
        int timeoutInMilliseconds,
        CancellationToken cancellationToken = default
    )
    {
        var todoTask = todo.AsTask();
        var winner = await Task.WhenAny(
                todoTask,
                Task.Delay(timeoutInMilliseconds, cancellationToken)
            )
            .ConfigureAwait(false);
        var result = await todoTask.ConfigureAwait(false);
        return (winner == todoTask, result);
    }
}
