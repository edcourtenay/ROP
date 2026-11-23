using System;

using ROP.Example.Models;

namespace ROP.Example.Services;

public interface ITransferService
{
    Func<TransferRequest, Result<RingfencedTransferRequest, TransferError>> RingfenceSourceAccount { get; }
    Func<RingfencedTransferRequest, Result<TransferResult, TransferError>> TransferRingfencedAmount { get; }
    Func<TransferRequest, Result<TransferRequest, TransferError>> CheckSufficientFunds { get; }
}