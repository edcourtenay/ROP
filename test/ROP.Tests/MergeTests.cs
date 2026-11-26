using System;

namespace EdCourtenay.ROP.Tests;

public class MergeTests
{
    [Fact]
    public void Merge_should_map_success()
    {
        Result<int, string> result = 5;

        var merged = result.Merge(success => $"ok:{success}", failure => $"error:{failure}");

        merged.Should().Be("ok:5");
    }

    [Fact]
    public void Merge_should_map_failure()
    {
        Result<int, string> result = "boom";

        var merged = result.Merge(success => $"ok:{success}", failure => $"error:{failure}");

        merged.Should().Be("error:boom");
    }

    [Fact]
    public void Functional_merge_should_map_either_track()
    {
        Func<int, Result<int, string>> success = x => x;
        Func<int, Result<int, string>> failure = _ => "boom";

        success.Merge(x => $"ok:{x}", error => $"error:{error}")(5).Should().Be("ok:5");
        failure.Merge(x => $"ok:{x}", error => $"error:{error}")(5).Should().Be("error:boom");
    }
}
