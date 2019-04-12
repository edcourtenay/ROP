using System;

namespace ROP
{
    public static class RailwayExtensions
    {
        public static Result<TB, TC> Bind<TA, TB, TC>(this Result<TA, TC> twoTrackInput,
            Func<TA, Result<TB, TC>> switchFunction)
        {
            return twoTrackInput.IsSuccess
                ? switchFunction(twoTrackInput.AsSuccess.Item)
                : Result<TB, TC>.NewFailure(twoTrackInput.AsFailure.Item);
        }

        public static Func<TA, Result<TC, TD>> Bind<TA, TB, TC, TD>(this Func<TA, Result<TB, TD>> twoTrackInputFunction,
            Func<TB, Result<TC, TD>> switchFunction)
        {
            return input => twoTrackInputFunction.Invoke(input).Bind(switchFunction);
        }

        public static Result<TA, TB> Lift<TA, TB>(this TA input)
        {
            return input.Switch<TA, TA, TB>(x => x);
        }

        public static Result<TB, TC> Map<TA, TB, TC>(this Result<TA, TC> twoTrackInput, Func<TA, TB> oneTrackFunction)
        {
            return twoTrackInput.IsSuccess
                ? Result<TB, TC>.NewSuccess(oneTrackFunction.Invoke(twoTrackInput.AsSuccess.Item))
                : Result<TB, TC>.NewFailure(twoTrackInput.AsFailure.Item);
        }

        public static Func<TA, Result<TC, TD>> Map<TA, TB, TC, TD>(this Func<TA, Result<TB, TD>> twoTrackInputFunction,
            Func<TB, TC> oneTrackFunction)
        {
            return input => twoTrackInputFunction.Invoke(input).Map(oneTrackFunction);
        }

        public static Result<TB, TC> Switch<TA, TB, TC>(this TA input, Func<TA, TB> oneTrackFunction)
        {
            return Result<TB, TC>.NewSuccess(oneTrackFunction.Invoke(input));
        }

        public static Func<TA, Result<TB, TC>> Switch<TA, TB, TC>(this Func<TA, TB> func)
        {
            return input => input.Switch<TA, TB, TC>(func);
        }

        public static Result<TA, TB> Tee<TA, TB>(this Result<TA, TB> input, Action<TA> teeAction)
        {
            if (input is Result<TA, TB>.Success success)
            {
                teeAction.Invoke(success);
            }

            return input;
        }

        public static Func<TA, Result<TB, TC>> Tee<TA, TB, TC>(this Func<TA, Result<TB, TC>> result,
            Action<TB> teeAction)
        {
            return input => result.Invoke(input).Tee(teeAction);
        }

        public static Result<TA, TB> TeeFailure<TA, TB>(this Result<TA, TB> input, Action<TB> teeAction)
        {
            if (input is Result<TA, TB>.Failure failure)
            {
                teeAction.Invoke(failure);
            }

            return input;
        }

        public static Func<TA, Result<TB, TC>> TeeFailure<TA, TB, TC>(this Func<TA, Result<TB, TC>> result,
            Action<TC> teeAction)
        {
            return input => result.Invoke(input).TeeFailure(teeAction);
        }

        public static void Handle<TA, TB>(this Result<TA, TB> twoTrackInput, Action<TA> onSuccess, Action<TB> onFailure = null)
        {
            if (twoTrackInput is Result<TA, TB>.Success success)
            {
                onSuccess(success);
            }
            else if (onFailure != null && twoTrackInput is Result<TA, TB>.Failure failure)
            {
                onFailure(failure);
            }
        }

        public static Action<TA> Handle<TA, TB, TC>(this Func<TA, Result<TB, TC>> twoTrackInputFunction, Action<TB> onSuccess, Action<TC> onFailure = null)
        {
            return input => twoTrackInputFunction.Invoke(input).Handle(onSuccess, onFailure);
        }
    }
}
