using System;
using System.Linq;
using Zidium.Api.Others;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public class UnitTestCacheStorage : AccountDbCacheStorageBase<UnitTestCacheResponse, IUnitTestCacheReadObject, UnitTestCacheWriteObject>
    {
        protected override UnitTestCacheWriteObject LoadObject(AccountCacheRequest request, IStorage storage)
        {
            var data = storage.UnitTests.GetOneOrNullById(request.ObjectId);
            return UnitTestCacheWriteObject.Create(data, request.AccountId);
        }

        protected override void AddBatchObjects(IStorage storage, UnitTestCacheWriteObject[] writeObjects)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateBatchObjects(IStorage storage, UnitTestCacheWriteObject[] unitTests, bool useCheck)
        {
            var entities = unitTests.Select(unitTest =>
            {
                var lastData = unitTest.Response.LastSavedData;
                if (lastData == null)
                {
                    throw new Exception("unitTest.Response.LastSavedData == null");
                }
                var entity = lastData.CreateEf();

                if (lastData.ComponentId != unitTest.ComponentId)
                    entity.ComponentId.Set(unitTest.ComponentId);

                if (lastData.DisplayName != unitTest.DisplayName)
                    entity.DisplayName.Set(unitTest.DisplayName);

                if (lastData.Enable != unitTest.Enable)
                    entity.Enable.Set(unitTest.Enable);

                if (lastData.DisableComment != unitTest.DisableComment)
                    entity.DisableComment.Set(unitTest.DisableComment);

                if (lastData.DisableToDate != unitTest.DisableToDate)
                    entity.DisableToDate.Set(unitTest.DisableToDate);

                if (lastData.ErrorColor != unitTest.ErrorColor)
                    entity.ErrorColor.Set(unitTest.ErrorColor);

                if (lastData.IsDeleted != unitTest.IsDeleted)
                    entity.IsDeleted.Set(unitTest.IsDeleted);

                if (lastData.NextExecutionDate != unitTest.NextExecutionDate)
                    entity.NextExecutionDate.Set(unitTest.NextExecutionDate);

                if (lastData.ParentEnable != unitTest.ParentEnable)
                    entity.ParentEnable.Set(unitTest.ParentEnable);

                if (lastData.PeriodSeconds != unitTest.PeriodSeconds)
                    entity.PeriodSeconds.Set(unitTest.PeriodSeconds);

                if (lastData.SimpleMode != unitTest.SimpleMode)
                    entity.SimpleMode.Set(unitTest.SimpleMode);

                if (lastData.StatusDataId != unitTest.StatusDataId)
                    entity.StatusDataId.Set(unitTest.StatusDataId);

                if (lastData.SystemName != unitTest.SystemName)
                    entity.SystemName.Set(unitTest.SystemName);

                if (lastData.NoSignalColor != unitTest.NoSignalColor)
                    entity.NoSignalColor.Set(unitTest.NoSignalColor);

                if (lastData.ActualTime != unitTest.ActualTime)
                    entity.ActualTimeSecs.Set(TimeSpanHelper.GetSeconds(unitTest.ActualTime));

                if (lastData.LastExecutionDate != unitTest.LastExecutionDate)
                    entity.LastExecutionDate.Set(unitTest.LastExecutionDate);

                if (lastData.AttempCount != unitTest.AttempCount)
                    entity.AttempCount.Set(unitTest.AttempCount);

                if (lastData.AttempMax != unitTest.AttempMax)
                    entity.AttempMax.Set(unitTest.AttempMax);

                return entity;
            }).ToArray();

            storage.UnitTests.Update(entities);
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
