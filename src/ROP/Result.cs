namespace ROP
{
    public abstract record Result<TSuccess, TFailure>
    {
        public bool IsSuccess => this is Success;
        
        public bool IsFailure => this is Failure;

        public Success? AsSuccess => this as Success;

        public Failure? AsFailure => this as Failure;
        
        public record Success(TSuccess Item) : Result<TSuccess, TFailure>
        {
            public override string ToString() => $"SUCCESS: {Item?.ToString()}";

            public static implicit operator TSuccess(Success success) => success.Item;
        }

        public record Failure(TFailure Item) : Result<TSuccess, TFailure>
        {
            public override string ToString() => $"FAILURE: {Item?.ToString()}";

            public static implicit operator TFailure(Failure failure) => failure.Item;
        }
    }
}
