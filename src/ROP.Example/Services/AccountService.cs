using System.Collections.Concurrent;

namespace EdCourtenay.ROP.Example.Services;

public class AccountService : IAccountService
{
    private readonly ConcurrentDictionary<string, decimal> _accounts = new();

    public bool AccountExists(string requestAccountFrom) => !requestAccountFrom.StartsWith('7');

    public decimal GetAccountBalance(string accountId) => _accounts.GetOrAdd(accountId, 200m);

    public bool TransferFunds(string fromAccountId, string toAccountId, decimal amount)
    {
        if (fromAccountId.StartsWith('4'))
            return false;

        var fromBalance = GetAccountBalance(fromAccountId);
        var toBalance = GetAccountBalance(toAccountId);

        _accounts.AddOrUpdate(fromAccountId, _ => fromBalance - amount, (s, current) => current - amount);
        _accounts.AddOrUpdate(toAccountId, _ => toBalance + amount, (s, current) => current + amount);

        return true;
    }
}