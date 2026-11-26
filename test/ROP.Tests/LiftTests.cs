using System;

namespace ROP.Tests;

public class LiftTests
{
    [Fact]
    public void Lift_should_create_two_track_result_from_primitive()
    {
        const int expected = 10;

        var result = expected.Lift<int, string>();

        result.Should().BeEquivalentTo(new { Value = expected });
    }

    [Fact]
    public void Functional_lift_should_wrap_output_as_success()
    {
        Func<int, int> doubleValue = x => x * 2;

        var result = doubleValue.Lift<int, int, string>()(5);

        result.Should().BeEquivalentTo(new { Value = 10 });
    }
}