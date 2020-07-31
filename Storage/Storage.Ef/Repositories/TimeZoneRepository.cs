using System.Linq;

namespace Zidium.Storage.Ef
{
    internal class TimeZoneRepository : ITimeZoneRepository
    {
        public TimeZoneRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(TimeZoneForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.TimeZones.Add(new DbTimeZone()
                {
                    Name = entity.Name,
                    OffsetMinutes = entity.OffsetMinutes
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public TimeZoneForRead[] GetAll()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.TimeZones.AsNoTracking()
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public TimeZoneForRead GetOneByOffsetMinutes(int offsetMinutes)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.TimeZones.AsNoTracking()
                    .First(t => t.OffsetMinutes == offsetMinutes));
            }
        }

        private TimeZoneForRead DbToEntity(DbTimeZone entity)
        {
            if (entity == null)
                return null;

            return new TimeZoneForRead(entity.OffsetMinutes, entity.Name);
        }
    }
}
