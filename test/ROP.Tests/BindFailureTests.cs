using System;

namespace EdCourtenay.ROP.Tests;

public class BindFailureTests
{
    [Fact]
    public void BindFailure_should_recover_failure_to_success()
    {
        Result<int, string> result = "boom";

        var recovered = result.BindFailure(_ => (Result<int, string>)(-1));

        recovered.Handle(
            success => success.Should().Be(-1),
            _ => throw new Exception("Expected success"));
    }

    [Fact]
    public void BindFailure_should_pass_success_through_unchanged()
    {
        Result<int, string> result = 5;

        var recovered = result.BindFailure(_ => (Result<int, string>)(-1));

        recovered.Handle(
            success => success.Should().Be(5),
            _ => throw new Exception("Expected success"));
    }

    [Fact]
    public void BindFailure_should_replace_failure_with_new_failure()
    {
        Result<int, string> result = "boom";

        var rebound = result.BindFailure(error => (Result<int, Failure>)(new Failure(error)));

        rebound.Handle(
            _ => throw new Exception("Expected failure"),
            failure => failure.Should().Be(new Failure("boom")));
    }

    [Fact]
    public void Functional_bind_failure_should_recover_failure()
    {
        Func<int, Result<int, string>> pipeline = _ => "boom";

        var result = pipeline.BindFailure(_ => (Result<int, string>)(-1))(5);

        result.Handle(
            success => success.Should().Be(-1),
            _ => throw new Exception("Expected success"));
    }

    private sealed record Failure(string Message);
}
