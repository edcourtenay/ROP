using EdCourtenay.ROP.Example.Models;
using EdCourtenay.ROP.Example.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EdCourtenay.ROP.Example.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransferController : ControllerBase
{
    private readonly IValidationService _validationService;
    private readonly ITransferService _transferService;
    private readonly ILogger<TransferController> _logger;

    public TransferController(IValidationService validationService, ITransferService transferService, ILogger<TransferController> logger)
    {
        _validationService = validationService;
        _transferService = transferService;
        _logger = logger;
    }

    // POST api/values
    [HttpPost]
    public ActionResult Post([FromBody] TransferRequest request)
    {
        var func = _validationService.ValidateAccounts
            .Tee(_ => _logger.LogInformation("Accounts validated"))
            .Bind(_transferService.CheckSufficientFunds)
            .Bind(_transferService.RingfenceSourceAccount)
            .Bind(_transferService.TransferRingfencedAmount)
            .TeeFailure(error => _logger.LogWarning("Transfer failed: {Error}", error))
            .Map(r => new TransferResponse(r.FromAccount, r.ToAccount, r.TransferAmount, r.RingfenceReference))
            .MapFailure(error => new ProblemDetails { Title = "Transfer failed", Detail = error.ToString(), Status = StatusCodes.Status400BadRequest })
            .Merge(Ok, ActionResult (problem) => BadRequest(problem));

        return func
            .Invoke(request);
    }
}