using System;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public interface IMetricCacheReadObject: IAccountDbCacheReadObject
    {
        /// <summary>
        /// Ссылка на компоненты
        /// </summary>
        Guid ComponentId { get; }

        /// <summary>
        /// Ссылка на тип метрики
        /// </summary>
        Guid MetricTypeId { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        bool IsDeleted { get; }

        /// <summary>
        /// Признак что компонент включен
        /// </summary>
        bool Enable { get; }

        string DisableComment { get; set; }

        DateTime? DisableToDate { get; set; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        bool ParentEnable { get; }

        /// <summary>
        /// Компонент и его родитель включены
        /// </summary>
        bool CanProcess { get; }

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

        DateTime? CreateDate { get; }

        /// <summary>
        /// Текущее значение
        /// </summary>
        double? Value { get; }

        DateTime BeginDate { get; }

        DateTime ActualDate { get; }

        Guid StatusDataId { get; }
    }
}
