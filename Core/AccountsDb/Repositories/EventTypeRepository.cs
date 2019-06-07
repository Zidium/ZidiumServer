using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class EventTypeRepository : AccountBasedRepository<EventType>, IEventTypeRepository
    {
        public EventTypeRepository(AccountDbContext context) : base(context) { }

        public EventType GetOrCreate(EventType eventType)
        {
            EventType typeFromDb = null;
            if (eventType.Id == Guid.Empty)
            {
                typeFromDb = Context
                    .EventTypes
                    .FirstOrDefault(t =>
                        t.Category == eventType.Category &&
                        t.SystemName == eventType.SystemName);
            }
            else
            {
                typeFromDb = Context
                    .EventTypes
                    .FirstOrDefault(t =>
                        t.Category == eventType.Category &&
                        t.Id == eventType.Id);
            }

            if (typeFromDb == null)
            {
                if (eventType.Id == Guid.Empty)
                {
                    eventType.Id = Guid.NewGuid();
                }
                eventType.CreateDate = DateTime.Now;
                typeFromDb = Context.EventTypes.Add(eventType);
                Context.SaveChanges();
            }
            else
            {
                // укажем код, если его раньше не было
                if (typeFromDb.Code == null && eventType.Code != null)
                {
                    typeFromDb.Code = eventType.Code;
                    Context.SaveChanges();
                }
            }
            return typeFromDb;
        }

        public EventType GetOneOrNullBySystemName(string systemName)
        {
            return QueryAll().FirstOrDefault(t => t.SystemName == systemName);
        }

        public EventType Add(EventType entity)
        {
            entity.Id = Guid.NewGuid();
            Context.EventTypes.Add(entity);
            Context.SaveChanges();
            return entity;
        }

        public EventType GetById(Guid id)
        {
            var result = Context.EventTypes.Find(id);

            if (result == null)
                throw new ObjectNotFoundException(id, Naming.EventType);

            return result;
        }

        public EventType GetByIdOrNull(Guid id)
        {
            return Context.EventTypes.Find(id);
        }

        public IQueryable<EventType> QueryAll()
        {
            return Context.EventTypes.Where(t => t.IsDeleted == false);
        }

        public IQueryable<EventType> QueryAllWithDeleted()
        {
            return Context.EventTypes;
        }

        public EventType Update(EventType entity)
        {
            Context.SaveChanges();
            return entity;
        }

        public void Remove(EventType entity)
        {
            entity.IsDeleted = true;
            Update(entity);
        }

        public void Remove(Guid id)
        {
            var entity = GetById(id);
            Remove(entity);
        }

    }
}
