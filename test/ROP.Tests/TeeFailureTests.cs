using System;

namespace EdCourtenay.ROP.Tests;

public class TeeFailureTests
{
    [Fact]
    public void TeeFailure_should_run_action_on_failure_and_return_original_result()
    {
        var observed = string.Empty;
        Result<int, string> result = "boom";

        var returned = result.TeeFailure(error => observed = error);

        observed.Should().Be("boom");
        returned.Handle(
            _ => throw new Exception("Expected failure"),
            failure => failure.Should().Be("boom"));
    }

    [Fact]
    public void TeeFailure_should_not_run_action_on_success_and_return_original_result()
    {
        var invoked = false;
        Result<int, string> result = 5;

        var returned = result.TeeFailure(_ => invoked = true);

        invoked.Should().BeFalse();
        returned.Handle(
            success => success.Should().Be(5),
            _ => throw new Exception("Expected success"));
    }

    [Fact]
    public void Functional_tee_failure_should_run_action_on_failure()
    {
        var observed = string.Empty;
        Func<int, Result<int, string>> pipeline = _ => "boom";

        var result = pipeline.TeeFailure(error => observed = error)(5);

        observed.Should().Be("boom");
        result.Handle(
            _ => throw new Exception("Expected failure"),
            failure => failure.Should().Be("boom"));
    }
}
