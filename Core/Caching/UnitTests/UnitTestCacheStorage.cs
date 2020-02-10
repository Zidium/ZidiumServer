using System;
using Zidium.Api.Others;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Caching
{
    public class UnitTestCacheStorage : AccountDbCacheStorageBase<UnitTestCacheResponse, IUnitTestCacheReadObject, UnitTestCacheWriteObject>
    {
        protected override UnitTestCacheWriteObject LoadObject(AccountCacheRequest request, AccountDbContext accountDbContext)
        {
            var repository = accountDbContext.GetUnitTestRepository();
            var data = repository.GetByIdOrNull(request.ObjectId);
            return UnitTestCacheWriteObject.Create(data, request.AccountId);
        }

        protected override void AddBatchObject(AccountDbContext accountDbContext, UnitTestCacheWriteObject writeObject)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateBatchObject(AccountDbContext accountDbContext, UnitTestCacheWriteObject unitTest, bool useCheck)
        {
            if (unitTest.Response.LastSavedData == null)
            {
                throw new Exception("unitTest.Response.LastSavedData == null");
            }
            var dbEntity = unitTest.Response.LastSavedData.CreateEf();
            accountDbContext.UnitTests.Attach(dbEntity);

            dbEntity.IsDeleted = unitTest.IsDeleted;
            dbEntity.ComponentId = unitTest.ComponentId;
            dbEntity.CreateDate = unitTest.CreateDate;
            dbEntity.DisplayName = unitTest.DisplayName;
            dbEntity.Enable = unitTest.Enable;
            dbEntity.DisableComment = unitTest.DisableComment;
            dbEntity.DisableToDate = unitTest.DisableToDate;
            dbEntity.ErrorColor = unitTest.ErrorColor;
            dbEntity.IsDeleted = unitTest.IsDeleted;
            dbEntity.NextExecutionDate = unitTest.NextExecutionDate;
            dbEntity.ParentEnable = unitTest.ParentEnable;
            dbEntity.PeriodSeconds = unitTest.PeriodSeconds;
            dbEntity.SimpleMode = unitTest.SimpleMode;
            dbEntity.StatusDataId = unitTest.StatusDataId;
            dbEntity.SystemName = unitTest.SystemName;
            dbEntity.NoSignalColor = unitTest.NoSignalColor;
            dbEntity.ActualTimeSecs = TimeSpanHelper.GetSeconds(unitTest.ActualTime);
            dbEntity.LastExecutionDate = unitTest.LastExecutionDate;
            dbEntity.AttempCount = unitTest.AttempCount;
            dbEntity.AttempMax = unitTest.AttempMax;
        }

        public override int BatchCount
        {
            get { return 1; }
        }

        protected override Exception CreateNotFoundException(AccountCacheRequest request)
        {
            return new UnknownUnitTestIdException(request.ObjectId);
        }

        protected override void ValidateChanges(UnitTestCacheWriteObject oldObj, UnitTestCacheWriteObject newObj)
        {
            if (newObj.AccountId == Guid.Empty)
            {
                throw new Exception("unitTest.AccountId == Guid.Empty");
            }
            if (newObj.ComponentId == Guid.Empty)
            {
                throw new Exception("unitTest.ComponentId == Guid.Empty");
            }
            if (newObj.CreateDate == DateTime.MinValue)
            {
                throw new Exception("unitTest.CreateDate == DateTime.MinValue");
            }
            if (newObj.StatusDataId == Guid.Empty)
            {
                throw new Exception("unitTest.StatusDataId == Guid.Empty");
            }

            if (oldObj.AccountId != newObj.AccountId)
            {
                throw new Exception("oldObj.AccountId != newObj.AccountId");
            }

            if (StringHelper.GetLength(newObj.DisplayName) > 255)
            {
                throw new Exception("StringHelper.GetLength(newObj.DisplayName) > 255");
            }
            if (StringHelper.GetLength(newObj.SystemName) > 255)
            {
                throw new Exception("StringHelper.GetLength(newObj.SystemName) > 255");
            }
            if (StringHelper.GetLength(newObj.DisableComment) > 1000)
            {
                throw new Exception("StringHelper.GetLength(newObj.DisableComment) > 1000");
            }

            var newComponentRequest = new AccountCacheRequest()
            {
                AccountId = newObj.AccountId,
                ObjectId = newObj.ComponentId
            };

            // если изменился родитель
            if (oldObj.ComponentId != newObj.ComponentId)
            {
                // получим нового родителя
                using (var newComponent = AllCaches.Components.Write(newComponentRequest))
                {
                    // проверим, что у нового родителя нет ребенка с таким же именем
                    var child = newComponent.UnitTests.FindByName(newObj.SystemName);
                    if (child != null)
                    {
                        throw new Exception("У нового компонента уже есть проверка с таким именем");
                    }

                    // получим старого родителя
                    var oldComponentRequest = new AccountCacheRequest()
                    {
                        AccountId = newObj.AccountId,
                        ObjectId = oldObj.ComponentId
                    };
                    using (var oldComponent = AllCaches.Components.Write(oldComponentRequest))
                    {
                        var reference = new CacheObjectReference(newObj.Id, newObj.SystemName);
                        newComponent.WriteUnitTests.Add(reference);
                        oldComponent.WriteUnitTests.Delete(newObj.Id);
                        oldComponent.BeginSave();
                    }
                    newComponent.BeginSave();
                }
            }
            // если изменилось только системное имя
            else if (string.Equals(oldObj.SystemName, newObj.SystemName, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                // проверим что у компонента нет проверки с таким SystemName
                using (var component = AllCaches.Components.Write(newComponentRequest))
                {
                    var child = component.UnitTests.FindByName(newObj.SystemName);
                    if (child != null)
                    {
                        throw new Exception("У компонента уже есть проверка с таким SystemName");
                    }
                    component.WriteUnitTests.Rename(oldObj.SystemName, newObj.SystemName);
                    component.BeginSave();
                }
            }
        }

        protected override bool HasChanges(UnitTestCacheWriteObject oldObj, UnitTestCacheWriteObject newObj)
        {
            return ObjectChangesHelper.HasChanges(oldObj, newObj);
        }
    }
}
