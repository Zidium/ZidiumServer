using System;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public interface IUnitTestTypeCacheReadObject : IAccountDbCacheReadObject
    {
        /// <summary>
        /// Системное имя
        /// </summary>
        string SystemName { get; }

        /// <summary>
        /// Дружелюбное имя
        /// </summary>
        string DisplayName { get; }

        DateTime? CreateDate { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        bool IsDeleted { get; }

        /// <summary>
        /// Признак того, что тип системный (НЕ создан пользователем)
        /// </summary>
        bool IsSystem { get; }

        /// <summary>
        /// Цвет проверки, если нет сигнала
        /// </summary>
        ObjectColor? NoSignalColor { get; }

        /// <summary>
        /// Время актуальности проверки
        /// </summary>
        int? ActualTimeSecs { get; }

    }
}
