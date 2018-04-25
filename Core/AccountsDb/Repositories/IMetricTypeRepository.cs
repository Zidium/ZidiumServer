using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Репозиторий для работы с метриками
    /// </summary>
    public interface IMetricTypeRepository
    {
        /// <summary>
        /// Получение метрики по имени
        /// </summary>
        MetricType GetOneOrNullByName(string name);

        MetricType GetById(Guid id);

        void Remove(MetricType entity);

        void Remove(Guid id);

        IQueryable<MetricType> QueryAll();

        MetricType GetByIdOrNull(Guid id);

        MetricType Add(MetricType entity);
    }
}
