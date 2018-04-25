using System;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Тип метрики
    /// </summary>
    public class MetricType
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Название метрики
        /// </summary>
        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; set; }

        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Текст правила для красного цвета
        /// </summary>
        public string ConditionAlarm { get; set; }

        /// <summary>
        /// Текст правила для жёлтого цвета
        /// </summary>
        public string ConditionWarning { get; set; }

        /// <summary>
        /// Текст правила для зелёного цвета
        /// </summary>
        public string ConditionSuccess { get; set; }

        /// <summary>
        /// Цвет, если ни одно правило не выполнилось
        /// </summary>
        public ObjectColor? ConditionElseColor { get; set; }

        /// <summary>
        /// Цвет, если нет актуальных значений
        /// </summary>
        public ObjectColor? NoSignalColor { get; set; }

        public int? ActualTimeSecs { get; set; }
    }
}
