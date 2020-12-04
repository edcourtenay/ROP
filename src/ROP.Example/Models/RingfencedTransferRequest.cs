namespace ROP.Example.Models
{
    public record RingfencedTransferRequest(string AccountFrom, string AccountTo, decimal TransferAmount,
            string Reference, string RingfenceReference)
    {
        public static RingfencedTransferRequest FromTransferRequest(TransferRequest transferRequest, string ringfenceReference) =>
            new RingfencedTransferRequest(transferRequest.AccountFrom,
                transferRequest.AccountTo,
                transferRequest.TransferAmount,
                transferRequest.Reference,
                ringfenceReference);
    }
}
