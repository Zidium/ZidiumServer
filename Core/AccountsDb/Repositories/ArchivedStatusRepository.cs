using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class ArchivedStatusRepository : IArchivedStatusRepository
    {
        protected AccountDbContext Context { get; set; }

        public ArchivedStatusRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public void Add(ArchivedStatus archivedStatus)
        {
            if (archivedStatus == null)
            {
                throw new ArgumentNullException("archivedStatus");
            }
            Context.ArchivedStatuses.Add(archivedStatus);
        }

        public IQueryable<ArchivedStatus> GetTop(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            return Context.ArchivedStatuses.OrderBy(x => x.Id).Take(count);
        }

        public void Delete(ArchivedStatus archivedStatus)
        {
            if (archivedStatus == null)
            {
                throw new ArgumentNullException("archivedStatus");
            }
            Context.ArchivedStatuses.Remove(archivedStatus);
        }
    }
}
