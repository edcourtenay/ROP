namespace ROP.Example.Models
{
    public class RingfencedTransferRequest
    {
        public string AccountFrom { get; }
        public string AccountTo { get; }
        public decimal TransferAmount { get; }
        public string Reference { get; }
        public string RingfenceReference { get; }

        public RingfencedTransferRequest(string accountFrom, string accountTo, decimal transferAmount,
            string reference, string ringfenceReference)
        {
            AccountFrom = accountFrom;
            AccountTo = accountTo;
            TransferAmount = transferAmount;
            Reference = reference;
            RingfenceReference = ringfenceReference;
        }

        public static RingfencedTransferRequest FromTransferRequest(TransferRequest transferRequest,
            string ringfenceReference) =>
            new RingfencedTransferRequest(transferRequest.AccountFrom, transferRequest.AccountTo, transferRequest.TransferAmount, transferRequest.Reference, ringfenceReference);
    }
}