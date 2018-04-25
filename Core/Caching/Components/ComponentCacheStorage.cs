using System;
using Zidium.Api.Others;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

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

        protected override void AddBatchObject(AccountDbContext accountDbContext, ComponentCacheWriteObject component)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateBatchObject(AccountDbContext accountDbContext, ComponentCacheWriteObject component, bool useCheck)
        {
            if (component.Response.LastSavedData == null)
            {
                throw new Exception("component.Response.LastSavedData == null");
            }
            var dbEntity = component.Response.LastSavedData.CreateEf();
            accountDbContext.Components.Attach(dbEntity);

            dbEntity.ChildComponentsStatusId = component.ChildComponentsStatusId;
            dbEntity.ComponentTypeId = component.ComponentTypeId;
            dbEntity.CreatedDate = component.CreatedDate;
            dbEntity.DisableComment = component.DisableComment;
            dbEntity.DisableToDate = component.DisableToDate;
            dbEntity.DisplayName = component.DisplayName;
            dbEntity.Enable = component.Enable;
            dbEntity.EventsStatusId = component.EventsStatusId;
            dbEntity.ExternalStatusId = component.ExternalStatusId;
            dbEntity.InternalStatusId = component.InternalStatusId;
            dbEntity.IsDeleted = component.IsDeleted;
            dbEntity.ParentEnable = component.ParentEnable;
            dbEntity.ParentId = component.ParentId;
            dbEntity.SystemName = component.SystemName;
            dbEntity.UnitTestsStatusId = component.UnitTestsStatusId;
            dbEntity.Version = component.Version;
        }

        public override int BatchCount
        {
            get { return 100; }
        }

        protected override ComponentCacheWriteObject LoadObject(AccountCacheRequest request, AccountDbContext accountDbContext)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (accountDbContext == null)
            {
                throw new ArgumentNullException("accountDbContext");
            }
            var repository = accountDbContext.GetComponentRepository();
            var component = repository.GetByIdOrNull(request.ObjectId);
            return ComponentCacheWriteObject.Create(component, request.AccountId);
        }
    }
}
