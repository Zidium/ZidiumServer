/*
using System;
using System.Collections.Concurrent;
using Zidium.Core.AccountsDb;
using Zidium.Core.Caching;

namespace Zidium.Core.Api.Limits.Ver2
{
    public class LimitService : ILimitService
    {
        private ConcurrentDictionary<Guid, IAccountLimitChecker> _checkers = new ConcurrentDictionary<Guid, IAccountLimitChecker>(); 
        private readonly object _getCheckerLock = new object();

        protected LimitService()
        {
        }

        public IAccountLimitChecker GetAccountLimitChecker(Guid accountId)
        {
            IAccountLimitChecker checker = null;
            _checkers.TryGetValue(accountId, out checker);
            if (checker == null)
            {
                lock (_getCheckerLock)
                {
                    _checkers.TryGetValue(accountId, out checker);
                    if (checker == null)
                    {
                        checker = new RealAccountLimitChecker();
                        _checkers.TryAdd(accountId, checker);
                    }
                }
            }
            return checker;
        }

        public void Save()
        {
            //var dataBase = AllChaches.DataBases.GetAccountDataBaseByAccountId(AccountId);
            //using (var accountDbContext = AccountDbContext.CreateFromConnectionString(dataBase.ConnectionString))
            //{

            //}
            //foreach (var checker in _checkers.Values)
            //{
            //    checker.SaveData();
            //}
        }

        public static readonly LimitService Instance = new LimitService();
    }
}

*/