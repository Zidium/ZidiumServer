using System;
using System.Linq;

namespace Zidium.Core.AccountsDb.Repositories
{
    public class HttpRequestUnitTestRuleRepository : AccountBasedRepository<HttpRequestUnitTestRule>, IHttpRequestUnitTestRuleRepository
    {
        public HttpRequestUnitTestRuleRepository(AccountDbContext context) : base(context) { }

        public HttpRequestUnitTestRule Add(HttpRequestUnitTestRule entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
            Context.HttpRequestUnitTestRules.Add(entity);
            Context.SaveChanges();
            return entity;
        }

        public HttpRequestUnitTestRule GetById(Guid id)
        {
            var result = Context.HttpRequestUnitTestRules.Find(id);

            if (result == null)
                throw new ObjectNotFoundException(id, Naming.HttpRequestUnitTestRule);

            return result;
        }

        public HttpRequestUnitTestRule GetByIdOrNull(Guid id)
        {
            return Context.HttpRequestUnitTestRules.Find(id);
        }

        public IQueryable<HttpRequestUnitTestRule> QueryAll()
        {
            return Context.HttpRequestUnitTestRules.Where(t => t.HttpRequestUnitTest.UnitTest.IsDeleted == false && t.HttpRequestUnitTest.UnitTest.Component.IsDeleted == false);
        }

        public HttpRequestUnitTestRule Update(HttpRequestUnitTestRule entity)
        {
            Context.SaveChanges();
            return entity;
        }

        public void Remove(HttpRequestUnitTestRule entity)
        {
            entity.IsDeleted = true;
            Context.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var entity = GetById(id);
            Remove(entity);
        }
    }
}
