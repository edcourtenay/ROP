using System;
using System.Threading.Tasks;

namespace ROP.Tests;

public class ToAsyncTests
{
    [Fact]
    public async Task ToAsync_should_wrap_result_in_completed_task()
    {
        Result<int, string> result = 5;

        var task = result.ToAsync();

        task.IsCompletedSuccessfully.Should().BeTrue();
        (await task).Handle(
            success => success.Should().Be(5),
            _ => throw new Exception("Expected success"));
    }

    [Fact]
    public async Task Functional_toAsync_should_wrap_result_in_completed_task()
    {
        Func<int, Result<int, string>> pipeline = x => x;

        var task = pipeline.ToAsync()(5);

        task.IsCompletedSuccessfully.Should().BeTrue();
        (await task).Handle(
            success => success.Should().Be(5),
            _ => throw new Exception("Expected success"));
    }

    [Fact]
    public async Task BindAsync_should_compose_two_async_functions()
    {
        Func<int, Task<Result<int, string>>> first = x => Task.FromResult<Result<int, string>>(x);
        Func<int, Task<Result<int, string>>> second = x => Task.FromResult<Result<int, string>>(x + 1);

        var result = await first.BindAsync(second)(1);

        result.Handle(
            success => success.Should().Be(2),
            _ => throw new Exception("Expected success"));
    }

    [Fact]
    public async Task BindAsync_should_propagate_failure_without_invoking_the_next_function()
    {
        Func<int, Task<Result<int, string>>> first = _ => Task.FromResult<Result<int, string>>("boom");
        var invoked = false;
        Func<int, Task<Result<int, string>>> second = x =>
        {
            invoked = true;
            return Task.FromResult<Result<int, string>>(x + 1);
        };

        var result = await first.BindAsync(second)(1);

        invoked.Should().BeFalse();
        result.Handle(
            _ => throw new Exception("Expected failure"),
            failure => failure.Should().Be("boom"));
    }
}
