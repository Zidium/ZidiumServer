using System;
using System.Collections.Generic;

namespace Zidium.Core.Limits
{
    public static class AccountLimitsCheckerManager
    {
        private static readonly object LockObject = new object();

        private static readonly Dictionary<Guid, AccountLimitsChecker> Checkers = new Dictionary<Guid, AccountLimitsChecker>();

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
            var accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();
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
                    var storage = accountStorageFactory.GetStorageByAccountId(checker.AccountId);
                    count += checker.SaveData(storage);
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
