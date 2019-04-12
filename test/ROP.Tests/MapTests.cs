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
            Inner.CheckMethod(value)
                .Map(Inner.MapMethod)
                .Handle(onSuccess => {
                    expectedSuccess.Should().BeTrue();
                    onSuccess.Should().Be($"The number is: {value}");
                }, onFailure => {
                    expectedSuccess.Should().BeFalse();
                    onFailure.Should().Be("Number too large");
                });
        }

        [Theory]
        [InlineData(10, true)]
        [InlineData(30, false)]
        public void Functional_map_should_return_expected_success_result(int value, bool expectedSuccess)
        {
            Inner.CheckProperty
                .Map(Inner.MapProperty)
                .Handle(onSuccess => {
                    expectedSuccess.Should().BeTrue();
                    onSuccess.Should().Be($"The number is: {value}");
                }, onFailure => {
                    expectedSuccess.Should().BeFalse();
                    onFailure.Should().Be("Number too large");
                })
                .Invoke(value);
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
