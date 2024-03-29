﻿using System;
using Zidium.Api.Dto;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Метрика компонента
    /// </summary>
    public class DbMetric
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на компоненты
        /// </summary>
        public Guid ComponentId { get; set; }

        /// <summary>
        /// Компонент
        /// </summary>
        public virtual DbComponent Component { get; set; }

        /// <summary>
        /// Ссылка на тип метрики
        /// </summary>
        public Guid MetricTypeId { get; set; }

        /// <summary>
        /// Тип метрики
        /// </summary>
        public virtual DbMetricType MetricType { get; set; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Дата до которой выключен компонент
        /// </summary>
        public DateTime? DisableToDate { get; set; }

        /// <summary>
        /// Комментарий к компоненту (указывается при выключении)
        /// </summary>
        public string DisableComment { get; set; }

        /// <summary>
        /// Признак что компонент включен
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public bool ParentEnable { get; set; }

        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public double? Value { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime ActualDate { get; set; }

        public int? ActualTimeSecs { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

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

        public Guid StatusDataId { get; set; }

        public virtual DbBulb Bulb { get; set; }

    }
}
