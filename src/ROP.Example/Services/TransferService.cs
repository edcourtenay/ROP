using System;

using ROP.Example.Models;

namespace ROP.Example.Services;

public class TransferService : ITransferService
{
    private readonly IAccountService _accountService;

    public TransferService(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public Func<TransferRequest, Result<TransferRequest, string>> CheckSufficientFunds =>
        request =>
        {
            var accountBalance = _accountService.GetAccountBalance(request.AccountFrom);
            return accountBalance >= request.TransferAmount
                ? request
                : "Insufficient funds";
        };


    public Func<TransferRequest, Result<RingfencedTransferRequest, string>> RingfenceSourceAccount =>
        request =>
        {
            if (request.AccountFrom.StartsWith('8'))
            {
                return $"Service timed out while attempting to ringfence {request.TransferAmount} from account {request.AccountFrom}";
            }

            return RingfencedTransferRequest.FromTransferRequest(request, Guid.NewGuid().ToString("N"));
        };

    public Func<RingfencedTransferRequest, Result<TransferResult, string>> TransferRingfencedAmount =>
        request =>
        {
            var (accountFrom, accountTo, transferAmount, reference, ringfenceReference) = request;
            if (!_accountService.TransferFunds(accountFrom, accountTo, transferAmount))
            {
                return $"Network failure while attempting to fulfill ringfence {ringfenceReference}";
            }

            return new TransferResult(accountFrom, accountTo, transferAmount,
                reference, ringfenceReference,
                _accountService.GetAccountBalance(accountFrom),
                _accountService.GetAccountBalance(accountTo));
        };
}