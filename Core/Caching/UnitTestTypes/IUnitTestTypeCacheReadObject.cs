using System;
using Zidium.Core.Common;

namespace Zidium.Core.Caching
{
    public interface IUnitTestTypeCacheReadObject : IAccountDbCacheReadObject
    {
        /// <summary>
        /// Системное имя
        /// </summary>
        string SystemName { get; set; }

        /// <summary>
        /// Дружелюбное имя
        /// </summary>
        string DisplayName { get; set; }

        DateTime? CreateDate { get; set; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Признак того, что тип системный (НЕ создан пользователем)
        /// </summary>
        bool IsSystem { get; set; }

        /// <summary>
        /// Цвет проверки, если нет сигнала
        /// </summary>
        ObjectColor? NoSignalColor { get; set; }

        /// <summary>
        /// Время актуальности проверки
        /// </summary>
        int? ActualTimeSecs { get; set; }

    }
}
