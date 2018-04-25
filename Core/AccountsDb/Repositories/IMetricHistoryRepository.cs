using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Репозиторий для работы с данными метрик
    /// </summary>
    public interface IMetricHistoryRepository
    {
        /// <summary>
        /// Добавление данных метрик
        /// </summary>
        MetricHistory Add(MetricHistory data);

        /// <summary>
        /// Получение строки данных метрики по id записи
        /// </summary>
        MetricHistory GetOneOrNullById(Guid id, Guid componentId);

        /// <summary>
        /// Получение данных метрик за период
        /// </summary>
        IQueryable<MetricHistory> GetByPeriod(Guid componentId, DateTime? startDate, DateTime? endDate, Guid[] metricTypes);

        IQueryable<MetricHistory> GetAllByStatus(Guid statusEventId);

        IQueryable<MetricHistory> QueryAllByMetricType(Guid componentId, Guid metricTypeId);

        int DeleteMetricsHistory(Guid componentId, int maxCount, DateTime toDate);
    }
}
