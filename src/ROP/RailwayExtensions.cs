using System;

namespace ROP;

/// <summary>Provides synchronous operations for composing success and failure tracks.</summary>
/// <remarks>Exceptions thrown by supplied functions and actions propagate to the caller; they are not converted to failure results.</remarks>
public static class RailwayExtensions
{
    /// <summary>Applies a result-producing function to a success, or passes a failure through unchanged.</summary>
    /// <typeparam name="TSuccessIn">The input success type.</typeparam>
    /// <typeparam name="TSuccessOut">The output success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="twoTrackInput">The result to bind.</param>
    /// <param name="switchFunction">The function invoked for a success.</param>
    /// <returns>The function's result on success, or the original failure.</returns>
    /// <example><code>Result&lt;int, string&gt; result = 5;
    /// var next = result.Bind(x =&gt; (Result&lt;int, string&gt;)(x * 2));</code></example>
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

    /// <summary>Composes a result-producing function with another operation that runs only on success.</summary>
    /// <typeparam name="TInput">The input type of the composed function.</typeparam>
    /// <typeparam name="TSuccessIn">The first function's success type.</typeparam>
    /// <typeparam name="TSuccessOut">The second function's success type.</typeparam>
    /// <typeparam name="TFailure">The shared failure type.</typeparam>
    /// <param name="twoTrackInputFunction">The first result-producing function.</param>
    /// <param name="switchFunction">The operation invoked on success.</param>
    /// <returns>A function that runs the operations in sequence, passing failures through.</returns>
    /// <example><code>Func&lt;int, Result&lt;int, string&gt;&gt; first = x =&gt; x;
    /// var pipeline = first.Bind(x =&gt; (Result&lt;int, string&gt;)(x * 2));
    /// var result = pipeline(5);</code></example>
    public static Func<TInput, Result<TSuccessOut, TFailure>> Bind<TInput, TSuccessIn, TSuccessOut, TFailure>(this Func<TInput, Result<TSuccessIn, TFailure>> twoTrackInputFunction,
        Func<TSuccessIn, Result<TSuccessOut, TFailure>> switchFunction)
    {
        return input => twoTrackInputFunction(input).Bind(switchFunction);
    }

    /// <summary>Applies a result-producing function to a failure, or passes a success through unchanged.</summary>
    /// <remarks>Because the function returns a result, a failure can be recovered to a success.</remarks>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailureIn">The input failure type.</typeparam>
    /// <typeparam name="TFailureOut">The output failure type.</typeparam>
    /// <param name="twoTrackInput">The result to bind.</param>
    /// <param name="switchFunction">The function invoked for a failure.</param>
    /// <returns>The function's result on failure, or the original success.</returns>
    /// <example><code>Result&lt;int, string&gt; result = "invalid";
    /// var recovered = result.BindFailure(error =&gt; (Result&lt;int, string&gt;)(-1));</code></example>
    public static Result<TSuccess, TFailureOut> BindFailure<TSuccess, TFailureIn, TFailureOut>(this Result<TSuccess, TFailureIn> twoTrackInput,
        Func<TFailureIn, Result<TSuccess, TFailureOut>> switchFunction)
    {
        return twoTrackInput switch
        {
            TSuccess success => success,
            TFailureIn failure => switchFunction(failure),
            _ => throw new ArgumentOutOfRangeException(nameof(twoTrackInput), twoTrackInput, null)
        };
    }

    /// <summary>Composes a result-producing function with another operation that runs only on failure.</summary>
    /// <typeparam name="TInput">The input type of the composed function.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailureIn">The first function's failure type.</typeparam>
    /// <typeparam name="TFailureOut">The second function's failure type.</typeparam>
    /// <param name="twoTrackInputFunction">The first result-producing function.</param>
    /// <param name="switchFunction">The operation invoked on failure.</param>
    /// <returns>A function that runs the operations in sequence, passing successes through.</returns>
    /// <example><code>Func&lt;int, Result&lt;int, string&gt;&gt; first = x =&gt; "invalid";
    /// var pipeline = first.BindFailure(error =&gt; (Result&lt;int, string&gt;)(-1));
    /// var result = pipeline(5);</code></example>
    public static Func<TInput, Result<TSuccess, TFailureOut>> BindFailure<TInput, TSuccess, TFailureIn, TFailureOut>(this Func<TInput, Result<TSuccess, TFailureIn>> twoTrackInputFunction,
        Func<TFailureIn, Result<TSuccess, TFailureOut>> switchFunction)
    {
        return input => twoTrackInputFunction(input).BindFailure(switchFunction);
    }

    /// <summary>Wraps a value on the success track.</summary>
    /// <typeparam name="TSuccess">The value's type.</typeparam>
    /// <typeparam name="TFailure">The failure type of the result.</typeparam>
    /// <param name="input">The value to wrap.</param>
    /// <returns>A success result containing the input.</returns>
    /// <example><code>Result&lt;int, string&gt; result = 5.Lift&lt;int, string&gt;();</code></example>
    public static Result<TSuccess, TFailure> Lift<TSuccess, TFailure>(this TSuccess input)
    {
        return input.Switch<TSuccess, TSuccess, TFailure>(x => x);
    }

    /// <summary>Converts a single-track function into one that wraps each output as a success.</summary>
    /// <typeparam name="TInput">The function's input type.</typeparam>
    /// <typeparam name="TSuccess">The function's output and result success type.</typeparam>
    /// <typeparam name="TFailure">The failure type of the result.</typeparam>
    /// <param name="func">The single-track function to wrap.</param>
    /// <returns>A function that returns a success result for each output.</returns>
    /// <example><code>Func&lt;int, int&gt; doubleValue = x =&gt; x * 2;
    /// var lifted = doubleValue.Lift&lt;int, int, string&gt;();
    /// var result = lifted(5);</code></example>
    public static Func<TInput, Result<TSuccess, TFailure>> Lift<TInput, TSuccess, TFailure>(this Func<TInput, TSuccess> func)
    {
        return input => func(input).Lift<TSuccess, TFailure>();
    }

    /// <summary>Transforms a success value with a single-track function, passing failures through unchanged.</summary>
    /// <typeparam name="TSuccessIn">The input success type.</typeparam>
    /// <typeparam name="TSuccessOut">The transformed success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="twoTrackInput">The result to transform.</param>
    /// <param name="oneTrackFunction">The function invoked for a success.</param>
    /// <returns>A result containing the transformed success or the original failure.</returns>
    /// <example><code>Result&lt;int, string&gt; result = 5;
    /// var doubled = result.Map(x =&gt; x * 2);</code></example>
    public static Result<TSuccessOut, TFailure> Map<TSuccessIn, TSuccessOut, TFailure>(this Result<TSuccessIn, TFailure> twoTrackInput, Func<TSuccessIn, TSuccessOut> oneTrackFunction)
    {
        return twoTrackInput switch
        {
            TSuccessIn success => oneTrackFunction(success),
            TFailure failure => failure,
            _ => throw new ArgumentOutOfRangeException(nameof(twoTrackInput), twoTrackInput, null)
        };
    }

    /// <summary>Composes a result-producing function with a transformation of its success value.</summary>
    /// <typeparam name="TInput">The input type of the composed function.</typeparam>
    /// <typeparam name="TSuccessIn">The original success type.</typeparam>
    /// <typeparam name="TSuccessOut">The transformed success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="twoTrackInputFunction">The result-producing function.</param>
    /// <param name="oneTrackFunction">The transformation invoked on success.</param>
    /// <returns>A function that transforms successes and passes failures through.</returns>
    /// <example><code>Func&lt;int, Result&lt;int, string&gt;&gt; first = x =&gt; x;
    /// var pipeline = first.Map(x =&gt; x * 2);
    /// var result = pipeline(5);</code></example>
    public static Func<TInput, Result<TSuccessOut, TFailure>> Map<TInput, TSuccessIn, TSuccessOut, TFailure>(this Func<TInput, Result<TSuccessIn, TFailure>> twoTrackInputFunction,
        Func<TSuccessIn, TSuccessOut> oneTrackFunction)
    {
        return input => twoTrackInputFunction(input).Map(oneTrackFunction);
    }

    /// <summary>Transforms a failure value with a single-track function, passing successes through unchanged.</summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailureIn">The input failure type.</typeparam>
    /// <typeparam name="TFailureOut">The transformed failure type.</typeparam>
    /// <param name="twoTrackInput">The result to transform.</param>
    /// <param name="oneTrackFunction">The function invoked for a failure.</param>
    /// <returns>A result containing the original success or the transformed failure.</returns>
    /// <example><code>Result&lt;int, string&gt; result = "invalid";
    /// var wrapped = result.MapFailure(error =&gt; error.Length);</code></example>
    public static Result<TSuccess, TFailureOut> MapFailure<TSuccess, TFailureIn, TFailureOut>(this Result<TSuccess, TFailureIn> twoTrackInput, Func<TFailureIn, TFailureOut> oneTrackFunction)
    {
        return twoTrackInput switch
        {
            TSuccess success => success,
            TFailureIn failure => oneTrackFunction(failure),
            _ => throw new ArgumentOutOfRangeException(nameof(twoTrackInput), twoTrackInput, null)
        };
    }

    /// <summary>Composes a result-producing function with a transformation of its failure value.</summary>
    /// <typeparam name="TInput">The input type of the composed function.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailureIn">The original failure type.</typeparam>
    /// <typeparam name="TFailureOut">The transformed failure type.</typeparam>
    /// <param name="twoTrackInputFunction">The result-producing function.</param>
    /// <param name="oneTrackFunction">The transformation invoked on failure.</param>
    /// <returns>A function that transforms failures and passes successes through.</returns>
    /// <example><code>Func&lt;int, Result&lt;int, string&gt;&gt; first = x =&gt; "invalid";
    /// var pipeline = first.MapFailure(error =&gt; error.Length);
    /// var result = pipeline(5);</code></example>
    public static Func<TInput, Result<TSuccess, TFailureOut>> MapFailure<TInput, TSuccess, TFailureIn, TFailureOut>(this Func<TInput, Result<TSuccess, TFailureIn>> twoTrackInputFunction,
        Func<TFailureIn, TFailureOut> oneTrackFunction)
    {
        return input => twoTrackInputFunction(input).MapFailure(oneTrackFunction);
    }

    /// <summary>Applies a single-track function to a value and wraps its output as a success.</summary>
    /// <typeparam name="TInput">The input type.</typeparam>
    /// <typeparam name="TSuccess">The function's output and result success type.</typeparam>
    /// <typeparam name="TFailure">The failure type of the result.</typeparam>
    /// <param name="input">The value supplied to the function.</param>
    /// <param name="oneTrackFunction">The function to apply.</param>
    /// <returns>A success result containing the function's output.</returns>
    /// <example><code>var result = 5.Switch&lt;int, int, string&gt;(x =&gt; x * 2);</code></example>
    public static Result<TSuccess, TFailure> Switch<TInput, TSuccess, TFailure>(this TInput input, Func<TInput, TSuccess> oneTrackFunction)
    {
        return oneTrackFunction(input);
    }

    /// <summary>Converts a single-track function into one that wraps its output as a success.</summary>
    /// <typeparam name="TInput">The input type.</typeparam>
    /// <typeparam name="TSuccess">The function's output and result success type.</typeparam>
    /// <typeparam name="TFailure">The failure type of the result.</typeparam>
    /// <param name="func">The single-track function to wrap.</param>
    /// <returns>A function that returns a success result for each output.</returns>
    /// <example><code>Func&lt;int, int&gt; doubleValue = x =&gt; x * 2;
    /// var switched = doubleValue.Switch&lt;int, int, string&gt;();
    /// var result = switched(5);</code></example>
    public static Func<TInput, Result<TSuccess, TFailure>> Switch<TInput, TSuccess, TFailure>(this Func<TInput, TSuccess> func)
    {
        return input => input.Switch<TInput, TSuccess, TFailure>(func);
    }

    /// <summary>Performs an action on a success and returns the original result.</summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="input">The result to inspect.</param>
    /// <param name="teeAction">The action invoked on success.</param>
    /// <returns>The original result, unchanged.</returns>
    /// <example><code>Result&lt;int, string&gt; result = 5;
    /// var unchanged = result.Tee(x =&gt; Console.WriteLine(x));</code></example>
    public static Result<TSuccess, TFailure> Tee<TSuccess, TFailure>(this Result<TSuccess, TFailure> input, Action<TSuccess> teeAction)
    {
        switch (input)
        {
            case TSuccess success:
                teeAction(success);
                break;
            case TFailure:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(input), input, null);
        }

        return input;
    }

    /// <summary>Composes a result-producing function with a success-side action.</summary>
    /// <typeparam name="TInput">The input type of the composed function.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="result">The result-producing function.</param>
    /// <param name="teeAction">The action invoked on success.</param>
    /// <returns>A function that performs the action and returns the original result.</returns>
    /// <example><code>Func&lt;int, Result&lt;int, string&gt;&gt; first = x =&gt; x;
    /// var pipeline = first.Tee(x =&gt; Console.WriteLine(x));
    /// var result = pipeline(5);</code></example>
    public static Func<TInput, Result<TSuccess, TFailure>> Tee<TInput, TSuccess, TFailure>(this Func<TInput, Result<TSuccess, TFailure>> result,
        Action<TSuccess> teeAction)
    {
        return input => result(input).Tee(teeAction);
    }

    /// <summary>Performs an action on a failure and returns the original result.</summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="input">The result to inspect.</param>
    /// <param name="teeAction">The action invoked on failure.</param>
    /// <returns>The original result, unchanged.</returns>
    /// <example><code>Result&lt;int, string&gt; result = "invalid";
    /// var unchanged = result.TeeFailure(error =&gt; Console.WriteLine(error));</code></example>
    public static Result<TSuccess, TFailure> TeeFailure<TSuccess, TFailure>(this Result<TSuccess, TFailure> input, Action<TFailure> teeAction)
    {
        switch (input)
        {
            case TSuccess:
                break;
            case TFailure failure:
                teeAction(failure);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(input), input, null);
        }

        return input;
    }

    /// <summary>Composes a result-producing function with a failure-side action.</summary>
    /// <typeparam name="TInput">The input type of the composed function.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="result">The result-producing function.</param>
    /// <param name="teeAction">The action invoked on failure.</param>
    /// <returns>A function that performs the action and returns the original result.</returns>
    /// <example><code>Func&lt;int, Result&lt;int, string&gt;&gt; first = x =&gt; x;
    /// var pipeline = first.TeeFailure(error =&gt; Console.WriteLine(error));
    /// var result = pipeline(5);</code></example>
    public static Func<TInput, Result<TSuccess, TFailure>> TeeFailure<TInput, TSuccess, TFailure>(this Func<TInput, Result<TSuccess, TFailure>> result,
        Action<TFailure> teeAction)
    {
        return input => result(input).TeeFailure(teeAction);
    }

    /// <summary>Invokes the action for the result's track; a failure is ignored if no failure action is supplied.</summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="twoTrackInput">The result to handle.</param>
    /// <param name="onSuccess">The action invoked on success.</param>
    /// <param name="onFailure">The optional action invoked on failure.</param>
    /// <example><code>Result&lt;int, string&gt; result = 5;
    /// result.Handle(x =&gt; Console.WriteLine(x), error =&gt; Console.WriteLine(error));</code></example>
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
            default:
                throw new ArgumentOutOfRangeException(nameof(twoTrackInput), twoTrackInput, null);
        }
    }

    /// <summary>Creates an action that handles the result of a function using the appropriate track action.</summary>
    /// <typeparam name="TInput">The input type of the returned action.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="twoTrackInputFunction">The result-producing function.</param>
    /// <param name="onSuccess">The action invoked on success.</param>
    /// <param name="onFailure">The optional action invoked on failure; if omitted, failures are ignored.</param>
    /// <returns>An action that evaluates and handles the result.</returns>
    /// <example><code>Func&lt;int, Result&lt;int, string&gt;&gt; first = x =&gt; x;
    /// var handle = first.Handle(x =&gt; Console.WriteLine(x));
    /// handle(5);</code></example>
    public static Action<TInput> Handle<TInput, TSuccess, TFailure>(this Func<TInput, Result<TSuccess, TFailure>> twoTrackInputFunction, Action<TSuccess> onSuccess, Action<TFailure>? onFailure = null)
    {
        return input => twoTrackInputFunction(input).Handle(onSuccess, onFailure);
    }

    /// <summary>Maps either track of a result to a common output type.</summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <typeparam name="TOutput">The common output type.</typeparam>
    /// <param name="twoTrackInput">The result to merge.</param>
    /// <param name="successFunc">The function invoked on success.</param>
    /// <param name="failureFunc">The function invoked on failure.</param>
    /// <returns>The output of the function for the result's track.</returns>
    /// <example><code>Result&lt;int, string&gt; result = 5;
    /// string message = result.Merge(x =&gt; x.ToString(), error =&gt; error);</code></example>
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

    /// <summary>Composes a result-producing function with functions that map either track to a common output.</summary>
    /// <typeparam name="TInput">The input type of the composed function.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <typeparam name="TOutput">The common output type.</typeparam>
    /// <param name="twoTrackInputFunction">The result-producing function.</param>
    /// <param name="successFunc">The function invoked on success.</param>
    /// <param name="failureFunc">The function invoked on failure.</param>
    /// <returns>A function that evaluates the input and merges its result into one output.</returns>
    /// <example><code>Func&lt;int, Result&lt;int, string&gt;&gt; first = x =&gt; x;
    /// var describe = first.Merge(x =&gt; x.ToString(), error =&gt; error);
    /// string message = describe(5);</code></example>
    public static Func<TInput, TOutput> Merge<TInput, TSuccess, TFailure, TOutput>(this Func<TInput, Result<TSuccess, TFailure>> twoTrackInputFunction,
        Func<TSuccess, TOutput> successFunc, Func<TFailure, TOutput> failureFunc)
    {
        return input => twoTrackInputFunction(input).Merge(successFunc, failureFunc);
    }
}