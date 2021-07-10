using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class MetricForAdd
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Ссылка на компоненты
        /// </summary>
        public Guid ComponentId;

        /// <summary>
        /// Ссылка на тип метрики
        /// </summary>
        public Guid MetricTypeId;

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted;

        /// <summary>
        /// Дата до которой выключен компонент
        /// </summary>
        public DateTime? DisableToDate;

        /// <summary>
        /// Комментарий к компоненту (указывается при выключении)
        /// </summary>
        public string DisableComment;

        /// <summary>
        /// Признак что компонент включен
        /// </summary>
        public bool Enable;

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public bool ParentEnable;

        public DateTime? CreateDate;

        /// <summary>
        /// Текущее значение
        /// </summary>
        public double? Value;

        public DateTime BeginDate;

        public DateTime ActualDate;

        public int? ActualTimeSecs;

        public ObjectColor? NoSignalColor;

        /// <summary>
        /// Текст правила для красного цвета
        /// </summary>
        public string ConditionAlarm;

        /// <summary>
        /// Текст правила для жёлтого цвета
        /// </summary>
        public string ConditionWarning;

        /// <summary>
        /// Текст правила для зелёного цвета
        /// </summary>
        public string ConditionSuccess;

        /// <summary>
        /// Цвет, если ни одно правило не выполнилось
        /// </summary>
        public ObjectColor? ConditionElseColor;

        public Guid StatusDataId;
    }
}
