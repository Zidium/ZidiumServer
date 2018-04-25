using System;
using Zidium.Core.Common;

namespace Zidium.Core.Caching
{
    public interface IMetricTypeCacheReadObject : IAccountDbCacheReadObject
    {
        /// <summary>
        /// Название метрики
        /// </summary>
        string SystemName { get; }
        
        /// <summary>
        /// Признак удалённого
        /// </summary>
        bool IsDeleted { get; }

        DateTime? CreateDate { get; }

        /// <summary>
        /// Текст правила для красного цвета
        /// </summary>
        string ConditionRed { get; }

        /// <summary>
        /// Текст правила для жёлтого цвета
        /// </summary>
        string ConditionYellow { get; }

        /// <summary>
        /// Текст правила для зелёного цвета
        /// </summary>
        string ConditionGreen { get; }

        /// <summary>
        /// Цвет, если ни одно правило не выполнилось
        /// </summary>
        ObjectColor? ElseColor { get; }

        ObjectColor? NoSignalColor { get; }

        TimeSpan? ActualTime { get; }
    }
}
