using System;

namespace Zidium.Core.AccountsDb
{
    public abstract class AccountBasedRepository<T>
    {
        protected AccountDbContext Context { get; set; }

        protected AccountBasedRepository() { }

        protected AccountBasedRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

    }
}
