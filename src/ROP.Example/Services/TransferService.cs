using System;
using ROP.Example.Models;

namespace ROP.Example.Services
{
    public class TransferService : ITransferService
    {
        private readonly IAccountService _accountService;

        public TransferService(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public Func<TransferRequest, Result<TransferRequest, string>> CheckSufficentFunds =>
            request =>
            {
                var accountBalance = _accountService.GetAccountBalance(request.AccountFrom);
                return accountBalance >= request.TransferAmount
                    ? Result<TransferRequest, string>.NewSuccess(request)
                    : Result<TransferRequest, string>.NewFailure("Insufficient funds");
            };


        public Func<TransferRequest, Result<RingfencedTransferRequest, string>> RingfenceSourceAccount =>
            request =>
            {
                if (request.AccountFrom.StartsWith("8"))
                {
                    return Result<RingfencedTransferRequest, string>.NewFailure($"Service timed out while attempting to ringfence {request.TransferAmount} from account {request.AccountFrom}");
                }

                var ringfencedRequest =
                    RingfencedTransferRequest.FromTransferRequest(request, Guid.NewGuid().ToString("N"));
                return Result<RingfencedTransferRequest, string>.NewSuccess(ringfencedRequest);
            };

        public Func<RingfencedTransferRequest, Result<TransferResult, string>> TransferRingfencedAmount =>
            request =>
            {
                if (!_accountService.TransferFunds(request.AccountFrom, request.AccountTo, request.TransferAmount))
                {
                    return Result<TransferResult, string>.NewFailure($"Network failure while attempting to fulfill ringfence {request.RingfenceReference} ");
                }

                var transferResult = new TransferResult(request.AccountFrom, request.AccountTo, request.TransferAmount,
                    request.Reference, request.RingfenceReference,
                    _accountService.GetAccountBalance(request.AccountFrom),
                    _accountService.GetAccountBalance(request.AccountTo));

                return Result<TransferResult, string>.NewSuccess(transferResult);
            };
    }
}
