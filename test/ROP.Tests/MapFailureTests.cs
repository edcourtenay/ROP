using System;

namespace EdCourtenay.ROP.Tests;

public class MapFailureTests
{
    [Fact]
    public void MapFailure_should_transform_failure()
    {
        Result<int, string> result = "boom";

        var mapped = result.MapFailure(error => new Failure(error));

        mapped.Handle(
            _ => throw new Exception("Expected failure"),
            failure => failure.Should().Be(new Failure("boom")));
    }

    [Fact]
    public void MapFailure_should_pass_success_through_unchanged()
    {
        Result<int, string> result = 5;

        var mapped = result.MapFailure(error => new Failure(error));

        mapped.Handle(
            success => success.Should().Be(5),
            _ => throw new Exception("Expected success"));
    }

    [Fact]
    public void Functional_map_failure_should_transform_failure()
    {
        Func<int, Result<int, string>> pipeline = _ => "boom";

        var result = pipeline.MapFailure(error => new Failure(error))(5);

        result.Handle(
            _ => throw new Exception("Expected failure"),
            failure => failure.Should().Be(new Failure("boom")));
    }

    private sealed record Failure(string Message);
}
