using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Метрика компонента
    /// </summary>
    public class MetricForRead
    {
        public MetricForRead(
            Guid id, 
            Guid componentId, 
            Guid metricTypeId, 
            bool isDeleted, 
            DateTime? disableToDate, 
            string disableComment, 
            bool enable, 
            bool parentEnable, 
            DateTime? createDate, 
            double? value, 
            DateTime beginDate, 
            DateTime actualDate, 
            int? actualTimeSecs, 
            ObjectColor? noSignalColor, 
            string conditionAlarm, 
            string conditionWarning, 
            string conditionSuccess, 
            ObjectColor? conditionElseColor, 
            Guid statusDataId)
        {
            Id = id;
            ComponentId = componentId;
            MetricTypeId = metricTypeId;
            IsDeleted = isDeleted;
            DisableToDate = disableToDate;
            DisableComment = disableComment;
            Enable = enable;
            ParentEnable = parentEnable;
            CreateDate = createDate;
            Value = value;
            BeginDate = beginDate;
            ActualDate = actualDate;
            ActualTimeSecs = actualTimeSecs;
            NoSignalColor = noSignalColor;
            ConditionAlarm = conditionAlarm;
            ConditionWarning = conditionWarning;
            ConditionSuccess = conditionSuccess;
            ConditionElseColor = conditionElseColor;
            StatusDataId = statusDataId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        public Guid ComponentId { get; }

        /// <summary>
        /// Ссылка на тип метрики
        /// </summary>
        public Guid MetricTypeId { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; }

        /// <summary>
        /// Дата до которой выключен компонент
        /// </summary>
        public DateTime? DisableToDate { get; }

        /// <summary>
        /// Комментарий к компоненту (указывается при выключении)
        /// </summary>
        public string DisableComment { get; }

        /// <summary>
        /// Признак что компонент включен
        /// </summary>
        public bool Enable { get; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public bool ParentEnable { get; }

        public DateTime? CreateDate { get; }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public double? Value { get; }

        public DateTime BeginDate { get; }

        public DateTime ActualDate { get; }

        public int? ActualTimeSecs { get; }

        public ObjectColor? NoSignalColor { get; }

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

        public Guid StatusDataId { get; }

        public MetricForUpdate GetForUpdate()
        {
            return new MetricForUpdate(Id);
        }

    }
}
