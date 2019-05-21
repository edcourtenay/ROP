using System;
using ROP.Example.Models;

namespace ROP.Example.Services
{
    public interface IValidationService
    {
        Func<TransferRequest, Result<TransferRequest, string>> ValidateAccounts { get; }
    }
}