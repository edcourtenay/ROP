using EdCourtenay.ROP.Example.Controllers;
using EdCourtenay.ROP.Example.Models;
using EdCourtenay.ROP.Example.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace EdCourtenay.ROP.Example.Tests;

public class TransferExampleTests
{
    [Theory]
    [InlineData("7from", "7to", 10, "AccountFrom 7from not recognised")]
    [InlineData("1from", "7to", 10, "AccountTo 7to not recognised")]
    [InlineData("1from", "2to", 201, "Insufficient funds")]
    [InlineData("8from", "2to", 10, "Service timed out while attempting to ringfence 10 from account 8from")]
    public void Controller_returns_existing_failure_messages(string from, string to, int amount, string message)
    {
        var controller = CreateController();

        var result = controller.Post(new TransferRequest(from, to, amount, null));

        result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(message);
    }

    [Fact]
    public void Pipeline_preserves_typed_failures()
    {
        var accounts = new AccountService();
        var validation = new ValidationService(accounts, NullLogger<ValidationService>.Instance);
        var transfer = new TransferService(accounts);

        var invalid = validation.ValidateAccounts(new TransferRequest("7from", "2to", 10, null));
        var insufficient = transfer.CheckSufficientFunds(new TransferRequest("1from", "2to", 201, null));
        var timeout = transfer.RingfenceSourceAccount(new TransferRequest("8from", "2to", 10, null));
        var network = transfer.TransferRingfencedAmount(
            RingfencedTransferRequest.FromTransferRequest(new TransferRequest("4from", "2to", 10, null), "ref"));

        invalid.Handle(_ => throw new System.Exception("Expected failure"), error => error.Should().BeOfType<TransferError.AccountNotRecognised>());
        insufficient.Handle(_ => throw new System.Exception("Expected failure"), error => error.Should().BeOfType<TransferError.InsufficientFunds>());
        timeout.Handle(_ => throw new System.Exception("Expected failure"), error => error.Should().BeOfType<TransferError.RingfenceTimeout>());
        network.Handle(_ => throw new System.Exception("Expected failure"), error => error.Should().BeOfType<TransferError.TransferNetworkFailure>().Which.RingfenceReference.Should().Be("ref"));
    }

    [Fact]
    public void Controller_returns_success_and_network_failure()
    {
        var controller = CreateController();

        var success = controller.Post(new TransferRequest("1from", "2to", 10, null));
        var failure = controller.Post(new TransferRequest("4from", "2to", 10, null));

        success.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<TransferResult>();
        failure.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().BeOfType<string>()
            .Which.Should().StartWith("Network failure while attempting to fulfill ringfence ");
    }

    private static TransferController CreateController()
    {
        var accounts = new AccountService();
        return new TransferController(new ValidationService(accounts, NullLogger<ValidationService>.Instance),
            new TransferService(accounts), NullLogger<TransferController>.Instance);
    }
}