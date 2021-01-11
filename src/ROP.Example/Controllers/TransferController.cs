using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ROP.Example.Models;
using ROP.Example.Services;

namespace ROP.Example.Controllers
{
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
                            .Tee(transferRequest => _logger.LogInformation("Accounts validated"))
                            .Bind(_transferService.CheckSufficientFunds)
                            .Bind(_transferService.RingfenceSourceAccount)
                            .Bind(_transferService.TransferRingfencedAmount)
                            .Merge(Ok, s => (ActionResult)BadRequest(s));
            return func
                .Invoke(request);
        }
    }
}
