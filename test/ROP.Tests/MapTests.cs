using System;
using FluentAssertions;
using Xunit;

namespace ROP.Tests
{
    public class MapTests
    {
        [Theory]
        [InlineData(10, true)]
        [InlineData(30, false)]
        public void Map_should_return_expected_success_result(int value, bool expectedSuccess)
        {
            var result = Inner.CheckMethod(value)
                .Map(Inner.MapMethod);

            if (expectedSuccess)
            {
                result.IsSuccess.Should().BeTrue();
                result.AsSuccess.Item.Should().Be($"The number is: {value}");
            }
            else
            {
                result.IsFailure.Should().BeTrue();
                result.AsFailure.Item.Should().Be("Number too large");
            }
        }

        [Theory]
        [InlineData(10, true)]
        [InlineData(30, false)]
        public void Functional_map_should_return_expected_success_result(int value, bool expectedSuccess)
        {
            var result = Inner.CheckProperty
                .Map(Inner.MapProperty)
                .Invoke(value);

            if (expectedSuccess)
            {
                result.IsSuccess.Should().BeTrue();
                result.AsSuccess.Item.Should().Be($"The number is: {value}");
            }
            else
            {
                result.IsFailure.Should().BeTrue();
                result.AsFailure.Item.Should().Be("Number too large");
            }
        }

        class Inner
        {
            public static Func<int, string> MapProperty
                => MapMethod;

            public static string MapMethod(int i)
                => $"The number is: {i}";

            public static Func<int, Result<int, string>> CheckProperty { get; }
                = CheckMethod;

            public static Result<int, string> CheckMethod(int i)
                => i < 20
                    ? Result<int, string>.NewSuccess(i)
                    : Result<int, string>.NewFailure("Number too large");
        }
    }
}
