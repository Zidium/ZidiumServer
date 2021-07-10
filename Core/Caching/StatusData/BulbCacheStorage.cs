using System;
using System.Linq;
using Zidium.Common;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public class BulbCacheStorage : AccountDbCacheStorageBase<BulbCacheResponse, IBulbCacheReadObject, BulbCacheWriteObject>
    {
        protected override void AddBatchObjects(IStorage storage, BulbCacheWriteObject[] writeObjects)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateBatchObjects(IStorage storage, BulbCacheWriteObject[] writeObjects, bool useCheck)
        {
            var entities = writeObjects.Select(bulb =>
            {
                var lastData = bulb.Response.LastSavedData;

                if (lastData == null)
                {
                    throw new Exception("writeObject.Response.LastSavedData");
                }
                var entity = lastData.CreateEfBulb();

                if (lastData.IsDeleted != bulb.IsDeleted)
                    entity.IsDeleted.Set(bulb.IsDeleted);

                if (lastData.LastChildBulbId != bulb.LastChildBulbId)
                    entity.LastChildBulbId.Set(bulb.LastChildBulbId);

                if (lastData.LastEventId != bulb.LastEventId)
                    entity.LastEventId.Set(bulb.LastEventId);

                if (lastData.Message != bulb.Message)
                    entity.Message.Set(bulb.Message);

                if (lastData.PreviousStatus != bulb.PreviousStatus)
                    entity.PreviousStatus.Set(bulb.PreviousStatus);

                if (lastData.StartDate != bulb.StartDate)
                    entity.StartDate.Set(bulb.StartDate);

                if (lastData.Status != bulb.Status)
                    entity.Status.Set(bulb.Status);

                if (lastData.StatusEventId != bulb.StatusEventId)
                    entity.StatusEventId.Set(bulb.StatusEventId);

                if (lastData.ActualDate != bulb.ActualDate)
                    entity.ActualDate.Set(bulb.ActualDate);

                if (lastData.Count != bulb.Count)
                    entity.Count.Set(bulb.Count);

                if (lastData.EndDate != bulb.EndDate)
                    entity.EndDate.Set(bulb.EndDate);

                if (lastData.FirstEventId != bulb.FirstEventId)
                    entity.FirstEventId.Set(bulb.FirstEventId);

                if (lastData.HasSignal != bulb.HasSignal)
                    entity.HasSignal.Set(bulb.HasSignal);

                if (lastData.ComponentId != bulb.ComponentId)
                    entity.ComponentId.Set(bulb.ComponentId);

                if (lastData.MetricId != bulb.MetricId)
                    entity.MetricId.Set(bulb.MetricId);

                if (lastData.UnitTestId != bulb.UnitTestId)
                    entity.UnitTestId.Set(bulb.UnitTestId);

                return entity;
            }).ToArray();

            storage.Bulbs.Update(entities);
        }

        public override int BatchCount
        {
            get { return 1; }
        }

        protected override BulbCacheWriteObject LoadObject(AccountCacheRequest request, IStorage storage)
        {
            var bulb = storage.Bulbs.GetOneOrNullById(request.ObjectId);
            return BulbCacheWriteObject.Create(bulb);
        }

        protected override Exception CreateNotFoundException(AccountCacheRequest request)
        {
            return new UserFriendlyException("Не удалось найти колбаску с ID " + request.ObjectId);
        }

        protected override void ValidateChanges(BulbCacheWriteObject oldData, BulbCacheWriteObject newData)
        {
            if (newData.StartDate == DateTime.MinValue)
            {
                throw new Exception("statusData.StartDate == DateTime.MinValue");
            }
            if (newData.EndDate == DateTime.MinValue)
            {
                throw new Exception("statusData.EndDate == DateTime.MinValue");
            }
            if (newData.ActualDate == DateTime.MinValue)
            {
                throw new Exception("statusData.ActualDate == DateTime.MinValue");
            }
        }

        protected override bool HasChanges(BulbCacheWriteObject oldObj, BulbCacheWriteObject newObj)
        {
            return ObjectChangesHelper.HasChanges(oldObj, newObj);
        }
    }
}
