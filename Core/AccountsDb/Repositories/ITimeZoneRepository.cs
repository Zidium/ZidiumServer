using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public interface ITimeZoneRepository
    {
        IQueryable<TimeZone> QueryAll();

        TimeZone GetOneByOffsetMinutes(int offsetMinutes);
    }
}
