using System.ComponentModel;

namespace EdCourtenay.ROP.Example.Models;

public record TransferRequest(
    [property: DefaultValue("12345678")] string AccountFrom,
    [property: DefaultValue("12445690")] string AccountTo,
    [property: DefaultValue(typeof(decimal), "56.0")] decimal TransferAmount,
    [property: DefaultValue("Example transfer")] string? Reference);