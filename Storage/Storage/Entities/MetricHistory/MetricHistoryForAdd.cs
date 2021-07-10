using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class MetricHistoryForAdd
    {
        public Guid Id;

        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        public Guid ComponentId;

        /// <summary>
        /// Ссылка на тип метрики
        /// </summary>
        public Guid MetricTypeId;

        /// <summary>
        /// Дата и время начала
        /// </summary>
        public DateTime BeginDate;

        /// <summary>
        /// Дата и время актуальности
        /// </summary>
        public DateTime ActualDate;

        /// <summary>
        /// Значение метрики (число)
        /// </summary>
        public double? Value;

        /// <summary>
        /// Цвет
        /// </summary>
        public ObjectColor Color;

        public Guid? StatusEventId;

        public bool HasSignal;
    }
}
