using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    class UnitTestTypeRepository : AccountBasedRepository<UnitTestType>, IUnitTestTypeRepository
    {
        public UnitTestTypeRepository(AccountDbContext context) : base(context) { }

        public UnitTestType Add(UnitTestType unitTestType)
        {
            if (unitTestType.Id == Guid.Empty)
            {
                unitTestType.Id = Guid.NewGuid();
                unitTestType.CreateDate = DateTime.Now;
            }

            Context.UnitTestTypes.Add(unitTestType);
            Context.SaveChanges();

            return unitTestType;
        }

        public UnitTestType GetOneOrNullByIdOrSystemName(Guid id, string systemName)
        {
            if (id != Guid.Empty)
                return Context.UnitTestTypes.FirstOrDefault(
                    x => x.Id == id
                         && x.IsDeleted == false);
            else
                return Context.UnitTestTypes.FirstOrDefault(
                    x => x.SystemName == systemName
                         && x.IsDeleted == false);
        }

        public UnitTestType GetOneOrNullBySystemName(string systemName)
        {
            return QueryAll().FirstOrDefault(t => t.SystemName == systemName);
        }

        public IQueryable<UnitTestType> QueryAllForGui(string search)
        {
            var query = QueryAll();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.SystemName.Contains(search) || t.DisplayName.Contains(search));
            return query;
        }

        public UnitTestType GetById(Guid id)
        {
            var result = Context.UnitTestTypes.Find(id);

            if (result == null)
                throw new ObjectNotFoundException(id, Naming.UnitTestType);

            return result;
        }

        public UnitTestType GetByIdOrNull(Guid id)
        {
            return Context.UnitTestTypes.Find(id);
        }

        public UnitTestType Update(UnitTestType unitTestType)
        {
            Context.SaveChanges();
            return unitTestType;
        }

        public void Remove(UnitTestType unitTestType)
        {
            unitTestType.IsDeleted = true;
            Update(unitTestType);
        }

        public IQueryable<UnitTestType> QuerySystemForGui()
        {
            return QueryAll().Where(t => t.IsSystem);
        }

        public IQueryable<UnitTestType> QueryAllCustom()
        {
            return QueryAll().Where(t => !t.IsSystem);
        }

        public IQueryable<UnitTestType> QueryAll()
        {
            return Context.UnitTestTypes.Where(t => t.IsDeleted == false);
        }

        public void Remove(Guid id)
        {
            var entity = GetById(id);
            Remove(entity);
        }
    }
}
