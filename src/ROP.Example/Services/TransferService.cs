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

    public Func<TransferRequest, Result<TransferRequest, TransferError>> CheckSufficientFunds =>
        request =>
        {
            var accountBalance = _accountService.GetAccountBalance(request.AccountFrom);
            return accountBalance >= request.TransferAmount
                ? request
                : new TransferError.InsufficientFunds();
        };


    public Func<TransferRequest, Result<RingfencedTransferRequest, TransferError>> RingfenceSourceAccount =>
        request =>
        {
            if (request.AccountFrom.StartsWith('8'))
            {
                return new TransferError.RingfenceTimeout(request.TransferAmount, request.AccountFrom);
            }

            return RingfencedTransferRequest.FromTransferRequest(request, Guid.NewGuid().ToString("N"));
        };

    public Func<RingfencedTransferRequest, Result<TransferResult, TransferError>> TransferRingfencedAmount =>
        request =>
        {
            var (accountFrom, accountTo, transferAmount, reference, ringfenceReference) = request;
            if (!_accountService.TransferFunds(accountFrom, accountTo, transferAmount))
            {
                return new TransferError.TransferNetworkFailure(ringfenceReference);
            }

            return new TransferResult(accountFrom, accountTo, transferAmount,
                reference, ringfenceReference,
                _accountService.GetAccountBalance(accountFrom),
                _accountService.GetAccountBalance(accountTo));
        };
}