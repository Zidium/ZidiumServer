using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class StatusDataRepository : AccountBasedRepository<IStatusDataRepository>, IStatusDataRepository
    {
        public StatusDataRepository(AccountDbContext accountDbContext)
            : base(accountDbContext)
        {

        }

        public Bulb Add(Bulb entity)
        {
            Context.Bulbs.Add(entity);
            Context.SaveChanges();
            return entity;
        }

        public Bulb GetById(Guid id)
        {
            var result = Context.Bulbs.Find(id);

            if (result == null)
                throw new ObjectNotFoundException(id, Naming.StatusData);

            return result;
        }

        public Bulb GetByIdOrNull(Guid id)
        {
            return Context.Bulbs.Find(id);
        }

        public IQueryable<Bulb> QueryAll()
        {
            return Context.Bulbs;
        }

        public void Remove(Bulb entity)
        {
            Context.Bulbs.Remove(entity);
        }

        public void Remove(Guid id)
        {
            var component = GetById(id);
            Remove(component);
        }
    }
}
