using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public interface IArchivedStatusRepository
    {
        void Add(ArchivedStatus archivedStatus);

        IQueryable<ArchivedStatus> GetTop(int count);

        void Delete(ArchivedStatus archivedStatus);
    }
}
