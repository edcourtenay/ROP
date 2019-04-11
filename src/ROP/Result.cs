namespace ROP
{
    public abstract class Result<TSuccess, TFailure>
    {
        private Result()
        {
        }

        public static Result<TSuccess, TFailure> NewSuccess(TSuccess item) => new Success(item);

        public static Result<TSuccess, TFailure> NewFailure(TFailure item) => new Failure(item);

        public bool IsSuccess => this is Success;

        public bool IsFailure => this is Failure;

        public Success AsSuccess => this as Success;

        public Failure AsFailure => this as Failure;

        public class Success : Result<TSuccess, TFailure>
        {
            internal Success(TSuccess item)
            {
                Item = item;
            }

            public TSuccess Item { get; }

            public override string ToString() => $"SUCCESS: {Item.ToString()}";

            public static implicit operator TSuccess(Success success) => success.Item;
        }

        public class Failure : Result<TSuccess, TFailure>
        {
            internal Failure(TFailure item)
            {
                Item = item;
            }

            public TFailure Item { get; }

            public override string ToString() => $"FAILURE: {Item.ToString()}";

            public static implicit operator TFailure(Failure failure) => failure.Item;
        }
    }
}
