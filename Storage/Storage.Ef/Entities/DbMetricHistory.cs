using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Данные метрики
    /// </summary>
    public class DbMetricHistory
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        public Guid ComponentId { get; set; }

        public virtual DbComponent Component { get; set; }

        /// <summary>
        /// Ссылка на метрику
        /// </summary>
        public Guid MetricTypeId { get; set; }

        public virtual DbMetricType MetricType { get; set; }

        /// <summary>
        /// Дата и время начала
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// Дата и время актуальности
        /// </summary>
        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Значение метрики (число)
        /// </summary>
        public double? Value { get; set; }

        /// <summary>
        /// Цвет
        /// </summary>
        public ObjectColor Color { get; set; }

        public Guid? StatusEventId { get; set; }

        public bool HasSignal { get; set; }

        public virtual DbEvent StatusEvent { get; set; }

    }
}
