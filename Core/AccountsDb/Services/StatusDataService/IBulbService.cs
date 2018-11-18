using System;
using System.Collections.Generic;
using Zidium.Core.Api;
using Zidium.Core.Caching;

namespace Zidium.Core.AccountsDb
{
    public interface IBulbService
    {
        IBulbCacheReadObject GetRaw(Guid accountId, Guid statusId);

        //IBulbCacheReadObject SetUnknownStatus(Guid accountId, Guid statusId, DateTime processDate);

        IBulbCacheReadObject SetSignal(Guid bulbId, BulbSignal signal);

        //IStatusDataCacheReadObject SetIfMoreDanger(Guid statusId, StatusSignal signal);

        //IBulbCacheReadObject Disable(Guid accountId, Guid statusId);

        // IBulbCacheReadObject Enable(Guid accountId, Guid statusId);

        void CalculateByChilds(Guid accountId, Guid statusId, List<Guid> childs);

        /// <summary>
        /// Создает колбаску статуса без привязки к владельцу (чтобы не было циклической зависимости внешнего ключа)
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="createDate"></param>
        /// <param name="eventCategory"></param>
        /// <returns></returns>
        Bulb CreateBulb(
            Guid accountId,
            DateTime createDate,
            EventCategory eventCategory,
            Guid ownerId,
            string statusMessage);
    }
}
