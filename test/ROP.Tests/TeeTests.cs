using System;

namespace ROP.Tests;

public class TeeTests
{
    [Fact]
    public void Tee_should_run_action_on_success_and_return_original_result()
    {
        var observed = 0;
        Result<int, string> result = 5;

        var returned = result.Tee(value => observed = value);

        observed.Should().Be(5);
        returned.Handle(
            success => success.Should().Be(5),
            _ => throw new Exception("Expected success"));
    }

    [Fact]
    public void Tee_should_not_run_action_on_failure_and_return_original_result()
    {
        var invoked = false;
        Result<int, string> result = "boom";

        var returned = result.Tee(_ => invoked = true);

        invoked.Should().BeFalse();
        returned.Handle(
            _ => throw new Exception("Expected failure"),
            failure => failure.Should().Be("boom"));
    }

    [Fact]
    public void Functional_tee_should_run_action_on_success()
    {
        var observed = 0;
        Func<int, Result<int, string>> pipeline = x => x;

        var result = pipeline.Tee(value => observed = value)(5);

        observed.Should().Be(5);
        result.Handle(
            success => success.Should().Be(5),
            _ => throw new Exception("Expected success"));
    }
}
