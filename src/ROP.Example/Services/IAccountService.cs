namespace ROP.Example.Services
{
    public interface IAccountService
    {
        bool AccountExists(string requestAccountFrom);
        decimal GetAccountBalance(string accountId);
        bool TransferFunds(string fromAccountId, string toAccountId, decimal amount);
    }
}
