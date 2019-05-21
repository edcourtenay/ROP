namespace ROP.Example.Models
{
    public class TransferResult
    {
        public string FromAccount { get; }
        public string ToAccount { get; }
        public decimal TransferAmount { get; }
        public string Reference { get; }
        public string RingfenceReference { get; }
        public decimal FromAccountBalance { get; }
        public decimal ToAccountBalance { get; }

        public TransferResult(string fromAccount, string ToAccount, decimal transferAmount, string reference, string ringfenceReference, decimal fromAccountBalance, decimal toAccountBalance)
        {
            FromAccount = fromAccount;
            this.ToAccount = ToAccount;
            TransferAmount = transferAmount;
            Reference = reference;
            RingfenceReference = ringfenceReference;
            FromAccountBalance = fromAccountBalance;
            ToAccountBalance = toAccountBalance;
        }
    }
}