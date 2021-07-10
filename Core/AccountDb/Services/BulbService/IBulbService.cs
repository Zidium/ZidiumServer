using System;
using Zidium.Api.Dto;
using Zidium.Core.Caching;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface IBulbService
    {
        IBulbCacheReadObject GetRaw(Guid statusId);

        IBulbCacheReadObject SetSignal(Guid bulbId, BulbSignal signal);

        void CalculateByChilds(Guid statusId, Guid[] childs);

        /// <summary>
        /// Создает колбаску статуса без привязки к владельцу (чтобы не было циклической зависимости внешнего ключа)
        /// </summary>
        /// <param name="createDate"></param>
        /// <param name="eventCategory"></param>
        /// <param name="ownerId"></param>
        /// <param name="statusMessage"></param>
        /// 
        /// <returns></returns>
        Guid CreateBulb(DateTime createDate,
            EventCategory eventCategory,
            Guid ownerId,
            string statusMessage);

        BulbForRead GetParent(BulbForRead bulb);

        BulbForRead[] GetChilds(BulbForRead bulb);

        Guid GetComponentId(BulbForRead bulb);

    }
}
