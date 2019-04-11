using System;
using System.Threading.Tasks;

namespace ROP
{
    public static class RailwayAsyncExtensions
    {
        public static Func<TA, Task<Result<TC, TD>>> BindAsync<TA, TB, TC, TD>(
            this Func<TA, Task<Result<TB, TD>>> left, Func<TB, Task<Result<TC, TD>>> right)
        {
            return input => left.Invoke(input).BindAsync(right);
        }

        public static Task<Result<TB, TC>> BindAsync<TA, TB, TC>(this Task<Result<TA, TC>> twoTrackInput,
            Func<TA, Task<Result<TB, TC>>> switchFunction)
        {
            return WhenSuccessful(twoTrackInput, switchFunction);
        }

        public static Func<TA, Task<Result<TC, TD>>> BindAsync<TA, TB, TC, TD>(this Func<TA, Result<TB, TD>> left,
            Func<TB, Task<Result<TC, TD>>> right)
        {
            return input => left.Invoke(input).BindAsync(right);
        }


        public static Task<Result<TB, TC>> BindAsync<TA, TB, TC>(this Result<TA, TC> twoTrackInput,
            Func<TA, Task<Result<TB, TC>>> asyncSwitchFunction)
        {
            return twoTrackInput.ToAsync().BindAsync(asyncSwitchFunction);
        }

        public static Func<TA, Task<Result<TB, TC>>> ToAsync<TA, TB, TC>(this Func<TA, Result<TB, TC>> twoTrackFunction)
        {
            return input => twoTrackFunction.Invoke(input).ToAsync();
        }

        public static Task<Result<TA, TB>> ToAsync<TA, TB>(this Result<TA, TB> twoTrackInput)
        {
            return Task.FromResult(twoTrackInput);
        }

        private static async Task<Result<TS, TF>> WhenSuccessful<TA, TS, TF>(this Task<Result<TA, TF>> twoTrackInput,
            Func<TA, Task<Result<TS, TF>>> func)
        {
            var input = await twoTrackInput;

            return input.IsSuccess
                ? await func(input.AsSuccess.Item)
                : Result<TS, TF>.NewFailure(input.AsFailure.Item);
        }
    }
}
