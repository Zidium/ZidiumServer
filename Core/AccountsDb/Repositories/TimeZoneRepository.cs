using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class TimeZoneRepository : ITimeZoneRepository
    {
        protected AccountDbContext Context;

        public TimeZoneRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public IQueryable<TimeZone> QueryAll()
        {
            return Context.TimeZones;
        }

        public TimeZone GetOneByOffsetMinutes(int offsetMinutes)
        {
            var entity = Context.TimeZones.Find(offsetMinutes);

            if (entity == null)
                throw new ObjectNotFoundException("Не найден часовой пояс со смещением " + offsetMinutes + " минут");

            return entity;
        }
    }
}
