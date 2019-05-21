using System;
using ROP.Example.Models;

namespace ROP.Example.Services
{
    public interface ITransferService
    {
        Func<TransferRequest, Result<RingfencedTransferRequest, string>> RingfenceSourceAccount { get; }
        Func<RingfencedTransferRequest, Result<TransferResult, string>> TransferRingfencedAmount { get; }
        Func<TransferRequest, Result<TransferRequest, string>> CheckSufficentFunds { get; }
    }
}
