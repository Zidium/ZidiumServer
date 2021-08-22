using System;
using Zidium.Storage;

namespace Zidium.Core.Limits
{
    // TODO Move to AccountLimitsChecker
    public static class AccountLimitsCheckerManager
    {
        private static readonly object LockObject = new object();

        private static AccountLimitsChecker _checker;

        public static AccountLimitsChecker GetChecker()
        {
            lock (LockObject)
            {
                if (_checker == null)
                {
                    _checker = new AccountLimitsChecker();
                }

                return _checker;
            }
        }

        public static int Save()
        {
            var storageFactory = DependencyInjection.GetServicePersistent<IStorageFactory>();
            LastSaveException = null;
            int count = 0;

            AccountLimitsChecker checker;
            lock (LockObject)
            {
                checker = _checker;
            }

            if (checker != null)
            {
                try
                {
                    var storage = storageFactory.GetStorage();
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
