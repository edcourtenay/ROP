using Microsoft.Extensions.DependencyInjection;

using ROP.Example;
using ROP.Example.Services;

namespace ROP.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void ExampleServicesAreScopedAndResolvable()
    {
        using var host = Program.CreateWebHostBuilder(new string[0]).Build();
        using var scope = host.Services.CreateScope();
        using var otherScope = host.Services.CreateScope();

        Assert.IsType<TransferService>(scope.ServiceProvider.GetRequiredService<ITransferService>());
        Assert.IsType<ValidationService>(scope.ServiceProvider.GetRequiredService<IValidationService>());
        var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
        Assert.IsType<AccountService>(accountService);
        Assert.Same(accountService, scope.ServiceProvider.GetRequiredService<IAccountService>());
        Assert.NotSame(accountService, otherScope.ServiceProvider.GetRequiredService<IAccountService>());
    }
}