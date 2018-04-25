using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class MetricHistoryRepository : IMetricHistoryRepository
    {
        protected AccountDbContext Context;

        public MetricHistoryRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public MetricHistory Add(MetricHistory data)
        {
            if (data.Id == Guid.Empty)
                data.Id = Guid.NewGuid();

            Context.MetricHistories.Add(data);
            return data;
        }

        public MetricHistory GetOneOrNullById(Guid id, Guid componentId)
        {
            return Context.MetricHistories.FirstOrDefault(t => t.Id == id && t.ComponentId == componentId);
        }

        public IQueryable<MetricHistory> GetByPeriod(Guid componentId, DateTime? startDate, DateTime? endDate, Guid[] metricTypes)
        {
            var query = Context.MetricHistories.Where(t => t.ComponentId == componentId);

            if (metricTypes != null && metricTypes.Length > 0)
            {
                query = query.Where(t => metricTypes.Contains(t.MetricTypeId));
            }

            if (startDate.HasValue)
                query = query.Where(t => t.BeginDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.BeginDate <= endDate.Value);

            return query;
        }

        public IQueryable<MetricHistory> GetAllByStatus(Guid statusEventId)
        {
            return Context.MetricHistories.Where(x => x.StatusEventId == statusEventId);
        }

        public IQueryable<MetricHistory> QueryAllByMetricType(Guid componentId, Guid metricTypeId)
        {
            var query = Context.MetricHistories.Where(t => t.ComponentId == componentId && t.MetricTypeId == metricTypeId);
            return query;
        }

        public int DeleteMetricsHistory(Guid componentId, int maxCount, DateTime toDate)
        {
            using (var connection = Context.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;

                    string query = string.Format(
                        "DELETE FROM [dbo].[MetricHistory] WHERE Id IN (SELECT TOP {0} Id FROM [dbo].[MetricHistory] WHERE ComponentId = @ComponentId AND ActualDate < @ActualDate ORDER BY ActualDate)",
                        maxCount);

                    command.CommandText = query;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@ActualDate";
                    parameter.Value = toDate;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@ComponentId";
                    parameter.Value = componentId;
                    command.Parameters.Add(parameter);

                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}
