using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    /// <summary>
    /// Метрика компонента
    /// </summary>
    public class MetricForUpdate
    {
        public MetricForUpdate(Guid id)
        {
            Id = id;
            IsDeleted = new ChangeTracker<bool>();
            DisableToDate = new ChangeTracker<DateTime?>();
            DisableComment = new ChangeTracker<string>();
            Enable = new ChangeTracker<bool>();
            ParentEnable = new ChangeTracker<bool>();
            Value = new ChangeTracker<double?>();
            BeginDate = new ChangeTracker<DateTime>();
            ActualDate = new ChangeTracker<DateTime>();
            ActualTimeSecs = new ChangeTracker<int?>();
            NoSignalColor = new ChangeTracker<ObjectColor?>();
            ConditionAlarm = new ChangeTracker<string>();
            ConditionWarning = new ChangeTracker<string>();
            ConditionSuccess = new ChangeTracker<string>();
            ConditionElseColor = new ChangeTracker<ObjectColor?>();
            StatusDataId = new ChangeTracker<Guid>();
        }

        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public ChangeTracker<bool> IsDeleted { get; }

        /// <summary>
        /// Дата до которой выключен компонент
        /// </summary>
        public ChangeTracker<DateTime?> DisableToDate { get; }

        /// <summary>
        /// Комментарий к компоненту (указывается при выключении)
        /// </summary>
        public ChangeTracker<string> DisableComment { get; }

        /// <summary>
        /// Признак что компонент включен
        /// </summary>
        public ChangeTracker<bool> Enable { get; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public ChangeTracker<bool> ParentEnable { get; }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public ChangeTracker<double?> Value { get; }

        public ChangeTracker<DateTime> BeginDate { get; }

        public ChangeTracker<DateTime> ActualDate { get; }

        public ChangeTracker<int?> ActualTimeSecs { get; }

        public ChangeTracker<ObjectColor?> NoSignalColor { get; }

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

        public ChangeTracker<Guid> StatusDataId { get; }
        
    }
}