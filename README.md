# ROP

ROP is a C# library for composing operations that can succeed or fail. It is inspired by Scott Wlaschin's [Railway Oriented Programming talk](https://fsharpforfunandprofit.com/rop/) and [blog post](https://fsharpforfunandprofit.com/posts/recipe-part2/).

A `Result<TSuccess, TFailure>` carries a value on either the success or failure track. Operations composed with `Bind` run on success; a failure bypasses subsequent operations and is passed along unchanged. This makes the happy path readable without discarding expected errors.

## Getting started

The solution targets `net11.0`. Install a .NET SDK compatible with the version and roll-forward settings in `global.json`, then from the repository root run:

```shell
dotnet build ROP.sln
dotnet test ROP.sln
```

Add the [ROP NuGet package](https://www.nuget.org/packages/ROP) to your project, or reference `src/ROP/ROP.csproj` directly, and import the `ROP` namespace:

```shell
dotnet add package ROP
```

## Usage

Return either a success or a failure value from an operation, then chain operations with `Bind`:

```csharp
using System;
using ROP;

Result<int, string> CheckMinimum(int value) =>
    value < 5 ? "Number too small" : value;

Result<int, string> CheckMaximum(int value) =>
    value > 10 ? "Number too large" : value;

var result = CheckMinimum(7)
    .Bind(CheckMaximum)
    .Map(value => value * 2);

result.Handle(
    success => Console.WriteLine($"Result: {success}"),
    failure => Console.WriteLine($"Error: {failure}"));
```

For `7`, the result is `14`. For a value below `5`, `CheckMaximum` and `Map` are skipped and the error is handled instead.

### Operations

- `Bind` chains a success value to an operation returning another `Result` with the same failure type.
- `Map` transforms a success value with a function that does not return a `Result`.
- `Lift` wraps a value as a success; `Switch` applies a function and wraps its output as a success.
- `Tee` and `TeeFailure` perform side effects on the respective track and return the original result.
- `Handle` invokes an action for the applicable track; its failure action is optional. `Merge` maps both tracks to a common output type.
- `BindAsync` chains asynchronous operations returning `Task<Result<...>>`; `ToAsync` wraps a synchronous result or result-producing function in a task.

Most operations also have overloads for composing result-producing functions before supplying their input. These helpers do not convert thrown exceptions into failure values.

## Solution projects

- `src/ROP` — the result type and railway composition extensions.
- `src/ROP.Example` — an ASP.NET Core transfer API demonstrating validation, side effects, chaining and merging results into HTTP responses. Run it with `dotnet run --project src/ROP.Example`; see `requests.http` for sample requests.
- `test/ROP.Tests` — tests for the library and example integration.

## Publishing

Packages are versioned from git tags using [MinVer](https://github.com/adamralph/minver): a tag such as `v1.2.3` produces version `1.2.3`, while untagged builds get a prerelease version.

To release:

1. Tag the commit and push the tag:

   ```shell
   git tag v1.2.3
   git push origin v1.2.3
   ```

2. The `Publish` workflow builds, tests and packs the library, then pauses on the `release` environment for approval.
3. Approve the deployment, and the package is pushed to nuget.org using [Trusted Publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing) (a short-lived, OIDC-issued API key — no stored secret).

The workflow can also be run manually from the Actions tab (`workflow_dispatch`).