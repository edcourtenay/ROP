using System;

using EdCourtenay.ROP.Example.Models;

namespace EdCourtenay.ROP.Example.Services;

public interface IValidationService
{
    Func<TransferRequest, Result<TransferRequest, TransferError>> ValidateAccounts { get; }
}