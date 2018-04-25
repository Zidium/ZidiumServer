using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Репозиторий для работы с подписками
    /// </summary>
    public class SubscriptionRepository : AccountBasedRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(AccountDbContext context) : base(context) { }

        public Subscription GetById(Guid id)
        {
            var result = Context.Subscriptions.Find(id);

            if (result == null)
                throw new ObjectNotFoundException(id, Naming.Subscription);

            return result;
        }

        public Subscription GetByIdOrNull(Guid id)
        {
            return Context.Subscriptions.Find(id);
        }

        public IQueryable<Subscription> QueryAll()
        {
            return Context.Subscriptions;
        }

        public void Remove(Subscription entity)
        {
            Context.Subscriptions.Remove(entity);
        }

        public Subscription Add(Subscription entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
            entity = Context.Subscriptions.Add(entity);
            return entity;
        }

        public void Remove(Guid id)
        {
            var entity = GetById(id);
            Remove(entity);
        }
    }
}
