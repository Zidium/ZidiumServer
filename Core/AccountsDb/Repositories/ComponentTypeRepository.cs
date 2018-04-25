using System;
using System.Linq;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class ComponentTypeRepository : AccountBasedRepository<ComponentType>, IComponentTypeRepository
    {
        public ComponentTypeRepository(AccountDbContext context) : base(context) { }

        public ComponentType Add(string displayName, string systemName)
        {
            var componentType = new ComponentType()
            {
                Id = Guid.NewGuid(),
                DisplayName = displayName,
                SystemName = systemName
            };
            componentType = Add(componentType);
            return componentType;
        }

        public void Remove(ComponentType componentType)
        {
            componentType.IsDeleted = true;
            Context.SaveChanges();
        }

        public void Remove(Guid componentTypeId)
        {
            var componentType = GetById(componentTypeId);
            Remove(componentType);
        }

        public ComponentType GetOneOrNullBySystemName(string systemName)
        {
            return QueryAll().FirstOrDefault(x => x.SystemName.Equals(systemName, StringComparison.OrdinalIgnoreCase));
        }

        public ComponentType UpdateBySystemName(string systemName, string displayName)
        {
            EmptyParameterException.Validate("systemName", systemName);
            var result = GetOneOrNullBySystemName(systemName);
            if (result != null)
            {
                result.DisplayName = displayName;
                result = Update(result);
            }
            return result;
        }

        public ComponentType Update(ComponentType componentType, string displayName, string systemName)
        {
            componentType.DisplayName = displayName;
            componentType.SystemName = systemName;
            componentType = Update(componentType);
            return componentType;
        }

        public ComponentType Update(Guid componentTypeId, string displayName, string systemName)
        {
            var componentType = GetById(componentTypeId);
            return Update(componentType, displayName, systemName);
        }

        public ComponentType Add(ComponentType entity)
        {
            Context.ComponentTypes.Add(entity);
            Context.SaveChanges();
            return entity;
        }

        public ComponentType GetById(Guid id)
        {
            var result = GetByIdOrNull(id);

            if (result == null)
                throw new ObjectNotFoundException(id, Naming.ComponentType);

            return result;
        }

        public ComponentType GetByIdOrNull(Guid id)
        {
            return Context.ComponentTypes.Find(id);
        }

        public IQueryable<ComponentType> QueryAll()
        {
            return Context.ComponentTypes.Where(x => x.IsDeleted == false);
        }

        public ComponentType Update(ComponentType entity)
        {
            Context.SaveChanges();
            return entity;
        }
    }
}
