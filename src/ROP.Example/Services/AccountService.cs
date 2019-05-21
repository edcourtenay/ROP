using System.Collections.Concurrent;

namespace ROP.Example.Services
{
    public class AccountService : IAccountService
    {
        private ConcurrentDictionary<string, decimal> _accounts = new ConcurrentDictionary<string, decimal>(); 

        public bool AccountExists(string requestAccountFrom) => !requestAccountFrom.StartsWith("7");

        public decimal GetAccountBalance(string accountId) => _accounts.GetOrAdd(accountId, 200m);

        public bool TransferFunds(string fromAccountId, string toAccountId, decimal amount)
        {
            if (fromAccountId.StartsWith("4"))
                return false;

            var fromBalance = GetAccountBalance(fromAccountId);
            var toBalance = GetAccountBalance(toAccountId);

            _accounts.AddOrUpdate(fromAccountId, s => fromBalance - amount, (s, current) => current - amount);
            _accounts.AddOrUpdate(toAccountId, s => toBalance + amount, (s, current) => current + amount);

            return true;
        }
    }
}
