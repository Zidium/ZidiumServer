using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Тип метрики
    /// </summary>
    public class MetricTypeForUpdate
    {
        public MetricTypeForUpdate(Guid id)
        {
            Id = id;
            SystemName = new ChangeTracker<string>();
            DisplayName = new ChangeTracker<string>();
            IsDeleted = new ChangeTracker<bool>();
            ConditionAlarm = new ChangeTracker<string>();
            ConditionWarning = new ChangeTracker<string>();
            ConditionSuccess = new ChangeTracker<string>();
            ConditionElseColor = new ChangeTracker<ObjectColor?>();
            NoSignalColor = new ChangeTracker<ObjectColor?>();
            ActualTimeSecs = new ChangeTracker<int?>();
        }

        public Guid Id { get; }

        /// <summary>
        /// Название метрики
        /// </summary>
        public ChangeTracker<string> SystemName { get; }

        public ChangeTracker<string> DisplayName { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public ChangeTracker<bool> IsDeleted { get; }

        /// <summary>
        /// Текст правила для красного цвета
        /// </summary>
        public ChangeTracker<string> ConditionAlarm { get; }

        /// <summary>
        /// Текст правила для жёлтого цвета
        /// </summary>
        public ChangeTracker<string> ConditionWarning { get; }

        /// <summary>
        /// Текст правила для зелёного цвета
        /// </summary>
        public ChangeTracker<string> ConditionSuccess { get; }

        /// <summary>
        /// Цвет, если ни одно правило не выполнилось
        /// </summary>
        public ChangeTracker<ObjectColor?> ConditionElseColor { get; }

        /// <summary>
        /// Цвет, если нет актуальных значений
        /// </summary>
        public ChangeTracker<ObjectColor?> NoSignalColor { get; }

        public ChangeTracker<int?> ActualTimeSecs { get; }

    }
}