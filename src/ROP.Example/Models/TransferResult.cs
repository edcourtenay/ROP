namespace ROP.Example.Models
{
    public record TransferResult(
        string FromAccount,
        string ToAccount,
        decimal TransferAmount,
        string Reference,
        string RingfenceReference,
        decimal FromAccountBalance,
        decimal ToAccountBalance)
    {
    }
}
