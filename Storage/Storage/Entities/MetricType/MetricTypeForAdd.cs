using System;

namespace Zidium.Storage
{
    public class MetricTypeForAdd
    {
        public Guid Id;

        /// <summary>
        /// Название метрики
        /// </summary>
        public string SystemName;

        public string DisplayName;

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted;

        public DateTime? CreateDate;

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

        /// <summary>
        /// Цвет, если нет актуальных значений
        /// </summary>
        public ObjectColor? NoSignalColor;

        public int? ActualTimeSecs;
    }
}
