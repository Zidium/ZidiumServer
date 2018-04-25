using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Caching
{
    public class MetricTypeCacheWriteObject : CacheWriteObjectBase<AccountCacheRequest, MetricTypeCacheResponse, IMetricTypeCacheReadObject, MetricTypeCacheWriteObject>, IMetricTypeCacheReadObject
    {
        /// <summary>
        /// Системное название метрики
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Отображаемое имя метрики
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Ссылка на аккаунт
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; set; }

        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Текст правила для красного цвета
        /// </summary>
        public string ConditionRed { get; set; }

        /// <summary>
        /// Цвет метрики, если нет сигнала
        /// </summary>
        public ObjectColor? NoSignalColor { get; set; }

        public TimeSpan? ActualTime { get; set; }

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

        public static MetricTypeCacheWriteObject Create(MetricType metricType, Guid accountId)
        {
            if (metricType == null)
            {
                return null;
            }
            var cache = new MetricTypeCacheWriteObject()
            {
                AccountId = accountId,
                Id = metricType.Id,
                IsDeleted = metricType.IsDeleted,
                CreateDate = metricType.CreateDate,
                SystemName = metricType.SystemName,
                DisplayName = metricType.DisplayName,
                NoSignalColor = metricType.NoSignalColor,
                ActualTime = TimeSpanHelper.FromSeconds(metricType.ActualTimeSecs),
                ConditionRed = metricType.ConditionAlarm,
                ConditionYellow = metricType.ConditionWarning,
                ConditionGreen = metricType.ConditionSuccess,
                ElseColor = metricType.ConditionElseColor
            };
            return cache;
        }

        public override int GetCacheSize()
        {
            return 200;
        }

        public MetricType CreateEf()
        {
            return new MetricType()
            {
                Id = Id,
                IsDeleted = IsDeleted,
                CreateDate = CreateDate,
                SystemName = SystemName,
                DisplayName = DisplayName,
                ActualTimeSecs = TimeSpanHelper.GetSeconds(ActualTime),
                NoSignalColor = NoSignalColor,
                ConditionAlarm = ConditionRed,
                ConditionWarning = ConditionYellow,
                ConditionSuccess = ConditionGreen,
                ConditionElseColor = ElseColor
            };
        }
    }
}
