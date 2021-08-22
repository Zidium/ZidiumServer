using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class MetricHistoryRepository : IMetricHistoryRepository
    {
        public MetricHistoryRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(MetricHistoryForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.MetricHistories.Add(new DbMetricHistory()
                {
                    Id = entity.Id,
                    Value = entity.Value,
                    MetricTypeId = entity.MetricTypeId,
                    ComponentId = entity.ComponentId,
                    ActualDate = entity.ActualDate,
                    BeginDate = entity.BeginDate,
                    Color = entity.Color,
                    HasSignal = entity.HasSignal,
                    StatusEventId = entity.StatusEventId
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public MetricHistoryForRead[] GetByPeriod(
            Guid componentId,
            DateTime? startDate,
            DateTime? endDate,
            Guid[] metricTypes,
            int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.MetricHistories.AsNoTracking()
                    .Where(t => t.ComponentId == componentId);

                if (metricTypes != null && metricTypes.Length > 0)
                {
                    query = query.Where(t => metricTypes.Contains(t.MetricTypeId));
                }

                if (startDate.HasValue)
                    query = query.Where(t => t.BeginDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(t => t.BeginDate <= endDate.Value);

                return query
                    .OrderByDescending(t => t.BeginDate)
                    .Take(maxCount)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public MetricHistoryForRead GetFirstByStatusEventId(Guid statusEventId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.MetricHistories.AsNoTracking()
                    .Where(x => x.StatusEventId == statusEventId)
                    .OrderBy(x => x.BeginDate).FirstOrDefault());
            }
        }

        public int DeleteMetricsHistory(int maxCount, DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var objectQuery = contextWrapper.Context.MetricHistories
                    .Where(t => t.BeginDate < toDate).OrderBy(t => t.BeginDate).Select(t => t.Id).Take(maxCount).ToParametrizedSql();

                DbConnection connection = null;
                try
                {
                    connection = contextWrapper.Context.CreateConnection();

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = contextWrapper.Context.Database.CurrentTransaction?.GetDbTransaction();
                        command.CommandTimeout = 0;

                        var query = $"DELETE FROM {contextWrapper.Context.FormatTableName("MetricHistory")} WHERE {contextWrapper.Context.FormatColumnName("Id")} IN ({objectQuery.Sql})";

                        command.CommandText = query;

                        foreach (var objectParameter in objectQuery.Parameters)
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = objectParameter.ParameterName;
                            parameter.Value = objectParameter.Value;
                            command.Parameters.Add(parameter);
                        }

                        return SqlCommandHelper.ExecuteNonQuery(command);
                    }
                }
                finally
                {
                    contextWrapper.Context.ReleaseConnection(connection);
                }
            }
        }

        public MetricHistoryForRead[] GetLast(Guid componentId, Guid metricTypeId, int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.MetricHistories.AsNoTracking()
                    .Where(t => t.ComponentId == componentId &&
                                t.MetricTypeId == metricTypeId)
                    .OrderByDescending(t => t.BeginDate)
                    .Take(maxCount)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbMetricHistory DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.MetricHistories.Find(id);
            }
        }

        private DbMetricHistory DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"История метрики {id} не найдена");

            return result;
        }

        private MetricHistoryForRead DbToEntity(DbMetricHistory entity)
        {
            if (entity == null)
                return null;

            return new MetricHistoryForRead(entity.Id, entity.ComponentId, entity.MetricTypeId, entity.BeginDate,
                entity.ActualDate, entity.Value, entity.Color, entity.StatusEventId, entity.HasSignal);
        }
    }
}
