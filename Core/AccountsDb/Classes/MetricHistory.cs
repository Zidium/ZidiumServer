using System;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Данные метрики
    /// </summary>
    public class MetricHistory
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        public Guid ComponentId { get; set; }

        public virtual Component Component { get; set; }

        /// <summary>
        /// Ссылка на метрику
        /// </summary>
        public Guid MetricTypeId { get; set; }

        public virtual MetricType MetricType { get; set; }

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

        public virtual Event StatusEvent { get; set; }
    }
}
