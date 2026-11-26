using System;

namespace EdCourtenay.ROP.Tests;

public class SwitchTests
{
    [Fact]
    public void Switch_should_wrap_function_output_as_success()
    {
        var result = 5.Switch<int, int, string>(x => x * 2);

        result.Handle(
            success => success.Should().Be(10),
            _ => throw new Exception("Expected success"));
    }

    [Fact]
    public void Functional_switch_should_wrap_output_as_success()
    {
        Func<int, int> doubleValue = x => x * 2;

        var result = doubleValue.Switch<int, int, string>()(5);

        result.Handle(
            success => success.Should().Be(10),
            _ => throw new Exception("Expected success"));
    }
}
