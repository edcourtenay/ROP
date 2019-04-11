using FluentAssertions;
using Xunit;

namespace ROP.Tests
{
    public class LiftTests
    {
        [Fact]
        public void Lift_should_create_two_track_result_from_primitive()
        {
            var expected = 10;

            var result = expected.Lift<int, string>();

            result.IsSuccess.Should().BeTrue();
            result.AsSuccess.Item.Should().Be(expected);
        }
    }
}
