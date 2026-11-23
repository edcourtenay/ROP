using System;
using System.Threading.Tasks;

namespace ROP;

public static class RailwayAsyncExtensions
{
    public static Func<TInput, Task<Result<TSuccessOut, TFailure>>> BindAsync<TInput, TSuccessIn, TSuccessOut, TFailure>(
        this Func<TInput, Task<Result<TSuccessIn, TFailure>>> left, Func<TSuccessIn, Task<Result<TSuccessOut, TFailure>>> right)
    {
        return input => left.Invoke(input).BindAsync(right);
    }

    public static Task<Result<TSuccessOut, TFailure>> BindAsync<TSuccessIn, TSuccessOut, TFailure>(this Task<Result<TSuccessIn, TFailure>> twoTrackInput,
        Func<TSuccessIn, Task<Result<TSuccessOut, TFailure>>> switchFunction)
    {
        return twoTrackInput.WhenSuccessful(switchFunction);
    }

    public static Func<TInput, Task<Result<TSuccessOut, TFailure>>> BindAsync<TInput, TSuccessIn, TSuccessOut, TFailure>(this Func<TInput, Result<TSuccessIn, TFailure>> left,
        Func<TSuccessIn, Task<Result<TSuccessOut, TFailure>>> right)
    {
        return input => left.Invoke(input).BindAsync(right);
    }


    public static Task<Result<TSuccessOut, TFailure>> BindAsync<TSuccessIn, TSuccessOut, TFailure>(this Result<TSuccessIn, TFailure> twoTrackInput,
        Func<TSuccessIn, Task<Result<TSuccessOut, TFailure>>> asyncSwitchFunction)
    {
        return twoTrackInput.ToAsync().BindAsync(asyncSwitchFunction);
    }

    public static Func<TInput, Task<Result<TSuccess, TFailure>>> ToAsync<TInput, TSuccess, TFailure>(this Func<TInput, Result<TSuccess, TFailure>> twoTrackFunction)
    {
        return input => twoTrackFunction.Invoke(input).ToAsync();
    }

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