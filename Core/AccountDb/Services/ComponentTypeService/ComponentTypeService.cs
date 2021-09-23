using System;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class ComponentTypeService : IComponentTypeService
    {
        public ComponentTypeService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        public ComponentTypeForRead GetOrCreateComponentType(GetOrCreateComponentTypeRequestDataDto data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.DisplayName == null)
            {
                data.DisplayName = data.SystemName;
            }
            if (data.SystemName == null)
            {
                data.SystemName = data.DisplayName;
            }
            if (string.IsNullOrEmpty(data.SystemName))
            {
                throw new UserFriendlyException("Не задано системное имя типа компонента");
            }

            var lockObject = LockObject.ForAccount();
            lock (lockObject)
            {
                var componentType = _storage.ComponentTypes.GetOneOrNullBySystemName(data.SystemName);
                if (componentType == null)
                {
                    var componentTypeForAdd = new ComponentTypeForAdd()
                    {
                        Id = Ulid.NewUlid(),
                        SystemName = data.SystemName,
                        DisplayName = data.DisplayName
                    };
                    _storage.ComponentTypes.Add(componentTypeForAdd);

                    componentType = _storage.ComponentTypes.GetOneById(componentTypeForAdd.Id);
                }

                return componentType;
            }
        }

        public void UpdateComponentType(UpdateComponentTypeRequestDataDto data)
        {
            var componentType = _storage.ComponentTypes.GetOneById(data.Id.Value);
            if (componentType.IsSystem)
            {
                throw new CantUpdateSystemObjectException();
            }

            var componentTypeForUpdate = componentType.GetForUpdate();
            if (!string.IsNullOrEmpty(data.DisplayName))
            {
                componentTypeForUpdate.DisplayName.Set(data.DisplayName);
            }
            if (!string.IsNullOrEmpty(data.SystemName))
            {
                componentTypeForUpdate.SystemName.Set(data.SystemName);
            }

            _storage.ComponentTypes.Update(componentTypeForUpdate);
        }

        public void Delete(Guid typeId)
        {
            var componentTypeForUpdate = new ComponentTypeForUpdate(typeId);
            componentTypeForUpdate.IsDeleted.Set(true);
            _storage.ComponentTypes.Update(componentTypeForUpdate);

            var components = _storage.Components.GetByComponentTypeId(typeId);
            var componentService = new ComponentService(_storage);
            foreach (var component in components)
                componentService.DeleteComponent(component.Id);
        }
    }
}
