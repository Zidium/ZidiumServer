using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Данные метрики
    /// </summary>
    public class MetricHistoryForRead
    {
        public MetricHistoryForRead(
            Guid id, 
            Guid componentId, 
            Guid metricTypeId, 
            DateTime beginDate, 
            DateTime actualDate, 
            double? value, 
            ObjectColor color, 
            Guid? statusEventId, 
            bool hasSignal)
        {
            Id = id;
            ComponentId = componentId;
            MetricTypeId = metricTypeId;
            BeginDate = beginDate;
            ActualDate = actualDate;
            Value = value;
            Color = color;
            StatusEventId = statusEventId;
            HasSignal = hasSignal;
        }

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
        /// Дата и время начала
        /// </summary>
        public DateTime BeginDate { get; }

        /// <summary>
        /// Дата и время актуальности
        /// </summary>
        public DateTime ActualDate { get; }

        /// <summary>
        /// Значение метрики (число)
        /// </summary>
        public double? Value { get; }

        /// <summary>
        /// Цвет
        /// </summary>
        public ObjectColor Color { get; }

        public Guid? StatusEventId { get; }

        public bool HasSignal { get; }

    }
}
