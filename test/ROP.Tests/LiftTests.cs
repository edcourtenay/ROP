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
}