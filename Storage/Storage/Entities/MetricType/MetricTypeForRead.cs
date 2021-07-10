using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    /// <summary>
    /// Тип метрики
    /// </summary>
    public class MetricTypeForRead
    {
        public MetricTypeForRead(
            Guid id,
            string systemName,
            string displayName,
            bool isDeleted,
            DateTime? createDate,
            string conditionAlarm,
            string conditionWarning,
            string conditionSuccess,
            ObjectColor? conditionElseColor,
            ObjectColor? noSignalColor,
            int? actualTimeSecs)
        {
            Id = id;
            SystemName = systemName;
            DisplayName = displayName;
            IsDeleted = isDeleted;
            CreateDate = createDate;
            ConditionAlarm = conditionAlarm;
            ConditionWarning = conditionWarning;
            ConditionSuccess = conditionSuccess;
            ConditionElseColor = conditionElseColor;
            NoSignalColor = noSignalColor;
            ActualTimeSecs = actualTimeSecs;
        }

        public Guid Id { get; }

        /// <summary>
        /// Название метрики
        /// </summary>
        public string SystemName { get; }

        public string DisplayName { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; }

        public DateTime? CreateDate { get; }

        /// <summary>
        /// Текст правила для красного цвета
        /// </summary>
        public string ConditionAlarm { get; }

        /// <summary>
        /// Текст правила для жёлтого цвета
        /// </summary>
        public string ConditionWarning { get; }

        /// <summary>
        /// Текст правила для зелёного цвета
        /// </summary>
        public string ConditionSuccess { get; }

        /// <summary>
        /// Цвет, если ни одно правило не выполнилось
        /// </summary>
        public ObjectColor? ConditionElseColor { get; }

        /// <summary>
        /// Цвет, если нет актуальных значений
        /// </summary>
        public ObjectColor? NoSignalColor { get; }

        public int? ActualTimeSecs { get; }

        public MetricTypeForUpdate GetForUpdate()
        {
            return new MetricTypeForUpdate(Id);
        }

    }
}
