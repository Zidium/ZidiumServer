using System;
using Zidium.Core.Common;

namespace Zidium.Core.Caching
{
    public interface ICacheStorage
    {
        int GetChangedCount();

        DateTime GetLastSaveChangesDate();

        int Count { get; }

        long AddCacheCount { get; }

        long AddDataBaseCount { get; }

        long UpdateCacheCount { get; }

        long UpdateDataBaseCount { get; }

        long GetSize();

        ExceptionTempInfo LastSaveException{ get; }

        long Generation { get; }

        int SaveChanges();

        /// <summary>
        /// Максимальное количество объектов в кэше
        /// </summary>
        int MaxCount { get; }

        /// <summary>
        /// Количество при котором начинается выгрузка данных
        /// </summary>
        int BeginUnloadCount { get; set; }

        /// <summary>
        /// Количество при котором заканчивается выгрузка данных
        /// </summary>
        int StopUnloadCount { get; set; }
    }
}
