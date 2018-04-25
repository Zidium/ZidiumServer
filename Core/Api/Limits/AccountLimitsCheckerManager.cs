using System;
using System.Collections.Generic;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Limits
{
    public static class AccountLimitsCheckerManager
    {
        private static readonly object LockObject = new object();

        private static Dictionary<Guid, AccountLimitsChecker> Checkers = new Dictionary<Guid, AccountLimitsChecker>();

        public static AccountLimitsChecker GetCheckerForAccount(Guid accountId)
        {
            lock (LockObject)
            {
                AccountLimitsChecker result;

                if (Checkers.ContainsKey(accountId))
                    result = Checkers[accountId];
                else
                {
                    result = new AccountLimitsChecker(accountId);
                    Checkers.Add(accountId, result);
                }

                return result;
            }
        }

        public static int Save()
        {
            LastSaveException = null;
            int count = 0;

            List<AccountLimitsChecker> checkers;
            lock (LockObject)
            {
                checkers = new List<AccountLimitsChecker>(Checkers.Values);
            }

            foreach (var checker in checkers)
            {
                try
                {
                    using (var context = AccountDbContext.CreateFromAccountIdLocalCache(checker.AccountId))
                    {
                        count += checker.SaveData(context);
                    }
                }
                catch (Exception exception)
                {
                    LastSaveException = exception;
                }
            }

            return count;
        }

        public static Exception LastSaveException { get; internal set; }
    }
}
