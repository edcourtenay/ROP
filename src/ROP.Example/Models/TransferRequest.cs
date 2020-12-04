namespace ROP.Example.Models
{
    public record TransferRequest(string AccountFrom, string AccountTo, decimal TransferAmount, string Reference)
    {
    }
}
