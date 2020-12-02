using FluentAssertions;
using Xunit;

namespace ROP.Tests
{
    public class ResultTests
    {
        [Fact]
        public void NewSuccess_should_return_Success_with_expected_values()
        {
            var result = new Result<int, string>.Success(10);

            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();

            result.AsSuccess.Item.Should().Be(10);
            result.AsFailure.Should().BeNull();

            // Check implicit conversion
            10.Should().Be(result.AsSuccess);
            ((int)result.AsSuccess).Should().Be(10);
        }

        [Fact]
        public void NewFailure_should_return_Failure_with_expected_values()
        {
            var result = new Result<int, string>.Failure("error state");

            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();

            result.AsSuccess.Should().BeNull("a failure state cannot also have a success state");
            result.AsFailure.Item.Should().Be("error state");

            // Check implicit conversion
            "error state".Should().Be(result.AsFailure);
            ((string) result.AsFailure).Should().Be("error state");
        }
    }
}
