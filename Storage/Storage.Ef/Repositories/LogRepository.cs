using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Zidium.Api.Dto;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class LogRepository : ILogRepository
    {
        public LogRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(LogForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.Logs.Add(new DbLog()
                {
                    Id = entity.Id,
                    Context = entity.Context,
                    ComponentId = entity.ComponentId,
                    Date = entity.Date,
                    Level = entity.Level,
                    Message = entity.Message,
                    Order = entity.Order,
                    ParametersCount = entity.ParametersCount,
                    Parameters = entity.Properties.Select(t => new DbLogProperty()
                    {
                        Id = t.Id,
                        LogId = t.LogId,
                        DataType = t.DataType,
                        Name = t.Name,
                        Value = t.Value
                    }).ToArray()
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Add(LogForAdd[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.ChangeTracker.AutoDetectChangesEnabled = false;
                try
                {
                    foreach (var entity in entities)
                    {
                        contextWrapper.Context.Logs.Add(new DbLog()
                        {
                            Id = entity.Id,
                            Context = entity.Context,
                            ComponentId = entity.ComponentId,
                            Date = entity.Date,
                            Level = entity.Level,
                            Message = entity.Message,
                            Order = entity.Order,
                            ParametersCount = entity.ParametersCount,
                            Parameters = entity.Properties.Select(t => new DbLogProperty()
                            {
                                Id = t.Id,
                                LogId = t.LogId,
                                DataType = t.DataType,
                                Name = t.Name,
                                Value = t.Value
                            }).ToArray()
                        });
                    }
                }
                finally
                {
                    contextWrapper.Context.ChangeTracker.AutoDetectChangesEnabled = true;
                }
                contextWrapper.Context.SaveChanges();
            }
        }

        public LogForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public LogForRead[] Find(
            int maxCount,
            Guid componentId,
            DateTime? fromDate,
            DateTime? toDate,
            LogLevel[] importanceLevels,
            string context,
            string message,
            string propertyName,
            string propertyValue)
        {
            using (_storage.GetContextWrapper())
            {
                if (importanceLevels == null)
                {
                    importanceLevels = new LogLevel[0];
                }

                var query = GetRecordsQuery(componentId, fromDate, toDate, importanceLevels.ToArray(), context, message, propertyName, propertyValue);

                if (maxCount < 1 || maxCount > 1000)
                {
                    maxCount = 1000;
                }

                query = query.OrderBy(t => t.Date).ThenBy(t => t.Order).Take(maxCount);

                return query.AsEnumerable().Select(DbToEntity).ToArray();
            }
        }

        public int DeleteLogProperties(int maxCount, DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var subQuery = contextWrapper.Context.Logs
                    .Where(t => t.Date < toDate).OrderBy(t => t.Date).Select(t => t.Id).Take(maxCount);

                var objectQuery = contextWrapper.Context.LogProperties
                    .Where(t => subQuery.Contains(t.LogId)).OrderBy(t => t.Id).Select(t => t.Id).Take(maxCount).ToParametrizedSql();

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

                        var query = $"DELETE FROM {contextWrapper.Context.FormatTableName("LogParameters")} WHERE {contextWrapper.Context.FormatColumnName("Id")} IN ({objectQuery.Sql})";

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

        public int DeleteLogs(int maxCount, DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var objectQuery = contextWrapper.Context.Logs
                    .Where(t => t.Date < toDate).OrderBy(t => t.Date).Select(t => t.Id).Take(maxCount).ToParametrizedSql();

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

                        var query = $"DELETE FROM {contextWrapper.Context.FormatTableName("Logs")} WHERE {contextWrapper.Context.FormatColumnName("Id")} IN ({objectQuery.Sql})";

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

        public LogForRead[] GetFirstRecords(
            Guid componentId,
            DateTime? fromDate,
            LogLevel[] importanceLevels,
            string context,
            int maxCount)
        {
            using (_storage.GetContextWrapper())
            {
                var query = GetRecordsQuery(componentId, fromDate, null, importanceLevels, context, null, null, null);
                query = query.OrderBy(t => t.Date).ThenBy(t => t.Order).Take(maxCount);
                return query.AsEnumerable().Select(DbToEntity).ToArray();
            }
        }

        public LogForRead[] GetLastRecords(
            Guid componentId,
            DateTime? toDate,
            LogLevel[] importanceLevels,
            string context,
            int maxCount)
        {
            using (_storage.GetContextWrapper())
            {
                var query = GetRecordsQuery(componentId, null, toDate, importanceLevels, context, null, null, null);
                query = query.OrderByDescending(t => t.Date).ThenByDescending(t => t.Order).Take(maxCount);
                query = query.OrderBy(t => t.Date).ThenBy(t => t.Order);
                return query.AsEnumerable().Select(DbToEntity).ToArray();
            }
        }

        public LogForRead[] GetPreviousRecords(
            Guid componentId,
            DateTime date,
            int order,
            LogLevel[] importanceLevels,
            string context,
            int maxCount)
        {
            using (_storage.GetContextWrapper())
            {
                var query = GetRecordsQuery(componentId, null, null, importanceLevels, context, null, null, null);
                var sameDateQuery = query.Where(t => t.Date == date && t.Order < order).OrderByDescending(t => t.Date).ThenByDescending(t => t.Order).Take(maxCount);
                var prevDateQuery = query.Where(t => t.Date < date).OrderByDescending(t => t.Date).ThenByDescending(t => t.Order).Take(maxCount);
                var result = sameDateQuery.Concat(prevDateQuery);
                result = result.OrderByDescending(t => t.Date).ThenByDescending(t => t.Order).Take(maxCount);
                result = result.OrderBy(t => t.Date).ThenBy(t => t.Order);
                return result.AsEnumerable().Select(DbToEntity).ToArray();
            }
        }

        public LogForRead[] GetNextRecords(
            Guid componentId,
            DateTime date,
            int order,
            LogLevel[] importanceLevels,
            string context,
            int maxCount)
        {
            using (_storage.GetContextWrapper())
            {
                var query = GetRecordsQuery(componentId, null, null, importanceLevels, context, null, null, null);
                var sameDateQuery = query.Where(t => t.Date == date && t.Order > order).OrderBy(t => t.Date).ThenBy(t => t.Order).Take(maxCount);
                var nextDateQuery = query.Where(t => t.Date > date).OrderBy(t => t.Date).ThenBy(t => t.Order).Take(maxCount);
                var result = sameDateQuery.Concat(nextDateQuery);
                result = result.OrderBy(t => t.Date).ThenBy(t => t.Order).Take(maxCount);
                return result.AsEnumerable().Select(DbToEntity).ToArray();
            }
        }

        public LogSearchResult FindPreviousRecordByText(
            string text,
            Guid componentId,
            DateTime date,
            int order,
            LogLevel[] importanceLevels,
            string context,
            int maxCount)
        {
            using (_storage.GetContextWrapper())
            {
                text = text.ToLowerInvariant();

                // Сначала поищем в рамках текущей секунды, но с меньшим Order
                // Это быстро, поэтому top не нужен

                var query = GetRecordsQuery(componentId, null, null, importanceLevels, context, null, null, null);
                var result = query
                    .Include("Parameters")
                    .Where(t => t.Date == date && t.Order < order &&
                        (t.Message.ToLower().Contains(text) || t.Parameters.Any(x => x.Name.ToLower().Contains(text) || x.Value.ToLower().Contains(text))))
                    .OrderByDescending(t => t.Date).ThenByDescending(t => t.Order).FirstOrDefault();

                if (result != null)
                    return new LogSearchResult()
                    {
                        Found = true,
                        Record = DbToEntity(result)
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
                        Record = DbToEntity(result)
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
                    Record = DbToEntity(lastRecord)
                };
            }
        }

        public LogSearchResult FindNextRecordByText(
            string text,
            Guid componentId,
            DateTime date,
            int order,
            LogLevel[] importanceLevels,
            string context,
            int maxCount)
        {
            using (_storage.GetContextWrapper())
            {
                text = text.ToLowerInvariant();

                // Сначала поищем в рамках текущей секунды, но с большим Order
                // Это быстро, поэтому top не нужен

                var query = GetRecordsQuery(componentId, null, null, importanceLevels, context, null, null, null);
                var result = query
                    .Include("Parameters")
                    .Where(t => t.Date == date && t.Order > order &&
                        (t.Message.ToLower().Contains(text) || t.Parameters.Any(x => x.Name.ToLower().Contains(text) || x.Value.ToLower().Contains(text))))
                    .OrderBy(t => t.Date).ThenBy(t => t.Order).FirstOrDefault();

                if (result != null)
                    return new LogSearchResult()
                    {
                        Found = true,
                        Record = DbToEntity(result)
                    };

                // Если в текущей секунде не нашли, то ищем в блоке из N записей
                // Сначала проверим, есть ли вообще такие записи
                // Условие с DateTime.UtcNow нужно, чтобы не получить бесконечный цикл при постоянном добавлении новых записей
                var now = DateTime.UtcNow;
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
                    .Where(t => t.Message.Contains(text) || t.Parameters.Any(x => x.Name.Contains(text) || x.Value.Contains(text)))
                    .OrderBy(t => t.Date).ThenBy(t => t.Order)
                    .FirstOrDefault();

                // Возвращаем запись, если нашли

                if (result != null)
                    return new LogSearchResult()
                    {
                        Found = true,
                        Record = DbToEntity(result)
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
                    Record = DbToEntity(lastRecord)
                };
            }
        }

        private DbLog DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Logs.Find(id);
            }
        }

        private DbLog DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Настройка {id} не найдена");

            return result;
        }

        private LogForRead DbToEntity(DbLog entity)
        {
            if (entity == null)
                return null;

            return new LogForRead(entity.Id, entity.ComponentId, entity.Level, entity.Date, entity.Order,
                entity.Message, entity.ParametersCount, entity.Context);
        }

        private IQueryable<DbLog> GetRecordsQuery(Guid componentId, DateTime? fromDate, DateTime? toDate, LogLevel[] importanceLevels, string context, string message, string propertyName, string propertyValue)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Logs.AsNoTracking().Where(x => x.ComponentId == componentId);

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
                    query = query.Where(x => x.Context.ToLower().StartsWith(context.ToLower()));
                }

                if (!string.IsNullOrEmpty(message))
                {
                    query = query.Where(x => x.Message.ToLower().Contains(message.ToLower()));
                }

                if (!string.IsNullOrEmpty(propertyName) && string.IsNullOrEmpty(propertyValue))
                {
                    query = query.Where(x => x.Parameters.Any(t => t.Name.ToLower().Contains(propertyName.ToLower())));
                }
                else if (!string.IsNullOrEmpty(propertyValue) && string.IsNullOrEmpty(propertyName))
                {
                    query = query.Where(x => x.Parameters.Any(t => t.Value.ToLower().Contains(propertyValue.ToLower())));
                }
                else if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
                {
                    query = query.Where(x => x.Parameters.Any(t =>
                        t.Name.ToLower().Contains(propertyName.ToLower()) &&
                        t.Value.ToLower().Contains(propertyValue.ToLower())));
                }

                return query;
            }
        }
    }
}
