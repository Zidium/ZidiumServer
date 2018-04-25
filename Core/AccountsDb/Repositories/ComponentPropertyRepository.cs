using System;
using System.Linq;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public class ComponentPropertyRepository : AccountBasedRepository<ComponentProperty>, IComponentPropertyRepository
    {
        public ComponentPropertyRepository(AccountDbContext context) : base(context) { }

        public ComponentProperty Add(ComponentProperty entity)
        {
            Context.ComponentProperties.Add(entity);
            Context.SaveChanges();
            return entity;
        }

        public ComponentProperty GetById(Guid id)
        {
            var result = Context.ComponentProperties.Find(id);

            if (result == null)
                throw new ObjectNotFoundException(id, Naming.ComponentProperty);

            return result;
        }

        public ComponentProperty GetByIdOrNull(Guid id)
        {
            return Context.ComponentProperties.Find(id);
        }

        public IQueryable<ComponentProperty> QueryAll()
        {
            throw new NotImplementedException();
        }

        public ComponentProperty Update(ComponentProperty entity)
        {
            Context.SaveChanges();
            return entity;
        }

        public void Remove(ComponentProperty entity)
        {
            Context.ComponentProperties.Remove(entity);
            Context.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var property = GetById(id);
            if (property != null)
                Remove(property);
        }

        public ComponentProperty Update(ComponentProperty property, string name, string value, DataType datatype)
        {
            property.Name = name;
            property.Value = value;
            property.DataType = datatype;
            property = Update(property);
            return property;
        }

        public ComponentProperty Add(Component component, string name, string value, DataType datatype)
        {
            var property = new ComponentProperty()
            {
                Id = Guid.NewGuid(),
                Component = component,
                Name = name,
                Value = value,
                DataType = datatype
            };
            property = Add(property);
            return property;
        }
    }
}
