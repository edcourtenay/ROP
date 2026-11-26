using System;

namespace ROP.Tests;

public class HandleTests
{
    [Fact]
    public void Handle_should_invoke_success_action_on_success()
    {
        var observed = 0;
        Result<int, string> result = 5;

        result.Handle(success => observed = success, _ => throw new Exception("Expected success"));

        observed.Should().Be(5);
    }

    [Fact]
    public void Handle_should_invoke_failure_action_on_failure()
    {
        var observed = string.Empty;
        Result<int, string> result = "boom";

        result.Handle(_ => throw new Exception("Expected failure"), failure => observed = failure);

        observed.Should().Be("boom");
    }

    [Fact]
    public void Handle_should_not_throw_when_failure_action_is_omitted()
    {
        Result<int, string> result = "boom";

        result.Handle(_ => throw new Exception("Expected failure"));
    }

    [Fact]
    public void Functional_handle_should_invoke_appropriate_action()
    {
        var observed = string.Empty;
        Func<int, Result<int, string>> pipeline = _ => "boom";

        pipeline.Handle(_ => throw new Exception("Expected failure"), failure => observed = failure)(5);

        observed.Should().Be("boom");
    }
}
