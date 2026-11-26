using System;

using EdCourtenay.ROP.Example.Models;

using Microsoft.Extensions.Logging;

namespace EdCourtenay.ROP.Example.Services;

public class ValidationService : IValidationService
{
    private readonly IAccountService _accountService;
    private readonly ILogger<ValidationService> _logger;

    public ValidationService(IAccountService accountService, ILogger<ValidationService> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    private Func<TransferRequest, Result<TransferRequest, TransferError>> ValidateFromAccount =>
        request => _accountService.AccountExists(request.AccountFrom)
            ? request
            : new TransferError.AccountNotRecognised("AccountFrom", request.AccountFrom);

    private Func<TransferRequest, Result<TransferRequest, TransferError>> ValidateToAccount =>
        request => _accountService.AccountExists(request.AccountTo)
            ? request
            : new TransferError.AccountNotRecognised("AccountTo", request.AccountTo);

    public Func<TransferRequest, Result<TransferRequest, TransferError>> ValidateAccounts =>
        ValidateFromAccount
            .Bind(ValidateToAccount);
}