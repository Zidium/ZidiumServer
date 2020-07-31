using System;
using System.Linq;
using Zidium.Api.Others;
using Zidium.Core.Api;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public class ComponentCacheStorage : AccountDbCacheStorageBase<ComponentCacheResponse, IComponentCacheReadObject, ComponentCacheWriteObject>
    {
        protected override Exception CreateNotFoundException(AccountCacheRequest request)
        {
            return new UnknownComponentIdException(request.ObjectId, request.AccountId);
        }

        protected override void ValidateChanges(ComponentCacheWriteObject oldComponent, ComponentCacheWriteObject newComponent)
        {
            if (oldComponent == null)
            {
                throw new ArgumentNullException("oldComponent");
            }
            if (newComponent == null)
            {
                throw new ArgumentNullException("newComponent");
            }
            if (oldComponent.AccountId != newComponent.AccountId)
            {
                throw new ArgumentNullException("Нельзя изменять AccountId");
            }
            if (newComponent.AccountId == Guid.Empty)
            {
                throw new ArgumentNullException("AccountId is Empty");
            }
            if (newComponent.ParentId == null)
            {
                if (newComponent.IsRoot == false)
                {
                    throw new ArgumentException("Не указан ParentId");
                }
            }
            if (newComponent.SystemName == null)
            {
                throw new ArgumentException("Не указан SystemName");
            }
            if (StringHelper.GetLength(newComponent.DisplayName) > 255)
            {
                throw new Exception("StringHelper.GetLength(newComponent.DisplayName) > 255");
            }
            if (StringHelper.GetLength(newComponent.SystemName) > 255)
            {
                throw new Exception("StringHelper.GetLength(newComponent.SystemName) > 255");
            }
            if (StringHelper.GetLength(newComponent.DisableComment) > 1000)
            {
                throw new Exception("StringHelper.GetLength(newComponent.DisableComment) > 1000");
            }

            var accountId = newComponent.AccountId;

            // если объект удалили
            if (newComponent.IsDeleted)
            {
                using (var parent = AllCaches.Components.Write(new AccountCacheRequest()
                {
                    AccountId = newComponent.AccountId,
                    ObjectId = newComponent.ParentId.Value
                }))
                {
                    parent.WriteChilds.Delete(newComponent.Id);
                    parent.BeginSave();
                };
            }

            // если изменился родитель
            if (newComponent.ParentId != oldComponent.ParentId)
            {
                // проверим наличие нового родителя
                var parentRequest = new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = newComponent.ParentId.Value
                };
                using (var parent = AllCaches.Components.Write(parentRequest))
                {
                    // проверим, что у нового родителя нет детей с таким же именем
                    var child = parent.Childs.FindByName(newComponent.SystemName);
                    if (child != null)
                    {
                        throw new ParameterErrorException("Системное имя должно быть уникальным");
                    }

                    // родителу можно добавить нового ребенка
                    var newReference = new CacheObjectReference(newComponent.Id, newComponent.SystemName);
                    parent.WriteChilds.Add(newReference);

                    // удлалим ссылку в старом родителе
                    if (oldComponent.ParentId.HasValue)
                    {
                        var oldParentRequest = new AccountCacheRequest()
                        {
                            AccountId = accountId,
                            ObjectId = oldComponent.ParentId.Value
                        };
                        using (var oldParent = AllCaches.Components.Write(oldParentRequest))
                        {
                            oldParent.WriteChilds.Delete(newComponent.Id);
                            oldParent.BeginSave();
                        }
                    }
                    parent.BeginSave();
                }
            }
            // если родитель НЕ изменился, но изменилось системное имя
            else if (string.Equals(oldComponent.SystemName, newComponent.SystemName, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                // проверим, что у родителя нет ребенка с таким же именем
                var parentRequest = new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = newComponent.ParentId.Value
                };
                using (var parent = AllCaches.Components.Write(parentRequest))
                {
                    // проверим, что у нового родителя нет детей с таким же именем
                    var child = parent.Childs.FindByName(newComponent.SystemName);
                    if (child != null)
                    {
                        throw new ParameterErrorException("SystemName должен быть уникальным");
                    }

                    // родителю можно добавить нового ребенка
                    parent.WriteChilds.Rename(oldComponent.SystemName, newComponent.SystemName);
                    parent.BeginSave();
                }
            }
        }

        protected override bool HasChanges(ComponentCacheWriteObject oldObj, ComponentCacheWriteObject newObj)
        {
            // изменение ссылок на детей приведет к попытке сохранения данного объекта в БД,
            // хотя возможно сохранять нечего (все изменения в детях), 
            // но это не критично, т.к. связи меняются очень редко
            // нужно вернуть true, чтобы сохранить новый объект в response
            if (oldObj.ReferencesVersion != newObj.ReferencesVersion)
            {
                return true;
            }
            return ObjectChangesHelper.HasChanges(oldObj, newObj);
        }

        protected override void AddBatchObjects(IStorage storage, ComponentCacheWriteObject[] components)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateBatchObjects(IStorage storage, ComponentCacheWriteObject[] components, bool useCheck)
        {
            var entities = components.Select(component =>
            {
                var lastData = component.Response.LastSavedData;

                if (lastData == null)
                {
                    throw new Exception("component.Response.LastSavedData == null");
                }
                var entity = lastData.CreateEf();

                if (lastData.ChildComponentsStatusId != component.ChildComponentsStatusId)
                    entity.ChildComponentsStatusId.Set(component.ChildComponentsStatusId);

                if (lastData.ComponentTypeId != component.ComponentTypeId)
                    entity.ComponentTypeId.Set(component.ComponentTypeId);

                if (lastData.DisableComment != component.DisableComment)
                    entity.DisableComment.Set(component.DisableComment);

                if (lastData.DisableToDate != component.DisableToDate)
                    entity.DisableToDate.Set(component.DisableToDate);

                if (lastData.DisplayName != component.DisplayName)
                    entity.DisplayName.Set(component.DisplayName);

                if (lastData.Enable != component.Enable)
                    entity.Enable.Set(component.Enable);

                if (lastData.EventsStatusId != component.EventsStatusId)
                    entity.EventsStatusId.Set(component.EventsStatusId);

                if (lastData.ExternalStatusId != component.ExternalStatusId)
                    entity.ExternalStatusId.Set(component.ExternalStatusId);

                if (lastData.InternalStatusId != component.InternalStatusId)
                    entity.InternalStatusId.Set(component.InternalStatusId);

                if (lastData.MetricsStatusId != component.MetricsStatusId)
                    entity.MetricsStatusId.Set(component.MetricsStatusId);

                if (lastData.IsDeleted != component.IsDeleted)
                    entity.IsDeleted.Set(component.IsDeleted);

                if (lastData.ParentEnable != component.ParentEnable)
                    entity.ParentEnable.Set(component.ParentEnable);

                if (lastData.ParentId != component.ParentId)
                    entity.ParentId.Set(component.ParentId);

                if (lastData.SystemName != component.SystemName)
                    entity.SystemName.Set(component.SystemName);

                if (lastData.UnitTestsStatusId != component.UnitTestsStatusId)
                    entity.UnitTestsStatusId.Set(component.UnitTestsStatusId);

                if (lastData.Version != component.Version)
                    entity.Version.Set(component.Version);

                return entity;
            }).ToArray();

            storage.Components.Update(entities);
        }

        public override int BatchCount
        {
            get { return 100; }
        }

        protected override ComponentCacheWriteObject LoadObject(AccountCacheRequest request, IStorage storage)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (storage == null)
            {
                throw new ArgumentNullException("storage");
            }
            var component = storage.Components.GetOneOrNullById(request.ObjectId);
            return ComponentCacheWriteObject.Create(component, request.AccountId, storage);
        }
    }
}
