using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Репозиторий для работы с типом метрик
    /// </summary>
    public class MetricTypeRepository : AccountBasedRepository<MetricType>, IMetricTypeRepository
    {
        public MetricTypeRepository(AccountDbContext context) : base(context) { }

        public MetricType Add(MetricType entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            Context.MetricTypes.Add(entity);
            return entity;
        }

        public MetricType GetById(Guid id)
        {
            var entity = GetByIdOrNull(id);

            if (entity == null)
                throw new ObjectNotFoundException(id, Naming.Counter);

            return entity;
        }

        public MetricType GetByIdOrNull(Guid id)
        {
            return Context.MetricTypes.Find(id);
        }

        public IQueryable<MetricType> QueryAll()
        {
            return Context.MetricTypes.Where(t => t.IsDeleted == false);
        }

        public MetricType Update(MetricType entity)
        {
            Context.SaveChanges();
            return entity;
        }

        public void Remove(MetricType entity)
        {
            entity.IsDeleted = true;
            Context.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var entity = GetById(id);
            Remove(entity);
        }

        public MetricType GetOneOrNullByName(string name)
        {
            return QueryAll().FirstOrDefault(t => t.SystemName.ToLower() == name.ToLower());
        }
    }
}
