namespace ROP.Example.Models
{
    public class TransferRequest
    {
        public TransferRequest(string accountFrom, string accountTo, decimal transferAmount, string reference)
        {
            AccountFrom = accountFrom;
            AccountTo = accountTo;
            TransferAmount = transferAmount;
            Reference = reference;
        }

        public string AccountFrom { get; }
        public string AccountTo { get; }
        public decimal TransferAmount { get; }
        public string Reference { get; }
    }
}
