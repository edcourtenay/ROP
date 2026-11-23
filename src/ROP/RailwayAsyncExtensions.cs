using System;
using System.Threading.Tasks;

namespace ROP;

/// <summary>
/// Provides asynchronous composition and task adapters for two-track results.
/// </summary>
public static class RailwayAsyncExtensions
{
    /// <summary>Composes two asynchronous result-producing functions.</summary>
    /// <remarks>The returned function invokes <paramref name="left"/> when called. Its successful value is passed to
    /// <paramref name="right"/>; a failure is propagated without invoking <paramref name="right"/>.</remarks>
    /// <typeparam name="TInput">The input type of the first function.</typeparam>
    /// <typeparam name="TSuccessIn">The success type produced by the first function.</typeparam>
    /// <typeparam name="TSuccessOut">The success type produced by the second function.</typeparam>
    /// <typeparam name="TFailure">The failure type shared by both functions.</typeparam>
    /// <param name="left">The asynchronous first function.</param>
    /// <param name="right">The asynchronous function to invoke on success.</param>
    /// <returns>A function that produces the composed asynchronous result.</returns>
    /// <example><code>
    /// Func&lt;int, Task&lt;Result&lt;int, string&gt;&gt;&gt; first = x =&gt; Task.FromResult&lt;Result&lt;int, string&gt;&gt;(x);
    /// Func&lt;int, Task&lt;Result&lt;int, string&gt;&gt;&gt; next = x =&gt; Task.FromResult&lt;Result&lt;int, string&gt;&gt;(x + 1);
    /// var result = await first.BindAsync(next)(1);
    /// </code></example>
    public static Func<TInput, Task<Result<TSuccessOut, TFailure>>> BindAsync<TInput, TSuccessIn, TSuccessOut, TFailure>(
        this Func<TInput, Task<Result<TSuccessIn, TFailure>>> left, Func<TSuccessIn, Task<Result<TSuccessOut, TFailure>>> right)
    {
        return input => left.Invoke(input).BindAsync(right);
    }

    /// <summary>Asynchronously binds a task containing a result to another result-producing function.</summary>
    /// <remarks>After the input task completes, its success value is passed to <paramref name="switchFunction"/>.
    /// A failure is propagated without invoking the function. Task faults and cancellation propagate to the returned task.</remarks>
    /// <typeparam name="TSuccessIn">The input result's success type.</typeparam>
    /// <typeparam name="TSuccessOut">The function's success type.</typeparam>
    /// <typeparam name="TFailure">The failure type shared by both results.</typeparam>
    /// <param name="twoTrackInput">The task containing the input result.</param>
    /// <param name="switchFunction">The asynchronous function to invoke on success.</param>
    /// <returns>A task containing the function's result or the propagated failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The completed result matches neither the success nor failure type.</exception>
    /// <example><code>
    /// Task&lt;Result&lt;int, string&gt;&gt; input = Task.FromResult&lt;Result&lt;int, string&gt;&gt;(1);
    /// var result = await input.BindAsync(x =&gt; Task.FromResult&lt;Result&lt;int, string&gt;&gt;(x + 1));
    /// </code></example>
    public static Task<Result<TSuccessOut, TFailure>> BindAsync<TSuccessIn, TSuccessOut, TFailure>(this Task<Result<TSuccessIn, TFailure>> twoTrackInput,
        Func<TSuccessIn, Task<Result<TSuccessOut, TFailure>>> switchFunction)
    {
        return twoTrackInput.WhenSuccessful(switchFunction);
    }

    /// <summary>Composes a synchronous result-producing function with an asynchronous one.</summary>
    /// <remarks>The returned function invokes <paramref name="left"/> synchronously when called. Its successful value
    /// is passed to <paramref name="right"/>; a failure is propagated without invoking <paramref name="right"/>.</remarks>
    /// <typeparam name="TInput">The input type of the first function.</typeparam>
    /// <typeparam name="TSuccessIn">The success type produced by the first function.</typeparam>
    /// <typeparam name="TSuccessOut">The success type produced by the second function.</typeparam>
    /// <typeparam name="TFailure">The failure type shared by both functions.</typeparam>
    /// <param name="left">The synchronous first function.</param>
    /// <param name="right">The asynchronous function to invoke on success.</param>
    /// <returns>A function that produces the composed asynchronous result.</returns>
    /// <example><code>
    /// Func&lt;int, Result&lt;int, string&gt;&gt; first = x =&gt; x;
    /// Func&lt;int, Task&lt;Result&lt;int, string&gt;&gt;&gt; next = x =&gt; Task.FromResult&lt;Result&lt;int, string&gt;&gt;(x + 1);
    /// var result = await first.BindAsync(next)(1);
    /// </code></example>
    public static Func<TInput, Task<Result<TSuccessOut, TFailure>>> BindAsync<TInput, TSuccessIn, TSuccessOut, TFailure>(this Func<TInput, Result<TSuccessIn, TFailure>> left,
        Func<TSuccessIn, Task<Result<TSuccessOut, TFailure>>> right)
    {
        return input => left.Invoke(input).BindAsync(right);
    }

    /// <summary>Asynchronously binds a result to another result-producing function.</summary>
    /// <remarks>The success value is passed to <paramref name="asyncSwitchFunction"/>; a failure is propagated
    /// without invoking it. Faults and cancellation from the function's task propagate to the returned task.</remarks>
    /// <typeparam name="TSuccessIn">The input result's success type.</typeparam>
    /// <typeparam name="TSuccessOut">The function's success type.</typeparam>
    /// <typeparam name="TFailure">The failure type shared by both results.</typeparam>
    /// <param name="twoTrackInput">The input result.</param>
    /// <param name="asyncSwitchFunction">The asynchronous function to invoke on success.</param>
    /// <returns>A task containing the function's result or the propagated failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The input result matches neither the success nor failure type.</exception>
    /// <example><code>
    /// Result&lt;int, string&gt; input = 1;
    /// var result = await input.BindAsync(x =&gt; Task.FromResult&lt;Result&lt;int, string&gt;&gt;(x + 1));
    /// </code></example>
    public static Task<Result<TSuccessOut, TFailure>> BindAsync<TSuccessIn, TSuccessOut, TFailure>(this Result<TSuccessIn, TFailure> twoTrackInput,
        Func<TSuccessIn, Task<Result<TSuccessOut, TFailure>>> asyncSwitchFunction)
    {
        return twoTrackInput.ToAsync().BindAsync(asyncSwitchFunction);
    }

    /// <summary>Adapts a synchronous result-producing function to return a task.</summary>
    /// <remarks>The original function runs synchronously when the returned function is called; its result is wrapped
    /// in an already completed task. Exceptions thrown by the original function are thrown synchronously.</remarks>
    /// <typeparam name="TInput">The function's input type.</typeparam>
    /// <typeparam name="TSuccess">The result's success type.</typeparam>
    /// <typeparam name="TFailure">The result's failure type.</typeparam>
    /// <param name="twoTrackFunction">The synchronous function to adapt.</param>
    /// <returns>A function that wraps each result in a completed task.</returns>
    /// <example><code>
    /// Func&lt;int, Result&lt;int, string&gt;&gt; compute = x =&gt; x + 1;
    /// var result = await compute.ToAsync()(1);
    /// </code></example>
    public static Func<TInput, Task<Result<TSuccess, TFailure>>> ToAsync<TInput, TSuccess, TFailure>(this Func<TInput, Result<TSuccess, TFailure>> twoTrackFunction)
    {
        return input => twoTrackFunction.Invoke(input).ToAsync();
    }

    /// <summary>Wraps a result in an already completed task.</summary>
    /// <typeparam name="TSuccess">The result's success type.</typeparam>
    /// <typeparam name="TFailure">The result's failure type.</typeparam>
    /// <param name="twoTrackInput">The result to wrap, whether successful or failed.</param>
    /// <returns>A completed task containing the original result.</returns>
    /// <example><code>
    /// Result&lt;int, string&gt; input = 1;
    /// var result = await input.ToAsync();
    /// </code></example>
    public static Task<Result<TSuccess, TFailure>> ToAsync<TSuccess, TFailure>(this Result<TSuccess, TFailure> twoTrackInput)
    {
        return Task.FromResult(twoTrackInput);
    }

    private static async Task<Result<TSuccessOut, TFailure>> WhenSuccessful<TSuccessIn, TSuccessOut, TFailure>(this Task<Result<TSuccessIn, TFailure>> twoTrackInput,
        Func<TSuccessIn, Task<Result<TSuccessOut, TFailure>>> func)
    {
        return await twoTrackInput switch
        {
            TSuccessIn success => await func(success),
            TFailure failure => failure,
            _ => throw new ArgumentOutOfRangeException(nameof(twoTrackInput))
        };
    }
}