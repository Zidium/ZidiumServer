using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Caching
{
    public class MetricCacheWriteObject : CacheWriteObjectBase<AccountCacheRequest, MetricCacheResponse, IMetricCacheReadObject, MetricCacheWriteObject>, IMetricCacheReadObject    
    {
        public Guid AccountId { get; set; }

        public override int GetCacheSize()
        {
            return 1;
        }

        /// <summary>
        /// Ссылка на компоненты
        /// </summary>
        public Guid ComponentId { get; set; }

        /// <summary>
        /// Ссылка на тип метрики
        /// </summary>
        public Guid MetricTypeId { get; set; }
        
        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Признак что компонент включен
        /// </summary>
        public bool Enable { get; set; }

        public DateTime? DisableToDate { get; set; }

        public string DisableComment { get; set; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public bool ParentEnable { get; set; }

        /// <summary>
        /// Компонент и его родитель включены
        /// </summary>
        public bool CanProcess
        {
            get { return Enable && ParentEnable; }
        }

        /// <summary>
        /// Текст правила для красного цвета
        /// </summary>
        public string ConditionRed { get; set; }

        /// <summary>
        /// Текст правила для жёлтого цвета
        /// </summary>
        public string ConditionYellow { get; set; }

        /// <summary>
        /// Текст правила для зелёного цвета
        /// </summary>
        public string ConditionGreen { get; set; }

        /// <summary>
        /// Цвет, если ни одно правило не выполнилось
        /// </summary>
        public ObjectColor? ElseColor { get; set; }

        public ObjectColor? NoSignalColor { get; set; }


        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public double? Value { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime ActualDate { get; set; }

        public TimeSpan? ActualTime { get; set; }

        public Guid StatusDataId { get; set; }

        public Metric CreateEf()
        {
            return new Metric()
            {
                Id = Id,
                Enable = Enable,
                DisableComment = DisableComment,
                DisableToDate = DisableToDate,
                IsDeleted = IsDeleted,
                ParentEnable = ParentEnable,
                CreateDate = CreateDate,
                Value = Value,
                BeginDate = BeginDate,
                ComponentId = ComponentId,
                ActualDate = ActualDate,
                MetricTypeId = MetricTypeId,
                StatusDataId = StatusDataId,
                NoSignalColor = NoSignalColor,
                ConditionAlarm = ConditionRed,
                ConditionWarning = ConditionYellow,
                ConditionSuccess = ConditionGreen,
                ConditionElseColor = ElseColor,
                ActualTimeSecs = TimeSpanHelper.GetSeconds(ActualTime)
            };
        }

        public static MetricCacheWriteObject Create(Metric metric, Guid accountId)
        {
            if (metric == null)
            {
                return null;
            }
            var cache = new MetricCacheWriteObject()
            {
                Id = metric.Id,
                Enable = metric.Enable,
                DisableToDate = metric.DisableToDate,
                DisableComment = metric.DisableComment,
                StatusDataId = metric.StatusDataId,
                Value = metric.Value,
                ParentEnable = metric.ParentEnable,
                ComponentId = metric.ComponentId,
                AccountId = accountId,
                BeginDate = metric.BeginDate,
                ActualDate = metric.ActualDate,
                IsDeleted = metric.IsDeleted,
                CreateDate = metric.CreateDate,
                MetricTypeId = metric.MetricTypeId,
                ActualTime = TimeSpanHelper.FromSeconds(metric.ActualTimeSecs),
                ConditionRed = metric.ConditionAlarm,
                ConditionYellow = metric.ConditionWarning,
                ConditionGreen = metric.ConditionSuccess,
                ElseColor = metric.ConditionElseColor,
                NoSignalColor = metric.NoSignalColor
            };
            return cache;
        }
    }
}
