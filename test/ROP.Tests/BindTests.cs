using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ROP.Tests
{
    public class BindTests
    {
        [Theory]
        [InlineData(7, null)]
        [InlineData(12, "Number too large")]
        [InlineData(4, "Number too small")]
        public void Bind_should_return_expected_result(int number, string errorMessage)
        {
            Inner.NumberTooLargeProperty
                .Bind(Inner.NumberTooSmallProperty)
                .Handle(onSuccess => onSuccess.Should().Be(number), onFailure => onFailure.Should().Be(errorMessage))
                .Invoke(number);
        }

        [Theory]
        [InlineData(7, null)]
        [InlineData(12, "Number too large")]
        [InlineData(4, "Number too small")]
        public void Functional_bind_should_return_expected_result(int number, string errorMessage)
        {
            Inner.NumberTooLargeMethod(number)
                .Bind(Inner.NumberTooSmallMethod)
                .Handle(onSuccess => onSuccess.Should().Be(number), onFailure => onFailure.Should().Be(errorMessage));
        }

        [Theory]
        [InlineData(7, 17, true, null)]
        [InlineData(9, 23, true, null)]
        [InlineData(12, 0, false, "Number too large")]
        [InlineData(4, 0, false, "Number too small")]
        public async Task Asynchronous_bind_should_return_expected_result(int number, int expectedResult, bool successExpected, string errorMessage)
        {
            var result = await Inner.NumberTooLargeMethod(number)
                .Bind(Inner.NumberTooSmallMethod)
                .BindAsync(Inner.SlowCalculationMethod);

            if (successExpected)
            {
                ((int)result.AsSuccess).Should().Be(expectedResult);
            }
            else
            {
                ((string)result.AsFailure).Should().Be(errorMessage);
            }
        }

        [Theory]
        [InlineData(7, 17, true, null)]
        [InlineData(9, 23, true, null)]
        [InlineData(12, 0, false, "Number too large")]
        [InlineData(4, 0, false, "Number too small")]
        public async Task Functional_asynchronous_bind_should_return_expected_result(int number, int expectedResult, bool successExpected, string errorMessage)
        {
            var result = await Inner.NumberTooLargeProperty
                .Bind(Inner.NumberTooSmallProperty)
                .BindAsync(Inner.SlowCalculationProperty)
                .Invoke(number);

            if (successExpected)
            {
                ((int) result.AsSuccess).Should().Be(expectedResult);
            }
            else
            {
                ((string) result.AsFailure).Should().Be(errorMessage);
            }
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        private class Inner
        {
            public static Func<int, Result<int, string>> NumberTooLargeProperty { get; }
                = NumberTooLargeMethod;

            public static Func<int, Result<int, string>> NumberTooSmallProperty { get; }
                = NumberTooSmallMethod;

            public static Func<int, Task<Result<long, string>>> SlowCalculationProperty { get; }
                = SlowCalculationMethod;

            public static Result<int, string> NumberTooLargeMethod(int i)
            {
                return i > 10
                    ? Result<int, string>.NewFailure("Number too large")
                    : Result<int, string>.NewSuccess(i);
            }

            public static Result<int, string> NumberTooSmallMethod(int i)
            {
                return i < 5
                    ? Result<int, string>.NewFailure("Number too small")
                    : Result<int, string>.NewSuccess(i);
            }

            public static Task<Result<long, string>> SlowCalculationMethod(int i)
            {
                return Task.Run(() => FindPrimeNumber(i).Lift<long, string>());
            }

            private static long FindPrimeNumber(int n)
            {
                var count = 0;
                long a = 2;
                while (count < n)
                {
                    long b = 2;
                    var prime = 1; // to check if found a prime
                    while (b * b <= a)
                    {
                        if (a % b == 0)
                        {
                            prime = 0;
                            break;
                        }

                        b++;
                    }

                    if (prime > 0)
                    {
                        count++;
                    }

                    a++;
                }

                return --a;
            }
        }
    }
}
