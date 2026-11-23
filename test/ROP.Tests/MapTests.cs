using System;

namespace ROP.Tests;

public class MapTests
{
    [Theory]
    [InlineData(10, true)]
    [InlineData(30, false)]
    public void Map_should_return_expected_success_result(int value, bool expectedSuccess)
    {
        Inner.CheckMethod(value)
            .Map(Inner.MapMethod)
            .Handle(onSuccess =>
            {
                expectedSuccess.Should().BeTrue();
                onSuccess.Should().Be($"Number is {value}");
            }, onFailure =>
            {
                expectedSuccess.Should().BeFalse();
                onFailure.Should().Be(new Error.NumberTooLarge());
            });
    }

    [Theory]
    [InlineData(10, true)]
    [InlineData(30, false)]
    public void Functional_map_should_return_expected_success_result(int value, bool expectedSuccess)
    {
        Inner.CheckProperty
            .Map(Inner.MapProperty)
            .Handle(onSuccess =>
            {
                expectedSuccess.Should().BeTrue();
                onSuccess.Should().Be($"Number is {value}");
            }, onFailure =>
            {
                expectedSuccess.Should().BeFalse();
                onFailure.Should().Be(new Error.NumberTooLarge());
            })
            .Invoke(value);
    }

    private static class Inner
    {
        public static Func<int, string> MapProperty { get; }
            = MapMethod;

        public static string MapMethod(int i)
            => $"Number is {i}";

        public static Func<int, Result<int, Error>> CheckProperty { get; }
            = CheckMethod;

        public static Result<int, Error> CheckMethod(int i)
            => i < 20
                ? i
                : new Error.NumberTooLarge();
    }

    public closed record Error
    {
        public record NumberTooLarge : Error;
        public record NumberTooSmall : Error;
    }
}
