using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class MetricRepository : AccountBasedRepository<Metric>, IMetricRepository
    {
        public MetricRepository(AccountDbContext context) : base(context) { }

        public Metric Add(Metric entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
            Context.Metrics.Add(entity);
            Context.SaveChanges();
            return entity;
        }

        public Metric GetById(Guid id)
        {
            var entity = GetByIdOrNull(id);

            if (entity == null)
                throw new ObjectNotFoundException(id, Naming.Counter);

            return entity;
        }

        public Metric GetByIdOrNull(Guid id)
        {
            return Context.Metrics.Find(id);
        }

        public IQueryable<Metric> QueryAll()
        {
            return Context.Metrics.Where(t => t.IsDeleted == false && t.MetricType.IsDeleted == false);
        }

        public Metric Update(Metric entity)
        {
            Context.SaveChanges();
            return entity;
        }

        public void Remove(Metric entity)
        {
            entity.IsDeleted = true;
            Context.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var entity = GetById(id);
            Remove(entity);
        }

        public List<Metric> GetNotActual(int maxCount)
        {
            var now = DateTime.Now;

            return QueryAll()
                .Where(x => x.ActualDate < now)
                .OrderBy(x => x.ActualDate)
                .Take(maxCount)
                .ToList();
        }

        public int GetCount()
        {
            return QueryAll().Count();
        }
    }
}
