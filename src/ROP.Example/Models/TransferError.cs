namespace EdCourtenay.ROP.Example.Models;

public closed record TransferError
{
    public sealed record AccountNotRecognised(string AccountField, string Account) : TransferError
    {
        public override string ToString() => $"{AccountField} {Account} not recognised";
    }

    public sealed record InsufficientFunds : TransferError
    {
        public override string ToString() => "Insufficient funds";
    }

    public sealed record RingfenceTimeout(decimal Amount, string Account) : TransferError
    {
        public override string ToString() => $"Service timed out while attempting to ringfence {Amount} from account {Account}";
    }

    public sealed record TransferNetworkFailure(string RingfenceReference) : TransferError
    {
        public override string ToString() => $"Network failure while attempting to fulfill ringfence {RingfenceReference}";
    }
}