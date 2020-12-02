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
                    ? new Result<TransferRequest, string>.Success(request)
                    : new Result<TransferRequest, string>.Failure("Insufficient funds");
            };


        public Func<TransferRequest, Result<RingfencedTransferRequest, string>> RingfenceSourceAccount =>
            request =>
            {
                if (request.AccountFrom.StartsWith("8"))
                {
                    return new Result<RingfencedTransferRequest, string>.Failure($"Service timed out while attempting to ringfence {request.TransferAmount} from account {request.AccountFrom}");
                }

                var ringfencedRequest =
                    RingfencedTransferRequest.FromTransferRequest(request, Guid.NewGuid().ToString("N"));
                return new Result<RingfencedTransferRequest, string>.Success(ringfencedRequest);
            };

        public Func<RingfencedTransferRequest, Result<TransferResult, string>> TransferRingfencedAmount =>
            request =>
            {
                if (!_accountService.TransferFunds(request.AccountFrom, request.AccountTo, request.TransferAmount))
                {
                    return new Result<TransferResult, string>.Failure($"Network failure while attempting to fulfill ringfence {request.RingfenceReference} ");
                }

                var transferResult = new TransferResult(request.AccountFrom, request.AccountTo, request.TransferAmount,
                    request.Reference, request.RingfenceReference,
                    _accountService.GetAccountBalance(request.AccountFrom),
                    _accountService.GetAccountBalance(request.AccountTo));

                return new Result<TransferResult, string>.Success(transferResult);
            };
    }
}
