using System;

namespace ROP;

public static class RailwayExtensions
{
    public static Result<TSuccessOut, TFailure> Bind<TSuccessIn, TSuccessOut, TFailure>(this Result<TSuccessIn, TFailure> twoTrackInput,
        Func<TSuccessIn, Result<TSuccessOut, TFailure>> switchFunction)
    {
        return twoTrackInput switch
        {
            TSuccessIn success => switchFunction(success),
            TFailure failure => failure,
            _ => throw new ArgumentOutOfRangeException(nameof(twoTrackInput), twoTrackInput, null)
        };
    }

    public static Func<TInput, Result<TSuccessOut, TFailure>> Bind<TInput, TSuccessIn, TSuccessOut, TFailure>(this Func<TInput, Result<TSuccessIn, TFailure>> twoTrackInputFunction,
        Func<TSuccessIn, Result<TSuccessOut, TFailure>> switchFunction)
    {
        return input => twoTrackInputFunction(input).Bind(switchFunction);
    }

    public static Result<TSuccess, TFailure> Lift<TSuccess, TFailure>(this TSuccess input)
    {
        return input.Switch<TSuccess, TSuccess, TFailure>(x => x);
    }

    public static Result<TSuccessOut, TFailure> Map<TSuccessIn, TSuccessOut, TFailure>(this Result<TSuccessIn, TFailure> twoTrackInput, Func<TSuccessIn, TSuccessOut> oneTrackFunction)
    {
        return twoTrackInput switch
        {
            TSuccessIn success => oneTrackFunction(success),
            TFailure failure => failure,
            _ => throw new ArgumentOutOfRangeException(nameof(twoTrackInput), twoTrackInput, null)
        };
    }

    public static Func<TInput, Result<TSuccessOut, TFailure>> Map<TInput, TSuccessIn, TSuccessOut, TFailure>(this Func<TInput, Result<TSuccessIn, TFailure>> twoTrackInputFunction,
        Func<TSuccessIn, TSuccessOut> oneTrackFunction)
    {
        return input => twoTrackInputFunction(input).Map(oneTrackFunction);
    }

    public static Result<TSuccess, TFailure> Switch<TInput, TSuccess, TFailure>(this TInput input, Func<TInput, TSuccess> oneTrackFunction)
    {
        return oneTrackFunction(input);
    }

    public static Func<TInput, Result<TSuccess, TFailure>> Switch<TInput, TSuccess, TFailure>(this Func<TInput, TSuccess> func)
    {
        return input => input.Switch<TInput, TSuccess, TFailure>(func);
    }

    public static Result<TSuccess, TFailure> Tee<TSuccess, TFailure>(this Result<TSuccess, TFailure> input, Action<TSuccess> teeAction)
    {
        if (input is TSuccess success)
        {
            teeAction(success);
        }

        return input;
    }

    public static Func<TInput, Result<TSuccess, TFailure>> Tee<TInput, TSuccess, TFailure>(this Func<TInput, Result<TSuccess, TFailure>> result,
        Action<TSuccess> teeAction)
    {
        return input => result(input).Tee(teeAction);
    }

    public static Result<TSuccess, TFailure> TeeFailure<TSuccess, TFailure>(this Result<TSuccess, TFailure> input, Action<TFailure> teeAction)
    {
        if (input is TFailure failure)
        {
            teeAction(failure);
        }

        return input;
    }

    public static Func<TInput, Result<TSuccess, TFailure>> TeeFailure<TInput, TSuccess, TFailure>(this Func<TInput, Result<TSuccess, TFailure>> result,
        Action<TFailure> teeAction)
    {
        return input => result(input).TeeFailure(teeAction);
    }

    public static void Handle<TSuccess, TFailure>(this Result<TSuccess, TFailure> twoTrackInput, Action<TSuccess> onSuccess, Action<TFailure>? onFailure = null)
    {
        switch (twoTrackInput)
        {
            case TSuccess success:
                onSuccess(success);
                break;
            case TFailure failure:
                onFailure?.Invoke(failure);
                break;
        }
    }

    public static Action<TInput> Handle<TInput, TSuccess, TFailure>(this Func<TInput, Result<TSuccess, TFailure>> twoTrackInputFunction, Action<TSuccess> onSuccess, Action<TFailure>? onFailure = null)
    {
        return input => twoTrackInputFunction(input).Handle(onSuccess, onFailure);
    }

    public static TOutput Merge<TSuccess, TFailure, TOutput>(this Result<TSuccess, TFailure> twoTrackInput, Func<TSuccess, TOutput> successFunc,
        Func<TFailure, TOutput> failureFunc)
    {
        return twoTrackInput switch
        {
            TSuccess success => successFunc(success),
            TFailure failure => failureFunc(failure),
            _ => throw new ArgumentOutOfRangeException(nameof(twoTrackInput), twoTrackInput, null)
        };
    }

    public static Func<TInput, TOutput> Merge<TInput, TSuccess, TFailure, TOutput>(this Func<TInput, Result<TSuccess, TFailure>> twoTrackInputFunction,
        Func<TSuccess, TOutput> successFunc, Func<TFailure, TOutput> failureFunc)
    {
        return input => twoTrackInputFunction(input).Merge(successFunc, failureFunc);
    }
}