using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    class UnitTestRepository : AccountBasedRepository<UnitTest>, IUnitTestRepository
    {
        public UnitTestRepository(AccountDbContext context) : base(context) { }

        public UnitTest Add(UnitTest unitTest)
        {
            if (unitTest.Id == Guid.Empty)
                unitTest.Id = Guid.NewGuid();
            unitTest.CreateDate = DateTime.Now;
            Context.UnitTests.Add(unitTest);
            Context.SaveChanges();
            return unitTest;
        }

        public UnitTest GetById(Guid id)
        {
            var result = Context.UnitTests.Find(id);

            if (result == null)
                throw new ObjectNotFoundException(id, Naming.UnitTest);
            return result;
        }

        public UnitTest GetByIdOrNull(Guid id)
        {
            return Context.UnitTests.Find(id);
        }

        public IQueryable<UnitTest> QueryAll()
        {
            return Context.UnitTests.Where(t => t.IsDeleted == false && t.Component.IsDeleted == false && t.Type.IsDeleted == false);
        }

        public IQueryable<UnitTest> QueryForGui(Guid? componentTypeId,
            Guid? componentId,
            Guid? unitTestTypeId,
            List<MonitoringStatus> statuses)
        {
            var query = QueryAll();
            if (componentTypeId.HasValue)
                query = query.Where(t => t.Component.ComponentTypeId == componentTypeId);
            if (componentId.HasValue)
                query = query.Where(t => t.ComponentId == componentId.Value);
            if (unitTestTypeId.HasValue)
                query = query.Where(t => t.TypeId == unitTestTypeId.Value);
            if (statuses != null && statuses.Count > 0)
                query = query.Where(t => t.Bulb != null && statuses.Contains(t.Bulb.Status));
            return query;
        }

        public UnitTest Update(UnitTest unitTest)
        {
            Context.SaveChanges();
            return unitTest;
        }

        public void Remove(UnitTest unitTest)
        {
            unitTest.IsDeleted = true;
            unitTest.Bulb.IsDeleted = true;
            Context.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var entity = GetById(id);
            Remove(entity);
        }

        public int GetCount()
        {
            return QueryAll().Count();
        }

        public UnitTest GetOneOrNull(Guid componentId,
            Guid unitTestTypeId,
            string systemName)
        {
            return Context.UnitTests.SingleOrDefault(x =>
                x.ComponentId == componentId
                && x.TypeId == unitTestTypeId
                && x.SystemName == systemName
                && x.IsDeleted == false);
        }

        public List<UnitTest> GetForProcessing(Guid typeId, DateTime date)
        {
            return Context.UnitTests
                .Include("Component")
                .Where(x => x.TypeId == typeId
                            && x.IsDeleted == false
                            && x.Enable
                            && x.ParentEnable
                            && (x.NextExecutionDate <= date || x.NextExecutionDate == null))
                .ToList();
        }

        public IQueryable<UnitTest> QueryNonActual()
        {
            var now = DateTime.Now;
            return QueryAll().Where(x => x.Bulb.ActualDate < now);
        }

        public IQueryable<UnitTest> QueryAllWithDeleted()
        {
            return Context.UnitTests;
        }
    }
}
