namespace EdCourtenay.ROP.Example.Models;

public record TransferResponse(
    string FromAccount,
    string ToAccount,
    decimal TransferAmount,
    string RingfenceReference);