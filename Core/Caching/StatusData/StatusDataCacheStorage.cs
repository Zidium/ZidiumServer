using System;
using System.Linq;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Caching
{
    public class StatusDataCacheStorage : AccountDbCacheStorageBase<StatusDataCacheResponse, IBulbCacheReadObject, BulbCacheWriteObject>
    {
        protected override void AddBatchObject(AccountDbContext accountDbContext, BulbCacheWriteObject writeObject)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateBatchObject(AccountDbContext accountDbContext, BulbCacheWriteObject writeObject, bool useCheck)
        {
            if (writeObject.Response.LastSavedData == null)
            {
                throw new Exception("writeObject.Response.LastSavedData");
            }
            var dbEntity = writeObject.Response.LastSavedData.CreateEfStatusData();

            // временная проверка, чтобы понять причины ошибки при выполнении accountDbContext.StatusDatas.Attach(dbEntity);
            // Cache.StatusDatas.LastSaveException: 31.08.2016 14:48:03 System.InvalidOperationException: A referential integrity constraint violation occurred: The property value(s) of 'StatusData.Id' on one end of a relationship do not match the property value(s) of 'StatusData.LastChildStatusDataId' on the other end.
            var localDb = accountDbContext.Bulbs.Local.FirstOrDefault(x => x.Id == dbEntity.Id);
            if (localDb != null)
            {
                throw new Exception("localDb != null");
            }
            accountDbContext.Bulbs.Attach(dbEntity);
            //try
            //{
            //    accountDbContext.StatusDatas.Attach(dbEntity);
            //}
            //catch (Exception exception)
            //{
            //    bool hasChild = false;
            //    if (dbEntity.LastChildStatusDataId.HasValue)
            //    {
            //        hasChild = accountDbContext.StatusDatas.Local.FirstOrDefault(
            //                x => x.Id == dbEntity.LastChildStatusDataId.Value) != null;
            //    }
            //    var data = new
            //    {
            //        hasChild,
            //        StatusDataId = dbEntity.Id,
            //        LastChildHasValue = dbEntity.LastChildStatusDataId.HasValue,
            //        LastChildStatusDataId = dbEntity.LastChildStatusDataId,
            //    };
            //    ComponentControl.Log.Error("Ошибка Attach", exception, data);
            //    throw;
            //}
            
            dbEntity.IsDeleted = writeObject.IsDeleted;
            dbEntity.LastChildBulbId = writeObject.LastChildBulbId;
            dbEntity.LastEventId = writeObject.LastEventId;
            dbEntity.Message = writeObject.Message;
            dbEntity.PreviousStatus = writeObject.PreviousStatus;
            dbEntity.StartDate = writeObject.StartDate;
            dbEntity.Status = writeObject.Status;
            dbEntity.StatusEventId = writeObject.StatusEventId;
            dbEntity.ActualDate = writeObject.ActualDate;
            dbEntity.Count = writeObject.Count;
            dbEntity.EndDate = writeObject.EndDate;
            dbEntity.FirstEventId = writeObject.FirstEventId;
            dbEntity.HasSignal = writeObject.HasSignal;
        }

        public override int BatchCount
        {
            get { return 1; }
        }

        protected override BulbCacheWriteObject LoadObject(AccountCacheRequest request, AccountDbContext accountDbContext)
        {
            var repository = accountDbContext.GetStatusDataRepository();
            var statusData = repository.GetByIdOrNull(request.ObjectId);
            return BulbCacheWriteObject.Create(statusData, request.AccountId);
        }

        protected override Exception CreateNotFoundException(AccountCacheRequest request)
        {
            return new UserFriendlyException("Не удалось найти колбаску с ID " + request.ObjectId);
        }

        protected override void ValidateChanges(BulbCacheWriteObject oldData, BulbCacheWriteObject newData)
        {
            if (newData.AccountId == Guid.Empty)
            {
                throw new Exception("statusData.AccountId == Guid.Empty");
            }
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
