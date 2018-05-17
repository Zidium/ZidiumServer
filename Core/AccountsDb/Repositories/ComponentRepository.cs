using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class ComponentRepository : AccountBasedRepository<Component>, IComponentRepository
    {
        public ComponentRepository(AccountDbContext context) : base(context) { }

        public int GetCount()
        {
            return QueryAll().Count();
        }

        public Component GetRoot()
        {
            return QueryAll().First(t => t.ParentId == null);
        }

        public Component GetById(Guid id)
        {
            var result = GetByIdOrNull(id);

            if (result == null)
                throw new ObjectNotFoundException(id, Naming.Component);

            return result;
        }

        public Component GetByIdOrNull(Guid id)
        {
            return Context.Components.Find(id);
        }

        public Component Add(Component component)
        {
            component = Context.Components.Add(component);
            Context.SaveChanges();
            return component;
        }

        public List<Component> GetAllBySystemName(string systemName)
        {
            return QueryAll().Where(x => x.SystemName.ToLower() == systemName.ToLower()).ToList();
        } 

        public IQueryable<Component> QueryAll()
        {
            return Context.Components.Where(t => t.IsDeleted == false);
        }

        public List<Component> GetChildComponents(Guid parentComponentId)
        {
            return QueryAll().Where(x => x.ParentId == parentComponentId).ToList();
        }

        public IQueryable<Component> GetWhereNotActualEventsStatus()
        {
            var now = DateTime.Now;

            var components = QueryAll().Where(x => x.EventsStatus.ActualDate < now);

            return components;
        }

        public IQueryable<Component> QueryAllWithDeleted()
        {
            return Context.Components;
        }

        public Component GetChild(Guid parentId, string systemName)
        {
            return QueryAll().SingleOrDefault(x => x.ParentId == parentId && x.SystemName == systemName);
        }

        public Component GetBySystemName(string systemName)
        {
            return QueryAll().SingleOrDefault(x => x.SystemName == systemName);
        }
    }
}
