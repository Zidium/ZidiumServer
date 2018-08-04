using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    /// <inheritdoc />
    public class LogRepository : ILogRepository
    {
        protected AccountDbContext Context;

        public LogRepository(AccountDbContext context)
        {
            Context = context;
        }

        public Log Add(Log log)
        {
            Context.Logs.Add(log);
            return log;
        }

        public List<Log> AddLogs(List<Log> logs)
        {
            foreach (var log in logs)
            {
                Context.Logs.Add(log);
            }
            Context.SaveChanges();
            return logs;
        }

        public List<Log> Find(
            int maxCount,
            Guid componentId,
            DateTime? fromDate,
            DateTime? toDate,
            IEnumerable<LogLevel> importanceLevels,
            string context)
        {
            if (importanceLevels == null)
            {
                importanceLevels = new LogLevel[0];
            }

            var query = GetRecordsQuery(componentId, fromDate, toDate, importanceLevels.ToArray(), context);

            if (maxCount < 1 || maxCount > 1000)
            {
                maxCount = 1000;
            }

            query = query.OrderBy(t => t.Date).ThenBy(t => t.Order).Take(maxCount);

            var result = query.ToList();
            return result;
        }

        public IQueryable<Log> GetFirstRecords(Guid componentId, DateTime? fromDate, LogLevel[] importanceLevels, string context, int maxCount)
        {
            var query = GetRecordsQuery(componentId, fromDate, null, importanceLevels, context);
            query = query.OrderBy(t => t.Date).ThenBy(t => t.Order).Take(maxCount);
            return query;
        }

        public IQueryable<Log> GetLastRecords(Guid componentId, DateTime? toDate, LogLevel[] importanceLevels, string context, int maxCount)
        {
            var query = GetRecordsQuery(componentId, null, toDate, importanceLevels, context);
            query = query.OrderByDescending(t => t.Date).ThenByDescending(t => t.Order).Take(maxCount);
            query = query.OrderBy(t => t.Date).ThenBy(t => t.Order);
            return query;
        }

        public IQueryable<Log> GetPreviousRecords(Guid componentId, DateTime date, int order, LogLevel[] importanceLevels, string context, int maxCount)
        {
            var query = GetRecordsQuery(componentId, null, null, importanceLevels, context);
            var sameDateQuery = query.Where(t => t.Date == date && t.Order < order).OrderByDescending(t => t.Date).ThenByDescending(t => t.Order).Take(maxCount);
            var prevDateQuery = query.Where(t => t.Date < date).OrderByDescending(t => t.Date).ThenByDescending(t => t.Order).Take(maxCount);
            var result = sameDateQuery.Concat(prevDateQuery);
            result = result.OrderByDescending(t => t.Date).ThenByDescending(t => t.Order).Take(maxCount);
            result = result.OrderBy(t => t.Date).ThenBy(t => t.Order);
            return result;
        }

        public IQueryable<Log> GetNextRecords(Guid componentId, DateTime date, int order, LogLevel[] importanceLevels, string context, int maxCount)
        {
            var query = GetRecordsQuery(componentId, null, null, importanceLevels, context);
            var sameDateQuery = query.Where(t => t.Date == date && t.Order > order).OrderBy(t => t.Date).ThenBy(t => t.Order).Take(maxCount);
            var nextDateQuery = query.Where(t => t.Date > date).OrderBy(t => t.Date).ThenBy(t => t.Order).Take(maxCount);
            var result = sameDateQuery.Concat(nextDateQuery);
            result = result.OrderBy(t => t.Date).ThenBy(t => t.Order).Take(maxCount);
            return result;
        }

        public LogSearchResult FindPreviousRecordByText(string text, Guid componentId, DateTime date, int order, LogLevel[] importanceLevels, string context, int maxCount)
        {
            text = text.ToLowerInvariant();

            // Сначала поищем в рамках текущей секунды, но с меньшим Order
            // Это быстро, поэтому top не нужен

            var query = GetRecordsQuery(componentId, null, null, importanceLevels, context);
            var result = query
                .Include("Parameters")
                .Where(t => t.Date == date && t.Order < order && 
                    (t.Message.ToLower().Contains(text) || t.Parameters.Any(x => x.Name.ToLower().Contains(text) || x.Value.ToLower().Contains(text))))
                .OrderByDescending(t => t.Date).ThenByDescending(t => t.Order).FirstOrDefault();

            if (result != null)
                return new LogSearchResult()
                {
                    Found = true,
                    Record = result
                };

            // Если в текущей секунде не нашли, то ищем в блоке из N записей
            // Сначала проверим, есть ли вообще такие записи
            var iterationQuery = query.Where(t => t.Date < date);

            var hasAnyRecords = iterationQuery.Any();

            // Если записей нет, поиск закончен
            if (!hasAnyRecords)
                return new LogSearchResult()
                {
                    Found = false
                };

            // Ищем в блоке из N записей

            result = iterationQuery
                .OrderByDescending(t => t.Date).ThenByDescending(t => t.Order)
                .Take(maxCount)
                .Include("Parameters")
                .Where(t => t.Message.ToLower().Contains(text) || t.Parameters.Any(x => x.Name.ToLower().Contains(text) || x.Value.ToLower().Contains(text)))
                .OrderByDescending(t => t.Date).ThenByDescending(t => t.Order)
                .FirstOrDefault();

            // Возвращаем запись, если нашли

            if (result != null)
                return new LogSearchResult()
                {
                    Found = true,
                    Record = result
                };

            // Если не нашли, то возвращаем Id последней записи блока, чтобы с неё можно было продолжить поиск

            var lastRecord = iterationQuery
                .OrderByDescending(t => t.Date).ThenByDescending(t => t.Order)
                .Take(maxCount)
                .OrderBy(t => t.Date).ThenBy(t => t.Order)
                .First();

            return new LogSearchResult()
            {
                Found = false,
                Record = lastRecord
            };
        }

        public LogSearchResult FindNextRecordByText(string text, Guid componentId, DateTime date, int order, LogLevel[] importanceLevels, string context, int maxCount)
        {
            text = text.ToLowerInvariant();

            // Сначала поищем в рамках текущей секунды, но с большим Order
            // Это быстро, поэтому top не нужен

            var query = GetRecordsQuery(componentId, null, null, importanceLevels, context);
            var result = query
                .Include("Parameters")
                .Where(t => t.Date == date && t.Order > order &&
                    (t.Message.ToLower().Contains(text) || t.Parameters.Any(x => x.Name.ToLower().Contains(text) || x.Value.ToLower().Contains(text))))
                .OrderBy(t => t.Date).ThenBy(t => t.Order).FirstOrDefault();

            if (result != null)
                return new LogSearchResult()
                {
                    Found = true,
                    Record = result
                };

            // Если в текущей секунде не нашли, то ищем в блоке из N записей
            // Сначала проверим, есть ли вообще такие записи
            // Условие с DateTime.Now нужно, чтобы не получить бесконечный цикл при постоянном добавлении новых записей
            var now = DateTime.Now;
            var iterationQuery = query.Where(t => t.Date > date && t.Date <= now);

            var hasAnyRecords = iterationQuery.Any();

            // Если записей нет, поиск закончен
            if (!hasAnyRecords)
                return new LogSearchResult()
                {
                    Found = false
                };

            // Ищем в блоке из N записей

            result = iterationQuery
                .OrderBy(t => t.Date).ThenBy(t => t.Order)
                .Take(maxCount)
                .Include("Parameters")
                .Where(t => t.Message.ToLower().Contains(text) || t.Parameters.Any(x => x.Name.ToLower().Contains(text) || x.Value.ToLower().Contains(text)))
                .OrderBy(t => t.Date).ThenBy(t => t.Order)
                .FirstOrDefault();

            // Возвращаем запись, если нашли

            if (result != null)
                return new LogSearchResult()
                {
                    Found = true,
                    Record = result
                };

            // Если не нашли, то возвращаем Id последней записи блока, чтобы с неё можно было продолжить поиск

            var lastRecord = iterationQuery
                .OrderBy(t => t.Date).ThenBy(t => t.Order)
                .Take(maxCount)
                .OrderByDescending(t => t.Date).ThenByDescending(t => t.Order)
                .First();

            return new LogSearchResult()
            {
                Found = false,
                Record = lastRecord
            };
        }

        private IQueryable<Log> GetRecordsQuery(Guid componentId, DateTime? fromDate, DateTime? toDate, LogLevel[] importanceLevels, string context)
        {
            var query = Context.Logs.Where(x => x.ComponentId == componentId);

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.Date >= fromDate);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.Date < toDate);
            }

            if (importanceLevels != null && importanceLevels.Length > 0)
            {
                query = query.Where(x => importanceLevels.Contains(x.Level));
            }

            if (!string.IsNullOrEmpty(context))
            {
                query = query.Where(x => x.Context.StartsWith(context));
            }

            return query;
        }

        public Log Find(Guid id, Guid componentId)
        {
            var result = Context.Logs.Find(id);
            if (result == null)
                throw new ObjectNotFoundException(id, Naming.Log);
            if (result.ComponentId != componentId)
                throw new AccessDeniedException(id, Naming.Log);
            return result;
        }

        public int DeleteLogProperties(int maxCount, DateTime toDate)
        {
            var subQuery = Context.Logs
                .Where(t => t.Date < toDate).OrderBy(t => t.Date).Select(t => t.Id).Take(maxCount);

            var objectQuery = (ObjectQuery) ((IObjectContextAdapter) Context).ObjectContext.CreateObjectSet<LogProperty>()
                .Where(t => subQuery.Contains(t.LogId)).OrderBy(t => t.Id).Select(t => t.Id).Take(maxCount);

            using (var connection = Context.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;

                    var query = $"DELETE FROM {Context.FormatTableName("LogParameters")} WHERE {Context.FormatColumnName("Id")} IN ({objectQuery.ToTraceString()})";

                    command.CommandText = query;

                    foreach (var objectParameter in objectQuery.Parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = objectParameter.Name;
                        parameter.Value = objectParameter.Value;
                        command.Parameters.Add(parameter);
                    }

                    return command.ExecuteNonQuery();
                }
            }
        }

        public int DeleteLogs(int maxCount, DateTime toDate)
        {
            var objectQuery = (ObjectQuery)((IObjectContextAdapter)Context).ObjectContext.CreateObjectSet<Log>()
                .Where(t => t.Date < toDate).OrderBy(t => t.Date).Select(t => t.Id).Take(maxCount);

            using (var connection = Context.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;

                    var query = $"DELETE FROM {Context.FormatTableName("Logs")} WHERE {Context.FormatColumnName("Id")} IN ({objectQuery.ToTraceString()})";

                    command.CommandText = query;

                    foreach (var objectParameter in objectQuery.Parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = objectParameter.Name;
                        parameter.Value = objectParameter.Value;
                        command.Parameters.Add(parameter);
                    }

                    return command.ExecuteNonQuery();
                }
            }
        }

        public IQueryable<Log> QueryAll(Guid[] componentIds)
        {
            return Context.Logs.Where(t => componentIds.Contains(t.ComponentId));
        }

        public IQueryable<Log> QueryAll(Guid componentId)
        {
            return Context.Logs.Where(t => t.ComponentId == componentId);
        }

        public Log Update(Log entity)
        {
            Context.SaveChanges();
            return entity;
        }

        public void Remove(Log entity)
        {
            Context.Logs.Remove(entity);
            Context.SaveChanges();
        }

        public void Remove(Guid id, Guid componentId)
        {
            var entity = Find(id, componentId);
            Remove(entity);
        }

    }
}
