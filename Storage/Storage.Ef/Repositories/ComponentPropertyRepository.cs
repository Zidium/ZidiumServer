using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class ComponentPropertyRepository : IComponentPropertyRepository
    {
        public ComponentPropertyRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(ComponentPropertyForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.ComponentProperties.Add(new DbComponentProperty()
                {
                    Id = entity.Id,
                    ComponentId = entity.ComponentId,
                    DataType = entity.DataType,
                    Name = entity.Name,
                    Value = entity.Value
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(ComponentPropertyForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var property = DbGetOneById(entity.Id);

                if (entity.Name.Changed())
                    property.Name = entity.Name.Get();

                if (entity.Value.Changed())
                    property.Value = entity.Value.Get();

                if (entity.DataType.Changed())
                    property.DataType = entity.DataType.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public ComponentPropertyForRead[] GetByComponentId(Guid componentId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.ComponentProperties.AsNoTracking()
                    .Where(t => t.ComponentId == componentId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public ComponentPropertyForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public void Delete(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var entity = DbGetOneById(id);
                contextWrapper.Context.ComponentProperties.Remove(entity);
                contextWrapper.Context.SaveChanges();
            }
        }

        private DbComponentProperty DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.ComponentProperties.Find(id);
            }
        }

        private DbComponentProperty DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Свойство {id} не найдено");

            return result;
        }

        private ComponentPropertyForRead DbToEntity(DbComponentProperty entity)
        {
            if (entity == null)
                return null;

            return new ComponentPropertyForRead(entity.Id, entity.ComponentId, entity.Name, entity.Value, entity.DataType);
        }
    }
}
